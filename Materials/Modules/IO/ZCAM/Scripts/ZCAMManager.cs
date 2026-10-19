/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Z CAM 相机接入门面：连接会话、拍照回读、MJPEG 取景。
/// 双通道架构（详见项目 CLAUDE.md）：控制命令经 ZCAMHttpClient 串行下发（唯一会话持有者），
/// 取景流走独立的 ZCAMMjpegViewSource 长连接，两通道互不阻塞。
///
/// 调用契约：
/// - 五个生命周期操作（OpenCamera/CloseCamera/TakePhoto/StartLiveView/StopLiveView）内部经操作门
///   串行排队，可放心 fire-and-forget：`ZCAMManager.Instance.OpenCamera();` 一行即安全；
/// - 操作失败走双通道——OnError 事件（裸调用户）+ 异常（await 用户）。同一调用点二选一，别两者都用；
/// - 生命周期操作（OpenCamera/CloseCamera/TakePhoto/StartLiveView/StopLiveView/录像开关）经操作门
///   串行排队，可放心 fire-and-forget：`ZCAMManager.Instance.EZStartRecord();` 一行即安全；
/// - 操作失败走双通道——OnError 事件（裸调用户）+ 异常（await 用户）。同一调用点二选一，别两者都用；
/// - 直通命令（SetParam/GetParamRaw）为单条 HTTP，未过操作门，建议 await。
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ToneTuneToolkit.Common;
using ToneTuneToolkit.Data;
using UnityEngine;

namespace ToneTuneToolkit.IO.ZCAM
{

  /// <summary>
  /// 
  /// 保活用GetCameraStatus，轮次需要在外部写，等到相机初始化完毕
  /// yield return new WaitUntil(() => ZCAMM.IsConnected || ZCAMM.State == ZCAMConnectionState.Disconnected);
  /// </summary>
  public class ZCAMManager : SingletonMaster<ZCAMManager>
  {
    private void Update()
    {
      PumpLiveView();
    }

    private void OnDestroy()
    {
      UninitDestroy();
    }

    private void OnApplicationQuit()
    {
      UninitApplicationQuit();
    }

    // ==================================================
    #region 简易控制

    public void EZInitCamera() => StartCoroutine(EZInitCameraCoroutine());
    private IEnumerator EZInitCameraCoroutine()
    {
      OpenCamera();
      yield return new WaitUntil(() => IsConnected || State == ZCAMConnectionState.Disconnected);
      StartLiveView();
    }

    public void EZInitCameraWithFormat() => StartCoroutine(EZInitCameraWithFormatCoroutine());
    private IEnumerator EZInitCameraWithFormatCoroutine()
    {
      OpenCamera();
      yield return new WaitUntil(() => IsConnected || State == ZCAMConnectionState.Disconnected);
      FormatCard();
      StartLiveView();
    }

    #endregion
    // ==================================================
    #region 参数

    [Header("连接")]
    [SerializeField] private string cameraIP = "10.98.32.1"; // 相机 IP。网线直连 Direct 模式固定为 10.98.32.1
    private float commandTimeout = 5f; // 控制命令超时

    [Header("取景")]
    [SerializeField] private ZCAMLiveViewChannel liveViewChannel = ZCAMLiveViewChannel.SSP; // 取景通道：Ssp = libssp 原生收流 + MF 解码直灌纹理（主通道，延迟地板）；Mjpeg = 相机内置 720p（调试用）
    [SerializeField] private RenderTexture liveViewRT = null; // 取景输出 RT：拖入项目资产 RT ZCAM（1920×1080）。PumpLiveView 每帧 Blit 刷新其内容
    private float liveViewFirstFrameTimeout = 8f; // StartLiveView等待首帧的超时

    [Header("拍照回读")]
    [SerializeField] private ZCAMPhotoResolution photoResolution = ZCAMPhotoResolution.UHD; // 拍照目标分辨率：跨档会临时切 movfmt 并在拍后还原（取景流自动恢复，多花几秒）；同档直拍无扰动
    private int photoPollAttempts = 20; // 拍照后轮询 DCIM 等待新文件落卡的次数（仅在拍照响应不带路径的旧固件下使用）
    private int photoPollIntervalMs = 300; // 每次轮询的间隔（毫秒）
    [SerializeField] private bool downloadPhotoFile = true; // 是否把照片完整下载到本地 StreamingAssets/ZCAM/（4K RAW 约 60MB/张）
    [SerializeField] private bool flipSavedPhotoX = false; // 保存到 StreamingAssets 的照片按 X 轴镜像（左右翻转）后落盘；仅对 JPEG 生效（重编码 JPG q95；RAW 保留原始数据）

    #endregion
    // ==================================================
    #region 内部参数

    private static string zcamconfigPath => Path.Combine(Application.streamingAssetsPath, "ToneTuneToolkit", "Configs", "zcamconfig.json"); // 默认配置文件路径

    private ZCAMHttpClient _client;
    private IZCAMViewSource _viewSource;
    private ZCAMCameraInfo _cameraInfo;
    private Texture2D _decodeTexture;    // 帧解码落点（RGBA 直灌 / JPEG 解码），随连接销毁
    private string _activeBaseUrl;
    private readonly SemaphoreSlim _operationGate = new SemaphoreSlim(1, 1); // 生命周期操作门：串行排队执行，裸调用（fire-and-forget）即安全

    #endregion
    // ==================================================
    #region 状态与事件

    public ZCAMConnectionState State { get; private set; } = ZCAMConnectionState.Disconnected; // 当前连接状态
    public bool IsConnected => State == ZCAMConnectionState.Connected;
    public bool IsLiveViewing { get; private set; } = false;
    public ZCAMCameraInfo CameraInfo => _cameraInfo; // 已连接相机的基本信息；未连接为 null
    public Texture LiveViewTexture => liveViewRT; // 取景输出纹理（场景中拖入即存在；首帧前为黑）。RawImage.texture = ZCAMManager.Instance.LiveViewTexture 一次接线即可



    public event Action<ZCAMCameraInfo> OnCameraOpened; // 相机连接成功（含会话抢占与对时完成）
    public event Action OnCameraClosed; // 相机会话释放
    public event Action<ZCAMPhotoResult> OnPhotoCaptured; // 拍照完成且图片已回读
    public event Action<ZCAMError> OnError; // 所有操作失败与意外错误（如取景流中断）的统一出口；await 调用方无需重复订阅

    public string CurrentIP => _cameraInfo?.EthIp; // 当前生效的相机IP
    public ZCAMLiveViewChannel LiveViewChannel => liveViewChannel; // 当前取景通道
    public ZCAMPhotoResolution PhotoResolution => photoResolution; // 当前拍照目标分辨率

    /// <summary>运行时切换拍照分辨率（调试用；下一次 TakePhoto 生效）</summary>
    public void SetPhotoResolution(ZCAMPhotoResolution resolution) => photoResolution = resolution;

    /// <summary>
    /// 运行时切换取景通道（调试用）。若正在取景会先排队停止，切换后需重新 StartLiveView。
    /// </summary>
    public void SetLiveViewChannel(ZCAMLiveViewChannel channel)
    {
      if (liveViewChannel == channel) { return; }
      if (IsLiveViewing)
      {
        StopLiveView(); // 过操作门排队，StartLiveView 读取的是新值，顺序安全
      }
      liveViewChannel = channel;
    }

    #endregion
    // ==================================================
    #region 操作基础设施

    /// <summary>
    /// 异常处理
    /// </summary>
    static ZCAMManager()
    {
      // fire-and-forget 被丢弃的 Task 若带异常，GC 时会打 UnobservedTaskException 噪音；
      // 失败已由 OnError 事件与 Debug 留痕双通道报告，这里对 ZCAM 系异常标记"已观察"消掉重复噪音；
      // 其他模块的未观察异常不受影响（保留 Unity 默认日志，真实信号不吞）
      TaskScheduler.UnobservedTaskException += (s, e) =>
      {
        if (e.Exception == null)
        {
          return;
        }
        foreach (Exception inner in e.Exception.InnerExceptions)
        {
          if (inner is ZCAMException)
          {
            continue;
          }
          return; // 含任一非 ZCAM 异常 → 不消音，交给 Unity 默认日志
        }
        e.SetObserved();
      };
    }

    /// <summary>
    /// 操作门 + 失败双通道包装：排队执行；失败时 OnError 事件 + Debug 留痕 + 向 await 调用方抛异常
    /// </summary>
    private async Task<TResult> RunOperationAsync<TResult>(string context, Func<Task<TResult>> operation)
    {
      await _operationGate.WaitAsync();
      try { return await operation(); }
      catch (Exception e)
      {
        var ze = e as ZCAMException ?? new ZCAMException($"[{context}] {e.GetType().Name}: {e.Message}");
        OnError?.Invoke(new ZCAMError { Context = context, Exception = ze });
        Debug.LogWarning($"[ZCAMM] {ze.Message}");
        throw ze;
      }
      finally { _operationGate.Release(); }
    }

    private Task RunOperationAsync(string context, Func<Task> operation) { return RunOperationAsync<object>(context, async () => { await operation(); return null; }); }

    #endregion
    // ==================================================
    #region 连接

    /// <summary>
    /// 打开相机
    /// info 探测 → 抢占会话 → 对时 → 确保录像模式。
    /// ip 传 null 时使用 Inspector 里的 cameraIP。可裸调用（失败走 OnError）。
    /// </summary>
    public Task<ZCAMCameraInfo> OpenCamera(string ip = null) { return RunOperationAsync<ZCAMCameraInfo>(nameof(OpenCamera), () => OpenCameraCoreAsync(ip)); }
    private async Task<ZCAMCameraInfo> OpenCameraCoreAsync(string ip)
    {
      if (State == ZCAMConnectionState.Connecting) { throw new ZCAMException("Connection already in progress"); }
      if (State == ZCAMConnectionState.Connected)
      {
        if (string.IsNullOrEmpty(ip) || ip == CurrentIP)
        { return _cameraInfo; }
        await CleanupAsync(raiseClosedEvent: true); // 换目标 IP：先释放旧连接
      }

      var targetIp = string.IsNullOrEmpty(ip) ? cameraIP : ip;
      State = ZCAMConnectionState.Connecting;
      _activeBaseUrl = "http://" + targetIp;
      _client = new ZCAMHttpClient(targetIp, commandTimeout);

      try
      {
        _cameraInfo = await _client.GetInfoAsync();
        await _client.GrabSessionAsync();
        await _client.SyncDateTimeAsync();

        // 拍照在 rec 模式下抓帧；处于回放则切回（standby 会被任何 /ctrl/* 自动唤醒）
        var mode = ZCAMCameraModeExtensions.FromCameraString(await _client.QueryModeAsync());
        if (mode.IsPlayback())
        {
          await _client.ChangeModeAsync("to_rec");
        }
      }
      catch (Exception)
      {
        await CleanupAsync(raiseClosedEvent: false);
        throw; // 连接失败细节由 ZCAMException 携带
      }

      if (liveViewRT == null)
      {
        throw new ZCAMException("LiveView RenderTexture is not assigned. Drag 'RT ZCAM' into the ZCAMManager component in the scene.");
      }

      State = ZCAMConnectionState.Connected;
      OnCameraOpened?.Invoke(_cameraInfo);
      Debug.Log($"[ZCAMM] Camera connected. {_cameraInfo}");
      return _cameraInfo;
    }



    /// <summary>
    /// 关闭相机
    /// 停取景（若有）→ 释放会话 → 清理状态。可重复调用，可裸调用。
    /// </summary>
    public Task CloseCamera() { return RunOperationAsync(nameof(CloseCamera), CloseCameraCoreAsync); }
    private async Task CloseCameraCoreAsync()
    {
      if (State == ZCAMConnectionState.Disconnected && _client == null) { return; }
      await CleanupAsync(raiseClosedEvent: true);
    }



    /// <summary>
    /// 清理连接
    /// </summary>
    /// <param name="raiseClosedEvent"></param>
    /// <returns></returns>
    private async Task CleanupAsync(bool raiseClosedEvent)
    {
      if (IsLiveViewing) { await StopLiveViewCoreAsync(); }
      _viewSource?.Stop();
      _viewSource = null;

      if (_client != null)
      {
        try { await _client.QuitSessionAsync(); }
        catch { /* 释放会话尽力而为，不阻塞关闭流程 */ }
        _client.Dispose();
        _client = null;
      }

      DestroyDecodeTexture();

      _cameraInfo = null;
      _activeBaseUrl = null;
      IsRecording = false; // 连接消失，录制状态位归零（相机侧若仍在录，由用户在相机上停止）
      State = ZCAMConnectionState.Disconnected;
      if (raiseClosedEvent)
      {
        OnCameraClosed?.Invoke();
        Debug.Log("[ZCAMM] Camera disconnected.");
      }
    }

    #endregion
    // ==================================================
    #region 拍照

    /// <summary>
    /// 拍照并回读：设画质 → 抓帧 → 轮询 DCIM 等新文件落卡 → 下载到本地并解码。
    /// JPEG：LocalPath + Texture 为完整原图；RAW/HEIF：LocalPath 为原文件，Texture 为缩略图。
    /// 可裸调用（失败走 OnError）。
    /// </summary>
    public Task<ZCAMPhotoResult> TakePhoto(ZCAMPhotoQuality quality = ZCAMPhotoQuality.JPEG) { return RunOperationAsync(nameof(TakePhoto), () => TakePhotoCoreAsync(quality, keepOnCard: true)); }

    /// <summary>
    /// 拍照并回读，且拍后删除卡内副本（本地 StreamingAssets/ZCAM/ 为唯一留存）。
    /// 仅在本地下载成功后才删卡；下载被禁用（downloadPhotoFile=false）时不删并保留卡内原件。
    /// 可裸调用（失败走 OnError）。
    /// </summary>
    public Task<ZCAMPhotoResult> TakePhotoWithNoCardSave(ZCAMPhotoQuality quality = ZCAMPhotoQuality.JPEG) { return RunOperationAsync(nameof(TakePhotoWithNoCardSave), () => TakePhotoCoreAsync(quality, keepOnCard: false)); }

    private async Task<ZCAMPhotoResult> TakePhotoCoreAsync(ZCAMPhotoQuality quality, bool keepOnCard)
    {
      EnsureConnected();

      // ── 分辨率档位协商：跨档临时切 movfmt，拍后还原 ──
      string targetFmt = photoResolution.ToMovfmt();
      string originalFmt = await _client.GetParamValueAsync(ZCAMParamKey.RecordFormat);
      bool switched = false;
      if (originalFmt != targetFmt)
      {
        await _client.SetParamAsync(ZCAMParamKey.RecordFormat, targetFmt);
        if (!await WaitMovfmtAsync(targetFmt, 3f))
        {
          throw new ZCAMException($"Failed to switch capture format to {targetFmt} (still {originalFmt})");
        }
        await Task.Delay(400); // 编码器切换稳定余量
        switched = true;
      }

      ZCAMPhotoResult result;
      try { result = await CaptureAndReadbackAsync(quality); }
      finally
      {
        if (switched)
        {
          // 还原拍摄档；若取景中，重置的 Stream 1 需要重启取景流（Core 版避免操作门重入死锁）
          await _client.SetParamAsync(ZCAMParamKey.RecordFormat, originalFmt);
          await WaitMovfmtAsync(originalFmt, 3f);
          if (IsLiveViewing)
          {
            await StopLiveViewCoreAsync();
            await StartLiveViewCoreAsync();
          }
        }
      }

      OnPhotoCaptured?.Invoke(result);
      Debug.Log($"[ZCAMM] Photo captured: {result}");

      // 卡内副本清理：仅当本地已落盘才删（安全方向——删除失败/跳过时卡内留原件）
      if (!keepOnCard)
      {
        if (result.LocalPath != null)
        {
          await _client.DeleteFileAsync(result.RemotePath);
          Debug.Log($"[ZCAMM] Card copy deleted: {result.RemotePath}");
        }
        else
        {
          Debug.LogWarning("[ZCAMM] Card copy kept: local download was disabled (downloadPhotoFile=false)");
        }
      }

      return result;
    }



    /// <summary>设画质 → 抓帧 → 定位新文件（优先响应路径，回退 DCIM 轮询）→ 下载解码</summary>
    private async Task<ZCAMPhotoResult> CaptureAndReadbackAsync(ZCAMPhotoQuality quality)
    {
      await _client.SetParamAsync(ZCAMParamKey.PhotoQuality, quality.ToParamValue());
      string capMsg = await _client.CaptureStillPathAsync();

      // 快路径：新固件的拍照响应 msg 直接带路径（"/tmp/mnt/sata/DCIM/..." → "/DCIM/..."）
      string remotePath = null;
      int dcimIdx = capMsg.IndexOf("/DCIM/", StringComparison.Ordinal);
      if (dcimIdx >= 0)
      {
        remotePath = capMsg.Substring(dcimIdx);
      }
      else
      {
        // 回退：老行为——基线对比 + 轮询 DCIM 找新文件
        var baseline = await GetNewestEntryAsync();
        // 拍照指令已发出（上面的 cap），直接轮询
        string newFolder = null, newFile = null;
        for (int i = 0; i < photoPollAttempts; i++)
        {
          await Task.Delay(photoPollIntervalMs);
          var entry = await GetNewestEntryAsync();
          if (entry.folder == null)
          {
            continue;
          }
          if (baseline.folder == null
              || string.CompareOrdinal(entry.folder, baseline.folder) > 0
              || (entry.folder == baseline.folder && string.CompareOrdinal(entry.file, baseline.file) > 0))
          {
            newFolder = entry.folder;
            newFile = entry.file;
            break;
          }
        }
        if (newFile == null)
        {
          throw new ZCAMException($"Capture accepted, but no new file found within {photoPollAttempts} polls. Card write may be incomplete");
        }
        remotePath = $"/DCIM/{newFolder}/{newFile}";
      }

      var result = new ZCAMPhotoResult
      {
        RemotePath = remotePath,
        Quality = quality,
      };

      // 下载与解码：JPEG 优先整图；RAW/HEIF 只能解缩略图（?act=thm 恒为 JPEG）
      byte[] fullBytes = null;
      if (downloadPhotoFile)
      {
        fullBytes = await _client.DownloadFileAsync(result.RemotePath);
        if (flipSavedPhotoX && quality == ZCAMPhotoQuality.JPEG)
        {
          fullBytes = FlipPhotoX(fullBytes); // 解码 → 左右镜像 → 重编码 JPG(q95) 后落盘
        }
        result.LocalPath = SaveToLocal(result.FileName, fullBytes);
      }

      if (quality == ZCAMPhotoQuality.JPEG && fullBytes != null)
      {
        result.Texture = DecodeTexture(fullBytes);
      }
      if (result.Texture == null)
      {
        var thm = await _client.DownloadFileAsync(result.RemotePath + "?act=thm");
        result.Texture = DecodeTexture(thm);
      }

      return result;
    }

    /// <summary>等待 movfmt 切换生效（轮询参数值）</summary>
    private async Task<bool> WaitMovfmtAsync(string target, float timeoutSec)
    {
      var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(timeoutSec);
      while (DateTime.UtcNow < deadline)
      {
        await Task.Delay(200);
        if (await _client.GetParamValueAsync(ZCAMParamKey.RecordFormat) == target)
        { return true; }
      }
      return await _client.GetParamValueAsync(ZCAMParamKey.RecordFormat) == target;
    }

    #endregion
    // ==================================================
    #region 取景

    /// <summary>
    /// 开始取景
    /// 首帧解码完成才算成功（超时报错走 OnError）。画面经 LiveViewTexture（RT）提供
    /// </summary>
    public Task StartLiveView() { return RunOperationAsync(nameof(StartLiveView), StartLiveViewCoreAsync); }
    private async Task StartLiveViewCoreAsync()
    {
      EnsureConnected();
      if (IsLiveViewing) { return; }

      if (liveViewChannel == ZCAMLiveViewChannel.SSP)
      { _viewSource = new ZCAMSspViewSource(_activeBaseUrl); }
      else
      { _viewSource = new ZCAMMjpegViewSource(_activeBaseUrl); }
      _viewSource.Start();

      var firstFrame = _viewSource.FirstFrameTask;
      var timeout = Task.Delay(TimeSpan.FromSeconds(liveViewFirstFrameTimeout));
      var done = await Task.WhenAny(firstFrame, timeout);

      if (done != firstFrame || firstFrame.Status != TaskStatus.RanToCompletion)
      {
        _viewSource.Stop();
        _viewSource = null;
        if (firstFrame.Exception != null)
        {
          throw new ZCAMException($"Live view failed to start [{liveViewChannel}]: {firstFrame.Exception.InnerException?.Message}");
        }
        throw new ZCAMException($"No first frame within {liveViewFirstFrameTimeout}s on [{liveViewChannel}] (channel unavailable or network down)");
      }

      IsLiveViewing = true;
    }

    /// <summary>
    /// 停止取景
    /// LiveViewTexture 随即销毁（UI 请监听 OnCameraClosed / OnError 处理）
    /// </summary>
    public Task StopLiveView() { return RunOperationAsync(nameof(StopLiveView), StopLiveViewCoreAsync); }
    private Task StopLiveViewCoreAsync()
    {
      if (!IsLiveViewing && _viewSource == null) { return Task.CompletedTask; }

      _viewSource?.Stop();
      _viewSource = null;
      IsLiveViewing = false;
      DestroyDecodeTexture();
      return Task.CompletedTask;
    }

    #endregion
    // ==================================================
    #region 录像

    public bool IsRecording { get; private set; } = false; // 录制状态位（成功下发后才翻转，跟随相机事实）

    /// <summary>
    /// 开始录像（取景与录制并行：录制走相机 Stream 0，不冲突）。
    /// 可裸调用（失败走 OnError / [ZCAMM] 日志）。
    /// </summary>
    public Task EZStartRecord() { return RunOperationAsync(nameof(EZStartRecord), EZStartRecordCoreAsync); }
    private async Task EZStartRecordCoreAsync()
    {
      EnsureConnected();
      if (IsRecording) { return; }
      await _client.StartRecordAsync();
      IsRecording = true;
      Debug.Log("[ZCAMM] Recording started. File goes to camera SD card /DCIM/A001/.");
    }

    /// <summary>
    /// 停止录像。重复调用无害。
    /// </summary>
    public Task EZStopRecord() { return RunOperationAsync(nameof(EZStopRecord), EZStopRecordCoreAsync); }
    private async Task EZStopRecordCoreAsync()
    {
      EnsureConnected();
      if (!IsRecording) { return; }
      await _client.StopRecordAsync();
      IsRecording = false;
      Debug.Log("[ZCAMM] Recording stopped.");
    }

    #endregion
    // ==================================================
    #region 格式化储存卡

    /// <summary>
    /// ⚠️ 格式化存储卡：不可逆清空卡内全部照片与视频（含未下载的原件）。
    /// 录像中调用会先自动停止录制。调用前请确认卡内数据已备份或确属废弃物。
    /// </summary>
    public Task FormatCard()
    {
      return RunOperationAsync(nameof(FormatCard), async () =>
      {
        EnsureConnected();
        if (IsRecording)
        {
          await _client.StopRecordAsync();
          IsRecording = false;
        }
        await _client.FormatCardAsync();
      });
    }

    #endregion
    // ==================================================
    #region 直通命令（参数）

    // 单条 HTTP 命令，由 ZCAMHttpClient 内部队列保证串行；未过操作门，建议调用方 await

    /// <summary>
    /// 写相机参数，如 SetParam(ZCAMParamKey.ISO, "800")
    /// </summary>
    public Task SetParam(string key, string value)
    {
      EnsureConnected();
      return _client.SetParamAsync(key, value);
    }

    /// <summary>
    /// 读参数原始 JSON（动态键名，P0 暴露原文）
    /// </summary>
    public Task<string> GetParamRaw(string key)
    {
      EnsureConnected();
      return _client.GetParamRawAsync(key);
    }

    /// <summary>
    /// 查询相机状态：工作模式 / 电量 / 待机与自动关机配置。
    /// 兼作保活心跳——任何 /ctrl/* 查询都会重置相机的待机计时器（auto_standby 防待机的免费手段）。
    /// </summary>
    public async Task<ZCAMCameraStatus> GetCameraStatus()
    {
      EnsureConnected();
      // Debug.Log(@$"[ZCAMM] Keep alive at {DateTime.Now}");
      return new ZCAMCameraStatus
      {
        Mode = await _client.QueryModeAsync(),
        Battery = await _client.GetParamValueAsync("battery"),
        AutoStandby = await _client.GetParamValueAsync("auto_standby"),
        AutoOff = await _client.GetParamValueAsync("auto_off"),
      };
    }

    /// <summary>
    /// 设置自动关机。合法值：Off / 30s / "1 min" / "2 min" / "4 min" / "8 min" / "15 min"（注意含空格）。
    /// 关机后只能物理开机，远程无解——常驻设备建议保持 Off。
    /// </summary>
    public Task SetAutoOff(string value)
    {
      return SetParam("auto_off", value);
    }

    #endregion
    // ==================================================
    #region 帧驱动

    /// <summary>
    /// 帧泵：每帧从取景源消费最新一帧，解码后 Blit 进 RT 上屏；顺带检测取景流意外中断。
    /// 纹理操作必须在主线程，故以 Update 轮询"最新帧槽位"实现（源内部丢帧优先）。
    /// </summary>
    private void PumpLiveView()
    {
      if (_viewSource == null || liveViewRT == null)
      {
        return;
      }

      // 流意外中断检测
      if (IsLiveViewing && _viewSource.IsCompleted)
      {
        var err = _viewSource.Error;
        _viewSource.Stop();
        _viewSource = null;
        IsLiveViewing = false;
        if (err != null)
        {
          var ze = new ZCAMException(err);
          OnError?.Invoke(new ZCAMError { Context = "LiveView", Exception = ze });
          Debug.LogWarning($"[ZCAMM] {err}");
        }
        return;
      }

      // 主线程消费最新帧（积压帧已在源内被丢弃）
      var frame = _viewSource.TryConsumeLatestFrame();
      if (frame == null)
      {
        return;
      }

      if (_viewSource.IsRawRgba)
      {
        // SSP 源：RGBA 裸数据直灌解码纹理（无编解码，延迟最低）
        int w = _viewSource.FrameWidth, h = _viewSource.FrameHeight;
        if (w <= 0 || h <= 0)
        {
          return;
        }
        if (_decodeTexture == null || _decodeTexture.width != w || _decodeTexture.height != h)
        {
          DestroyDecodeTexture();
          _decodeTexture = new Texture2D(w, h, TextureFormat.RGBA32, false) { name = "ZCAMDecode" };
        }
        _decodeTexture.LoadRawTextureData(frame);
        _decodeTexture.Apply(false, false);
      }
      else
      {
        // MJPEG 源：JPEG 解码（LoadImage 自动适配分辨率）
        if (_decodeTexture == null)
        {
          _decodeTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { name = "ZCAMDecode" };
        }
        if (!_decodeTexture.LoadImage(frame))
        {
          return;
        }
      }

      // 解码结果 Blit 进 RT：RawImage 只需在连接时绑定一次 RT，之后画面自动更新
      Graphics.Blit(_decodeTexture, liveViewRT);
    }

    #endregion
    // ==================================================
    #region 配置文件

    /// <summary>
    /// 从 JSON 配置文件读取相机参数并逐项下发（LitJson 解析）。
    /// 固定读取 StreamingAssets/ToneTuneToolkit/Configs/zcamconfig.json。
    /// 文件结构：{ "zcam_ip": "...", "zcam_setting": { "<paramKey>": <value>, ... } }
    /// 只处理 zcam_setting 下的键；单键失败不中断整体（收集后一次性抛出）。
    /// </summary>
    public Task<int> ApplySettingsFromJson() { return RunOperationAsync(nameof(ApplySettingsFromJson), ApplySettingsFromJsonCoreAsync); }
    private async Task<int> ApplySettingsFromJsonCoreAsync()
    {
      EnsureConnected();
      if (!File.Exists(zcamconfigPath))
      {
        throw new ZCAMException($"Config file not found: {zcamconfigPath}");
      }

      // LitJson：框架统一的 JSON 读取入口（ToneTuneToolkit.Data.LitJsonEditor）
      var settingNode = LitJsonEditor.Get(zcamconfigPath, "zcam_setting");
      if (settingNode == null || !settingNode.IsObject)
      {
        throw new ZCAMException($"Config file missing 'zcam_setting' node: {zcamconfigPath}");
      }

      var applied = new List<string>();
      var failed = new List<string>();
      // JsonData 经 IJsonWrapper 接口暴露 Keys（键名集合）——比 IDictionary 枚举强转更稳
      foreach (string key in settingNode.Keys)
      {
        var value = JsonValueToString(settingNode[key]);
        if (string.IsNullOrEmpty(value))
        {
          continue;
        }
        try
        {
          await SetParam(key, value);
          applied.Add($"{key}={value}");
        }
        catch (Exception e)
        {
          failed.Add($"{key}={value} ({e.Message})");
        }
      }

      if (failed.Count > 0)
      {
        throw new ZCAMException($"Failed to apply {failed.Count} param(s): {string.Join("; ", failed)}. Succeeded: {applied.Count}");
      }
      return applied.Count;
    }

    /// <summary>把 LitJson 值节点转字符串（字符串/数值/布尔通吃）</summary>
    private static string JsonValueToString(LitJson.JsonData v)
    {
      if (v.IsString)
      {
        return (string)v;
      }
      if (v.IsDouble)
      {
        return ((double)v).ToString(System.Globalization.CultureInfo.InvariantCulture);
      }
      if (v.IsInt || v.IsLong)
      {
        return v.ToString();
      }
      if (v.IsBoolean)
      {
        return ((bool)v) ? "1" : "0";
      }
      return v.ToJson();
    }

    #endregion
    // ==================================================
    #region 释放

    private void UninitDestroy()
    {
      _viewSource?.Stop();
      _client?.Dispose();
      _client = null;
    }

    private void UninitApplicationQuit()
    {
      // 退出时尽力释放会话；请求可能来不及完成，相机侧会话会自行超时回收
      _viewSource?.Stop();
      if (_client != null)
      {
        try { var _ = _client.QuitSessionAsync(); } catch { }
        _client.Dispose();
        _client = null;
      }
    }

    #endregion
    // ==================================================
    #region 工具

    private void EnsureConnected()
    {
      if (State != ZCAMConnectionState.Connected || _client == null)
      {
        throw new ZCAMException("Camera is not connected. Call OpenCamera first.");
      }
    }

    /// <summary>
    /// 取 DCIM 下字典序最大的文件夹与其内字典序最大的文件（DCF 命名递增，字典序即时间序）
    /// </summary>
    private async Task<(string folder, string file)> GetNewestEntryAsync()
    {
      var folders = await _client.ListDcimFoldersAsync();
      var folder = MaxOrdinal(folders);
      if (folder == null)
      {
        return (null, null);
      }
      var files = await _client.ListFilesAsync(folder);
      return (folder, MaxOrdinal(files));
    }

    private static string MaxOrdinal(string[] items)
    {
      if (items == null || items.Length == 0)
      {
        return null;
      }
      var max = items[0];
      for (int i = 1; i < items.Length; i++)
      {
        if (string.CompareOrdinal(items[i], max) > 0)
        {
          max = items[i];
        }
      }
      return max;
    }

    private static Texture2D DecodeTexture(byte[] jpegBytes)
    {
      if (jpegBytes == null || jpegBytes.Length == 0) { return null; }
      var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
      return tex.LoadImage(jpegBytes) ? tex : null;
    }

    /// <summary>销毁解码纹理并置空（Stop / Cleanup / 纹理重建 三处共用）。RT 为项目资产，不在此销毁</summary>
    private void DestroyDecodeTexture()
    {
      if (_decodeTexture == null) { return; }
      Destroy(_decodeTexture);
      _decodeTexture = null;
    }

    /// <summary>
    /// 照片水平镜像（X 轴翻转）：解码 JPEG → 像素左右倒序 → 重编码 JPG(q95)。
    /// 解码失败时原字节返回（不因翻转丢照片）。
    /// </summary>
    private static byte[] FlipPhotoX(byte[] jpegBytes)
    {
      var src = DecodeTexture(jpegBytes);
      if (src == null)
      {
        Debug.LogWarning("[ZCAMM] Photo flip skipped: decode failed, saving original");
        return jpegBytes;
      }

      int w = src.width, h = src.height;
      var flipped = new Texture2D(w, h, TextureFormat.RGBA32, false);
      var pixels = src.GetPixels32();
      var mirrored = new Color32[pixels.Length];
      for (int y = 0; y < h; y++)
      {
        for (int x = 0; x < w; x++)
        {
          mirrored[y * w + x] = pixels[y * w + (w - 1 - x)];
        }
      }
      flipped.SetPixels32(mirrored);
      flipped.Apply();

      var encoded = flipped.EncodeToJPG(95);
      Destroy(src);
      Destroy(flipped);
      return encoded;
    }

    private static string SaveToLocal(string fileName, byte[] bytes)
    {
      try
      {
        var dir = Path.Combine(Application.streamingAssetsPath, "ZCAM");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, fileName);
        File.WriteAllBytes(path, bytes);
        return path;
      }
      catch (Exception e)
      {
        Debug.LogWarning($"[ZCAMM] Failed to save photo locally: {e.Message}");
        return null;
      }
    }

    #endregion
  }
}