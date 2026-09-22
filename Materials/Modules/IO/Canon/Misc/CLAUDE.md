# Canon EDSDK 模块维护记录

> 位置：`Assets/ToneTuneToolkit/Modules/IO/Canon/`
> 参考资料：`D:\Workflow\_CanonRef\`（含 EDSDK 13.19.0 API 文档与官方 C# Sample `CameraControl`）

## 版本记账约定（2026-09-22）

源文件头的 `Code Version` **不再跟随改动维护**（用户确认）；变更以本文件的记录为准。

## 模块结构

| 文件 | 职责 |
|---|---|
| `Scripts/CanonSession.cs` | 核心：单例会话管理、STA 工作线程、事件泵、LiveView 取帧、拍照与下载 |
| `Scripts/CanonTestPanel.cs` | OnGUI 测试面板（连接 / LiveView 开关 / 拍照 / 断开） |
| `Scripts/CanonLiveViewRenderer.cs` | LiveView JPEG 解码 → Texture2D → Spout 发送 |
| `EDSDK/EDSDKAPI.cs` | EDSDK P/Invoke 封装 |
| `EDSDK/EDSDKConsts.cs` | 命令 / 属性 / 事件 / 错误码常量 |

## 线程模型（必须遵守的不变量）

- 全部 EDSDK 调用都在**同一条**后台 STA 线程（`COINIT_APARTMENTTHREADED`）上串行执行；
- 工作线程每轮循环：**① `EdsGetEvent()` 泵事件 → ② KeepAlive → ③ 执行一个队列任务**；
- **每轮只执行一个任务**（与官方 Sample 的 `CommandProcessor` 一致）。严禁改回“批处理排空队列”——见下方事故记录；
- `LiveViewTick` 通过“执行完把自己排回队尾”实现自续，帧率由 `Thread.Sleep(10)` 节流；
- EDSDK 回调（HandleObjectEvent 等）由 `EdsGetEvent()` 在工作线程内同步派发，回调里只投递任务/事件，不直接长耗时操作；
- Unity 主线程 API 一律经 `_main` 队列在 `Update()` 中执行；
- **工作线程上的日志直接用 `Debug.Log(@$"[Canon Session] ... at {DateTime.Now}")`，且插值只能是纯数值/字符串**：禁止 `{name}`、`{tag}` 等 `UnityEngine.Object` 原生属性插值（`GetName`/`GetTag` 仅主线程可用）。2026-09 曾因 `DownloadPhoto` 里的 `[{name}]` 抛 `CanonWorker: GetName` 错，并连带跳过 `OnPhotoSaved` 与 `_downloading=false`，导致后续照片全部被静默取消；心跳循环里的同类写法更会直接杀死工作线程。（此前的静态 `Log()` 包装已于 2026-09-22 按用户要求移除，统一为内联 `Debug.Log`）

## 事故记录：LiveView 中拍照不返图，停止 LiveView 才返图（2026-09-21 已修复）

### 症状
连接相机 → 开启 LiveView → 拍照：相机有咔嚓声但 `OnPhotoSaved` 不触发；手动点“停止 LiveView”后照片才下载回来。

### 根因
工作线程的任务处理处曾是“取出一个任务后，再用 `while (_jobs.TryTake(out j, 0))` 把队列**一口气排空**”的批处理写法。而 `LiveViewTick` 在返回前会把自己重新入队（自续），于是：

1. LiveView 一开启，内层排空循环取到 `LiveViewTick` → 执行 → 它又把自己放回队列 → 立刻又取到……**内层 while 永不退出**（活锁）；
2. 外层循环体顶部的 `EdsGetEvent()` 事件泵与 KeepAlive 从此**一次都执行不到**；
3. 拍照命令 `DoTakePhoto` 是普通任务、仍能被执行（所以相机真的拍了），但 SDK 发出的 `kEdsObjectEvent_DirItemRequestTransfer`（0x208 传图请求）**无人派发**，下载流程永不启动；
4. 停止 LiveView 后 `_liveViewActive=false`，`LiveViewTick` 不再自续 → 队列瞬间排空 → 内层循环退出 → `EdsGetEvent()` 恢复泵送 → 积压的传图事件被派发 → 照片立即下载。与症状逐字吻合。

### 修复
删除内层排空循环，改为**每轮只执行一个任务**（`CanonSession.cs` v1.6.2）。事件泵因此在每个 LiveView 帧之间都会运行（约 10–40ms 一次），KeepAlive 饥饿问题同时修复。

### 教训
- 在有“自续任务”的队列模型里，任何形式的“排空批处理”都会造成事件泵饥饿；任务消费速率必须与泵送节奏交错；
- EDSDK 的对象/状态回调依赖 `EdsGetEvent()` 泵送（本项目实测；文档亦要求 console 类程序定期调用）。任何把泵送挡住的写法都会表现为“事件迟到”而非报错，极易误诊为相机/USB 问题。

## FAQ：为什么场景重启后照片又从 IMG_0001.JPG 开始？（2026-09-21 已处理）

### 原因（相机侧行为）
- 文件名由相机分配（`EdsGetDirectoryItemInfo.szFileName`），代码只透传；
- 相机的连续文件编号靠“存储卡上已有最大编号 +1”推进并持久化；`SaveTo=Host` 时照片只缓存在相机内存直传 PC，**不落卡**，编号无从推进；
- 场景重启 → `EdsTerminateSDK` → 重启后重建会话，主机直传编号状态归零 → 又从 IMG_0001 发牌。同一会话内编号正常递增，重启即清零。

### 隐患与修复（主机侧）
旧逻辑用 `CreateAlways` 落盘，重启后的 IMG_0001.JPG 会**覆盖**上一次运行的同名文件。自 v1.6.4 起 `DownloadPhoto` 给主机侧文件名加时间戳前缀（v1.6.3 曾用“原名+时间+会话序号”后缀方案，后按需求简化）：

```
{yyyyMMddHHmmss}_{相机原名}{扩展名}
例：20260921152205_IMG_0001.JPG
```

时间戳精确到秒且位于最前——按文件名排序即按拍摄时间排序；会话内相机编号递增、跨会话时间戳不同，故不会重名。

### 备选：若希望相机编号持续递增
把 `DoOpenSession` 中的 `EdsSaveTo.Host` 改为 `EdsSaveTo.Both`（=3，卡+主机双存）：照片同时落卡，相机编号随卡推进、跨会话连续，且卡上留档；代价是消耗卡空间、需插卡。相机菜单“文件编号”需设为“连续”。

## 保活（心跳）机制：待机时如何避免相机休眠

EDSDK 没有底层"心跳包"，官方等价物是命令 **`kEdsCameraCommand_ExtendShutDownTimer`**（文档描述即 *Keep Device On*）：每次发送把相机"自动电源关闭"的倒计时重置为菜单设定时长。本项目实现为双保险（v1.6.5）：

1. **周期心跳**（`CanonSession` 工作线程循环步骤②）：会话打开期间每 `KeepAwakeIntervalSeconds`（默认 25s，Inspector 可调）发送一次 `ExtendShutDownTimer`；会话建立 2 秒后发首拍（避开初期 BUSY）。心跳瞬间的 `DEVICE_BUSY`（如恰逢拍照/下载）容忍并等下个周期，不升级为错误。
2. **预警兜底**：`kEdsStateEvent_WillSoonShutDown`（0x303，`inParameter`=距关机剩余秒数，仅机身菜单开启自动关机时才会发）——收到即在 `HandleStateEvent` 里经任务队列立即补发一次续期（`DoKeepAliveBeat`），同时重置周期计时。用于周期心跳偶发失败、或相机自动关机档位短于心跳间隔的场合。

注意事项：
- 心跳间隔必须**小于**相机"自动电源关闭"菜单档位（常见最短 30 秒，故默认 25 秒留余量）；若菜单可设"禁用"，则相机不会自动关机、预警事件也不会发；
- 待机（仅开会话、不开 LiveView）与 LiveView 中心跳都在跑——事件泵循环本身 ~20Hz 空转，到点即发；
- 不要在 SDK 事件回调内直接调 `EdsSendCommand`（回调由 `EdsGetEvent` 在工作线程内同步派发，嵌套 SDK 调用有风险），兜底统一走 `_jobs` 队列；
- 相机 USB 供电不影响自动关机逻辑，休眠与否仍取决于 ExtendShutDownTimer。

## 已核对、与官方 Sample 一致的写法（勿“顺手优化”）

- 拍照：`PressShutterButton_Completely` 后**立即** `ShutterButton_OFF`（官方 `TakePictureCommand` 即背靠背）；
- 停止 LiveView：仅清 `Evf_OutputDevice` 的 `EvfOutputDevice_PC` 位，不回写 `Evf_Mode=0`（官方 `EndEvfCommand` 同）；
- EVF 取帧：`EdsCreateMemoryStream` → `EdsCreateEvfImageRef` → `EdsDownloadEvfImage`，`OBJECT_NOTREADY / DEVICE_BUSY` 时丢帧重试（官方 `DownloadEvfCommand` 同）；
- 传图：`DirItemRequestTransfer` → `EdsGetDirectoryItemInfo` → `EdsCreateFileStream` → `EdsDownload` → `EdsDownloadComplete` → 两次 `EdsRelease`；
- `SaveTo=Host` + `EdsSetCapacity` 大容量假值，保证照片回传主机而非落卡。

## 事件面补齐：按 13.19.0 文档 §4.2 全集审计（2026-09-22，CartierPhotobooth 侧完成并实机测试通过，待同步回 CanonControl）

背景：以文档 §4.2 全部 19 种事件 + Sample 枚举多出的 3 种（VolumeAdded/VolumeRemoved/AfResult）= 22 种回调事件逐项核销，确认 4 个事件缺口 + 1 个处置缺陷，本次全部修复：

1. **InternalError(0x306) 拆连接**：文档 §4.2.18 要求收到即取消远程连接（设备大概率无法继续工作）。现复用 `HandleCameraShutdown` 拆会话（不终止 SDK，保留重连），并加门闸防其后的真实 Shutdown 重复触发 `OnCameraShutdown`。
2. **CaptureError(0x305) 结构化**：新增 `OnCaptureError(uint)`（错误码 0x01 拍摄失败 / 0x02 镜头收回 / 0x03 机构类错误 / 0x04 传感器清洁 / 0x05 静音操作 / 0x06 无卡 / 0x07 卡错误 / 0x08 写保护），兼发 `OnPhotoFailed` 与 `OnError`——释放失败意味着本次不会有照片回传。
3. **JobStatusChanged(0x302) + 僵尸请求处置（打包）**：
   - 新增 `OnJobStatusChanged(bool)`（是否有照片等待传回主机，文档 §4.2.11）；
   - `HandleCameraShutdown` / `DoClose` 对 pending 照片先 `EdsDownloadCancel` 再 Release（文档 §4.2.12 强制，原实现只 Release 不 Cancel，遗留僵尸请求 → 下次会话下载必 0x2）；
   - `DownloadPhoto` 全部失败路径同样补 Cancel（0x2 僵尸自愈）。
4. **拍照失败信号 `OnPhotoFailed(string)`**：触发点——快门命令报错 / CaptureError / 下载失败 / 传输中并发取消（原先静默丢照片、无任何事件）。
   - **应用层需跟进**：ShotManager 等订阅 `OnPhotoFailed` 才能避免拍照失败时 UI 空等（本次仅改模块）。
   - `DoTakePhoto` 中"AvailableShots 前后不降 = 快门被静默忽略"检测**仅记日志不发事件**——SaveTo=Host 无卡时该值是否递减未实测，确认可靠后才可升级为触发点。
5. 观测性：`HandleStateEvent` 未识别状态事件补 log（与对象事件对称）；保活失败日志带回错误码；`EDSDKConsts` 补 6 个缺失常量（DirItemInfoChanged / DirItemContentChanged / DirItemCancelTransferDT / VolumeAdded / VolumeRemoved / AfResult）。

**实机测试（2026-09-22，60D）：通过。** 正常链路（连接 / LiveView / 拍照 / 下载）无回归，新增事件未产生误报，AvailableShots 静默忽略检测在正常拍照下未误刷（维持 log-only 策略）。

**实测确认（2026-09-22，60D）**：
- **JobStatusChanged 会发出**，时序与文档 §4.2.11 一致：拍照 → 「有待传照片」→ `Photo downloaded` → 「无待传照片」，照片实际收到。「type1 协议机型不发此事件」的担忧对 60D 不成立；该事件可作为可靠依据用于：关会话前的待传排空判断、拍照流程的辅助状态信号。

**待测试**：
- 盖镜头盖 / 摘镜头触发 CaptureError 的失败路径（`OnCaptureError` / `OnPhotoFailed` / TestPanel 橙红日志的表现）；
- InternalError 后相机实际行为（是否随之 Shutdown、能否直接 Open 重连）——难以人为触发，机会性观察。

**明确未做**（第二梯队，留 CanonControl 排期）：电量/温度/Evf_OutputDevice 属性监控、下载进度、SaveTo/SetCapacity 失败校验。

**审计基准**：全集 22 种事件中 5 种已正确处理、14 种确认 N/A 且处置正确（未处理对象事件均正确 Release）、3 种为本轮补齐。`BulbExposureTime` 为文档修订史幽灵条目（EDSDK 2.0 时代），13.19 参考表与 Sample 常量中皆无。

## 排查工具

- `CanonTestPanel` 回调日志（连接 / LiveView / 拍照保存 / 错误均有推送）；
- `Debug.Log("[Canon] ...")` 前缀日志，含每次 SDK 调用的 err 码（hex）。常见错误码：`0x81 DEVICE_BUSY`、`0xA102 OBJECT_NOTREADY`、`0x2 INTERNAL_ERROR`（编辑器内 SDK 未干净 terminate，代码已有自愈逻辑）。
