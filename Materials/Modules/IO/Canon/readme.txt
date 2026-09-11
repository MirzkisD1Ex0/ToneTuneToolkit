ToneTuneCanonBridge · Canon 模块说明

用途：在 Unity 进程内直接集成 Canon EDSDK 13.19.0，控制佳能 DSLR（LiveView / 拍照），
并把 LiveView 画面经 KlakSpout（Spout 源名 "CanonLiveView"）共享给外部 DX11 应用。

环境要求：
- NVIDIA 独显：Unity 必须运行在独显上，否则 Spout 无法串流共享 LiveView 数据
  （NVIDIA 控制面板 → 管理 3D 设置）。
- Unity 2022.3.30f1 + KlakSpout 2.0.6 已验证可用。

关键实现要点：
- EDSDK 必须持续调用 EdsGetEvent() 泵送事件，否则拍照回传事件收不到、下载报 0x2。
- 所有 EDSDK 调用集中在单工作线程；事件经 ConcurrentQueue 转主线程（Update 消费）。
- 拍照 SaveTo=Host，图片经 DirItemRequestTransfer 事件下载，落盘 StreamingAssets，OnPhotoSaved 回调返回路径。
- 详见 MEMORY.md / CLAUDE.md。

参考：https://github.com/keijiro/KlakSpout/issues/81
