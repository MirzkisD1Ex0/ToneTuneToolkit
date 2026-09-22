/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.1
/// </summary>

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using ToneTuneToolkit.Common;
using ToneTuneToolkit.IO.Canon.EDSDK;

namespace ToneTuneToolkit.IO.Canon
{
  public class CanonSession : SingletonMaster<CanonSession>
  {
    // ---------------- 对外事件（均在主线程回调） ----------------
    public event Action OnCameraAdded;
    public event Action OnCameraNotConnected;
    public event Action<string> OnCameraConnected;
    public event Action OnCameraShutdown;
    public event Action OnLiveViewStarted;
    public event Action OnLiveViewStopped;
    public event Action<string> OnPhotoSaved;       // 拍照完成，参数为保存路径
    public event Action<string> OnPhotoFailed;      // 拍照失败（快门报错/释放失败/下载失败/被并发取消），参数为原因；与 OnPhotoSaved 相对的失败信号
    public event Action<byte[]> OnLiveViewFrame;    // LiveView JPEG 原始字节，参数为 JPEG
    public event Action<uint> OnCaptureError;       // 相机远程释放失败，参数为错误码（0x01~0x08，语义见文档 §4.2.17）
    public event Action<bool> OnJobStatusChanged;   // 是否有照片等待传回主机（文档 §4.2.11；type1 协议机型可能不发）
    public event Action<string> OnError;

    // ---------------- 对外状态 ----------------
    public bool IsCameraConnected { get; private set; }
    public bool IsLiveViewActive { get; private set; }
    public string DeviceDescription { get; private set; }
    public string StreamingAssetsPath { get; private set; }

    /// <summary>仅在工作线程内使用；主线程勿直接调用 EDSDK。读取句柄用（只读）。</summary>
    public IntPtr Camera { get { return _camera; } }

    // ---------------- 保活配置 ----------------
    /// <summary>会话打开期间定期发送 ExtendShutDownTimer，阻止相机自动关机/休眠（默认开启）。</summary>
    public bool KeepCameraAwake = true;
    /// <summary>保活间隔（秒）。必须小于相机“自动电源关闭”菜单的设定档位（常见最短 30 秒，故默认 25 秒留出余量）。</summary>
    public float KeepAwakeIntervalSeconds = 25f;

    // ---------------- 内部 ----------------
    private readonly BlockingCollection<Action> _jobs = new BlockingCollection<Action>();
    private readonly ConcurrentQueue<Action> _main = new ConcurrentQueue<Action>();
    private Thread _workerThread;
    private bool _workerStarted;
    private volatile bool _running;

    private IntPtr _camera;
    private bool _sdkReady;
    private bool _sessionOpen;
    private volatile bool _liveViewActive;
    private bool _eventPumpLoggedError;
    private bool _pumpStartedLogged;

    // 拍照传输状态（参考实现的 pending item 机制）
    private IntPtr _pendingItem;
    private volatile bool _downloading;

    // 保活：下次发送 ExtendShutDownTimer 的时间（UTC）；MaxValue 表示未激活
    private DateTime _nextKeepAlive = DateTime.MaxValue;

    // EDSDK 回调委托（保活，避免被 GC 回收）
    private EdsPropertyEventHandler _propertyHandler;
    private EdsObjectEventHandler _objectHandler;
    private EdsStateEventHandler _stateHandler;
    private EdsCameraAddedHandler _cameraAddedHandler;
    private EdsProgressCallback _progressHandler;

    // ================= 生命周期 =================

    protected override void Awake()
    {
      base.Awake();
      StreamingAssetsPath = Application.streamingAssetsPath;

      _propertyHandler = HandlePropertyEvent;
      _objectHandler = HandleObjectEvent;
      _stateHandler = HandleStateEvent;
      _cameraAddedHandler = HandleCameraAdded;
      _progressHandler = HandleProgress;

      StartWorker();
    }

    private void Update()
    {
      while (_main.TryDequeue(out Action a)) { a(); }
    }

    private void OnDestroy()
    {
      if (_workerStarted)
      {
        _jobs.Add(DoClose);
        _running = false;
        if (_workerThread != null && _workerThread.IsAlive) { _workerThread.Join(4000); }
      }
    }

    private void StartWorker()
    {
      // EDSDK 文档要求：相机访问线程用 COINIT_APARTMENTTHREADED(STA)。
      // 循环：① 泵送 EdsGetEvent() 派发对象/属性/状态回调 → ② 非阻塞处理队列任务。
      // 关键：不调用 EdsGetEvent() 则 EDSDK 事件回调不被派发，拍照回传(DirItemRequestTransfer)收不到，
      //       传输状态机也会卡住导致 EdsDownload 报 EDS_ERR_INTERNAL_ERROR。
      _workerThread = new Thread(() =>
      {
        EDSDKAPI.EdsComInit();
        try
        {
          while (_running)
          {
            // 1) 泵送事件：仅在 SDK 已初始化后调用。
            //    用 blocking TryTake 控制节流：空闲时约 50ms 一轮(~20Hz)，
            //    远低于原先 Sleep(5) 的 200Hz，减少对 EDSDK/USB 的持续占用（切窗卡顿）。
            if (_sdkReady)
            {
              if (!_pumpStartedLogged)
              {
                _pumpStartedLogged = true;
                Debug.Log("[Canon Session] EdsGetEvent is working.");
              }
              uint ev = EDSDKAPI.EdsGetEvent();
              if (ev != EDSDKConsts.EDS_ERR_OK && !_eventPumpLoggedError)
              {
                _eventPumpLoggedError = true;
                _main.Enqueue(() => OnError?.Invoke("EdsGetEvent: " + ev));
              }
            }

            // 2) 保活：会话期间定期延长自动关机计时，防止相机休眠（Keep Device On）。
            //    详见文档 EdsSendCommand -> kEdsCameraCommand_ExtendShutDownTimer。
            if (KeepCameraAwake && _sessionOpen && _camera != IntPtr.Zero)
            {
              DateTime now = DateTime.UtcNow;
              if (now >= _nextKeepAlive)
              {
                _nextKeepAlive = now.AddSeconds(Math.Max(5f, KeepAwakeIntervalSeconds));
                uint keepErr = EDSDKAPI.EdsSendCommand(_camera, EDSDKConsts.CameraCommand_ExtendShutDownTimer, 0);
                // 拍照/下载瞬间的 DEVICE_BUSY 属正常，下个周期自然重试，不升级为 OnError

                if (keepErr != EDSDKConsts.EDS_ERR_OK)
                {
                  Debug.Log(@$"[Canon Session] Keep awake failed, ExtendShutDownTimer err=0x{keepErr:X8} at {DateTime.Now}.");
                }
              }
            }

            // 3) 处理任务：空则阻塞至多 50ms（即上面 20Hz 的由来）。
            //    【关键】每轮只执行一个任务，随即回到循环顶部泵送事件（与官方 Sample 的 CommandProcessor 相同）。
            //    绝不可在此“批处理排空”队列：LiveViewTick 会自续入队，内层排空循环会被它喂成死循环，
            //    EdsGetEvent/保活将被饿死——拍照回传事件(DirItemRequestTransfer)不被派发，
            //    照片要等到手动停止 LiveView 才会下载回来（2026-09 排查过的坑，详见 Misc/CLAUDE.md）。
            if (_jobs.TryTake(out Action job, 50))
            {
              try { job(); }
              catch (Exception e)
              {
                _main.Enqueue(() => OnError?.Invoke("CanonWorker: " + e.Message));
              }
            }
          }

          // 停止时排空剩余任务（如 DoClose）
          while (_jobs.TryTake(out Action job, 0))
          {
            try { job(); }
            catch (Exception e) { _main.Enqueue(() => OnError?.Invoke("CanonWorker: " + e.Message)); }
          }
        }
        finally { EDSDKAPI.EdsComUninit(); }
      });
      _workerThread.IsBackground = true;
      // 先置位再启动线程，避免工作线程启动瞬间读到 _running=false 直接退出循环
      _workerStarted = true;
      _running = true;
      _workerThread.Start();
    }

    // ================= 公开操作（异步，投递到工作线程） =================

    public void Open()
    {
      if (_camera != IntPtr.Zero || _sessionOpen) { return; }
      _jobs.Add(DoOpenSession);
      Debug.Log(@$"[Canon Session] Camera opened at {DateTime.Now}.");
    }

    public void OpenAsync()
    {
      if (_camera != IntPtr.Zero || _sessionOpen) { return; }
      _jobs.Add(DoOpenSession);
      Debug.Log(@$"[Canon Session] Camera opened async at {DateTime.Now}.");
    }

    public void Close()
    {
      _jobs.Add(DoClose);
      Debug.Log(@$"[Canon Session] Camera closed at {DateTime.Now}.");
    }

    public void StartLiveView()
    {
      _jobs.Add(DoStartLiveView);
      Debug.Log(@$"[Canon Session] LiveView opened at {DateTime.Now}.");
    }

    public void StopLiveView()
    {
      _jobs.Add(DoStopLiveView);
      Debug.Log(@$"[Canon Session] LiveView closed at {DateTime.Now}.");
    }

    public void TakePhoto()
    {
      _jobs.Add(DoTakePhoto);
      Debug.Log(@$"[Canon Session] Taking photo at {DateTime.Now}.");
    }

    /// <summary>
    /// 发送相机状态指令（UILock/UIUnLock/直传模式进出，取值见 EDSDKConsts.CameraState_*）。
    /// 注意：UILock/UIUnLock 须成对调用；锁定时 inParam=1 可同时熄灭相机 TFT。
    /// </summary>
    public void SendStatusCommand(uint statusCommand, int param) => _jobs.Add(() => DoSendStatusCommand(statusCommand, param));

    // ================= 工作线程实现 =================

    private void DoOpenSession()
    {
      uint err;

      if (!EnsureSdkInitialized()) { return; }

      EDSDKAPI.EdsSetCameraAddedHandler(_cameraAddedHandler, IntPtr.Zero);

      IntPtr cameraList = IntPtr.Zero;
      err = EDSDKAPI.EdsGetCameraList(out cameraList);
      if (err != EDSDKConsts.EDS_ERR_OK) { PostError("EdsGetCameraList: " + err); EDSDKAPI.EdsRelease(cameraList); return; }

      int count = 0;
      err = EDSDKAPI.EdsGetChildCount(cameraList, out count);
      if (err != EDSDKConsts.EDS_ERR_OK) { PostError("EdsGetChildCount: " + err); EDSDKAPI.EdsRelease(cameraList); return; }

      if (count <= 0)
      {
        EDSDKAPI.EdsRelease(cameraList);
        _main.Enqueue(() => OnCameraNotConnected?.Invoke());
        return;
      }

      IntPtr camera = IntPtr.Zero;
      err = EDSDKAPI.EdsGetChildAtIndex(cameraList, 0, out camera);
      EDSDKAPI.EdsRelease(cameraList);
      if (err != EDSDKConsts.EDS_ERR_OK) { PostError("EdsGetChildAtIndex: " + err); return; }

      EdsDeviceInfo deviceInfo = new EdsDeviceInfo();
      err = EDSDKAPI.EdsGetDeviceInfo(camera, out deviceInfo);
      string deviceDesc = deviceInfo.szDeviceDescription ?? "Canon Camera";

      err = EDSDKAPI.EdsSetObjectEventHandler(camera, EDSDKConsts.ObjectEvent_All, _objectHandler, IntPtr.Zero);
      // Debug.Log(@$"[Canon Session] SetObjectEventHandler err=0x{err:X8} at {DateTime.Now}.");
      err = EDSDKAPI.EdsSetPropertyEventHandler(camera, EDSDKConsts.PropertyEvent_All, _propertyHandler, IntPtr.Zero);
      err = EDSDKAPI.EdsSetCameraStateEventHandler(camera, EDSDKConsts.StateEvent_All, _stateHandler, IntPtr.Zero);

      _camera = camera;

      err = EDSDKAPI.EdsOpenSession(camera);
      if (err != EDSDKConsts.EDS_ERR_OK) { PostError("EdsOpenSession: " + err); EDSDKAPI.EdsRelease(camera); _camera = IntPtr.Zero; return; }
      _sessionOpen = true;

      // 让拍摄的图像回传到主机，而非存卡内，便于直接下载。
      EdsCapacity capacity = new EdsCapacity { NumberOfFreeClusters = 0x7FFFFFFF, BytesPerSector = 0x1000, Reset = 1 };
      uint saveToErr = EDSDKAPI.EdsSetPropertyData(camera, EDSDKConsts.PropID_SaveTo, 0, (uint)EdsSaveTo.Host);
      uint capErr = EDSDKAPI.EdsSetCapacity(camera, capacity);
      uint saveToRead = 0;
      EDSDKAPI.EdsGetPropertyData(camera, EDSDKConsts.PropID_SaveTo, 0, out saveToRead);
      // Debug.Log(@$"[Canon Session] SaveTo=Host set err=0x{saveToErr:X8}, SetCapacity err=0x{capErr:X8}, SaveTo readback={saveToRead} ({(EdsSaveTo)saveToRead}) at {DateTime.Now}.");

      // 会话就绪后 2 秒启动保活，避开会话建立初期可能的 BUSY
      _nextKeepAlive = DateTime.UtcNow.AddSeconds(2);

      _main.Enqueue(() =>
      {
        DeviceDescription = deviceDesc;
        IsCameraConnected = true;
        OnCameraConnected?.Invoke(deviceDesc);
      });
    }

    private void DoClose()
    {
      _liveViewActive = false;
      _downloading = false;

      // 主动关闭时若仍有照片待传：按文档 §4.2.12 必须 Cancel 释放相机侧传输状态，否则遗留僵尸请求
      if (_pendingItem != IntPtr.Zero)
      {
        EDSDKAPI.EdsDownloadCancel(_pendingItem);
        EDSDKAPI.EdsRelease(_pendingItem);
        _pendingItem = IntPtr.Zero;
      }

      if (_sessionOpen && _camera != IntPtr.Zero)
      {
        EDSDKAPI.EdsCloseSession(_camera);
        _sessionOpen = false;
      }
      if (_camera != IntPtr.Zero)
      {
        EDSDKAPI.EdsRelease(_camera);
        _camera = IntPtr.Zero;
      }
      if (_sdkReady)
      {
        EDSDKAPI.EdsTerminateSDK();
        _sdkReady = false;
      }

      _main.Enqueue(() => { IsCameraConnected = false; IsLiveViewActive = false; });
    }

    private void DoTakePhoto()
    {
      if (_camera == IntPtr.Zero) { PostError("TakePhoto: camera not connected"); return; }
      uint beforeShot = 0;
      EDSDKAPI.EdsGetPropertyData(_camera, EDSDKConsts.PropID_AvailableShots, 0, out beforeShot);
      uint shutterErr = EDSDKAPI.EdsSendCommand(_camera, EDSDKConsts.CameraCommand_PressShutterButton,
        (int)EdsShutterButton.CameraCommand_ShutterButton_Completely);
      // Debug.Log(@$"[Canon Session] TakePhoto: shutter(completely) err=0x{shutterErr:X8}, availShots(前)={beforeShot} at {DateTime.Now}.");
      uint offErr = EDSDKAPI.EdsSendCommand(_camera, EDSDKConsts.CameraCommand_PressShutterButton,
        (int)EdsShutterButton.CameraCommand_ShutterButton_OFF);
      // Debug.Log(@$"[Canon Session] TakePhoto: shutter(off) err=0x{offErr:X8} at {DateTime.Now}.");
      uint afterShot = 0;
      EDSDKAPI.EdsGetPropertyData(_camera, EDSDKConsts.PropID_AvailableShots, 0, out afterShot);
      // Debug.Log(@$"[Canon Session] TakePhoto: availShots(后)={afterShot} (若后<前则相机确实拍了) at {DateTime.Now}.");
      if (shutterErr != EDSDKConsts.EDS_ERR_OK)
      {
        PostError("TakePhoto: " + shutterErr);
        _main.Enqueue(() => OnPhotoFailed?.Invoke("Shutter err=0x" + shutterErr.ToString("X8")));
        return;
      }
      // 快门命令返回 OK 但 AvailableShots 前后未降，可能是快门被静默忽略（不会有照片回传）。
      // 仅记日志、不发事件：SaveTo=Host 且无卡时该值是否递减未经实机确认，误报会打断正常流程。
      if (afterShot == beforeShot)
      {
        Debug.Log(@$"[Canon Session] TakePhoto: AvailableShots 前后均为 {beforeShot}，快门可能被静默忽略（该检测可靠性待实机确认） at {DateTime.Now}.");
      }
    }

    private void DoSendStatusCommand(uint statusCommand, int param)
    {
      if (_camera == IntPtr.Zero) { PostError("SendStatusCommand: camera not connected"); return; }
      uint err = EDSDKAPI.EdsSendStatusCommand(_camera, statusCommand, param);
      // Debug.Log(@$"[Canon Session] SendStatusCommand(0x{statusCommand:X}, {param}) err=0x{err:X8} at {DateTime.Now}.");
      if (err != EDSDKConsts.EDS_ERR_OK) { PostError("SendStatusCommand: " + err); }
    }

    /// <summary>
    /// 立即补发一次保活（Keep Device On）并重置周期计时。
    /// 供 kEdsStateEvent_WillSoonShutDown 兜底触发：周期心跳因 DEVICE_BUSY 等偶发失败、
    /// 或相机自动关机档位短于心跳间隔时，由相机的“即将关机”预警直接续命。
    /// </summary>
    private void DoKeepAliveBeat()
    {
      if (!_sessionOpen || _camera == IntPtr.Zero) { return; }
      uint err = EDSDKAPI.EdsSendCommand(_camera, EDSDKConsts.CameraCommand_ExtendShutDownTimer, 0);
      _nextKeepAlive = DateTime.UtcNow.AddSeconds(Math.Max(5f, KeepAwakeIntervalSeconds));
      Debug.Log(@$"[Canon Session] KeepAlive: 预警补发 ExtendShutDownTimer err=0x{err:X8} at {DateTime.Now}.");
    }

    private void DoStartLiveView()
    {
      if (_camera == IntPtr.Zero) { PostError("StartLiveView: camera not connected"); return; }
      uint err;

      uint evfMode = 0;
      err = EDSDKAPI.EdsGetPropertyData(_camera, EDSDKConsts.PropID_Evf_Mode, 0, out evfMode);
      if (err == EDSDKConsts.EDS_ERR_OK && evfMode == 0)
      {
        err = EDSDKAPI.EdsSetPropertyData(_camera, EDSDKConsts.PropID_Evf_Mode, 0, (uint)1);
      }

      if (err == EDSDKConsts.EDS_ERR_OK)
      {
        uint outputDevice = 0;
        err = EDSDKAPI.EdsGetPropertyData(_camera, EDSDKConsts.PropID_Evf_OutputDevice, 0, out outputDevice);
        if (err == EDSDKConsts.EDS_ERR_OK)
        {
          outputDevice |= EDSDKConsts.EvfOutputDevice_PC;
          err = EDSDKAPI.EdsSetPropertyData(_camera, EDSDKConsts.PropID_Evf_OutputDevice, 0, outputDevice);
        }
      }

      if (err != EDSDKConsts.EDS_ERR_OK) { PostError("StartLiveView: " + err); return; }

      _liveViewActive = true;
      _jobs.Add(LiveViewTick);
      _main.Enqueue(() => { IsLiveViewActive = true; OnLiveViewStarted?.Invoke(); });
    }

    private void DoStopLiveView()
    {
      _liveViewActive = false;
      if (_camera == IntPtr.Zero) { _main.Enqueue(() => { IsLiveViewActive = false; OnLiveViewStopped?.Invoke(); }); return; }

      uint outputDevice = 0;
      uint err = EDSDKAPI.EdsGetPropertyData(_camera, EDSDKConsts.PropID_Evf_OutputDevice, 0, out outputDevice);
      if (err == EDSDKConsts.EDS_ERR_OK)
      {
        outputDevice &= ~EDSDKConsts.EvfOutputDevice_PC;
        EDSDKAPI.EdsSetPropertyData(_camera, EDSDKConsts.PropID_Evf_OutputDevice, 0, outputDevice);
      }

      _main.Enqueue(() => { IsLiveViewActive = false; OnLiveViewStopped?.Invoke(); });
    }

    // ================= LiveView 取帧（自续提交，避免独占工作线程） =================

    private void LiveViewTick()
    {
      if (!_liveViewActive) { return; }

      DownloadEvfFrame();

      if (_liveViewActive)
      {
        Thread.Sleep(10);
        _jobs.Add(LiveViewTick);  // 自续到队尾；工作线程每轮仅执行一个任务并先泵送事件，故拍照等指令可在帧间执行
      }
    }

    private void DownloadEvfFrame()
    {
      if (_camera == IntPtr.Zero) { return; }
      uint err;
      IntPtr stream = IntPtr.Zero;
      IntPtr evfImage = IntPtr.Zero;

      err = EDSDKAPI.EdsCreateMemoryStream(2 * 1024 * 1024, out stream);
      if (err == EDSDKConsts.EDS_ERR_OK) { err = EDSDKAPI.EdsCreateEvfImageRef(stream, out evfImage); }
      if (err == EDSDKConsts.EDS_ERR_OK) { err = EDSDKAPI.EdsDownloadEvfImage(_camera, evfImage); }
      if (err == EDSDKConsts.EDS_ERR_OK)
      {
        byte[] jpeg = ReadStreamToBytes(stream);
        if (jpeg != null && jpeg.Length > 0) { _main.Enqueue(() => OnLiveViewFrame?.Invoke(jpeg)); }
      }
      // DEVICE_BUSY / OBJECT_NOTREADY 等：丢这帧即可，不阻塞循环

      if (evfImage != IntPtr.Zero) { EDSDKAPI.EdsRelease(evfImage); }
      if (stream != IntPtr.Zero) { EDSDKAPI.EdsRelease(stream); }
    }

    private byte[] ReadStreamToBytes(IntPtr stream)
    {
      UInt64 length = 0;
      uint err = EDSDKAPI.EdsGetLength(stream, out length);
      if (err != EDSDKConsts.EDS_ERR_OK || length == 0) { return null; }

      IntPtr ptr = IntPtr.Zero;
      err = EDSDKAPI.EdsGetPointer(stream, out ptr);
      if (err != EDSDKConsts.EDS_ERR_OK || ptr == IntPtr.Zero) { return null; }

      byte[] buffer = new byte[length];
      Marshal.Copy(ptr, buffer, 0, (int)length);
      return buffer;
    }

    // ================= 拍照下载（由对象事件 DirItemRequestTransfer 触发） =================

    private void DownloadPhoto()
    {
      IntPtr dirItem = _pendingItem;
      _pendingItem = IntPtr.Zero;
      if (dirItem == IntPtr.Zero) { _downloading = false; return; }

      uint err;
      EdsDirectoryItemInfo info = new EdsDirectoryItemInfo();
      err = EDSDKAPI.EdsGetDirectoryItemInfo(dirItem, out info);
      if (err != EDSDKConsts.EDS_ERR_OK)
      {
        Debug.Log(@$"[Canon Session] DownloadPhoto: GetDirectoryItemInfo err=0x{err:X8} at {DateTime.Now}.");
        PostError("GetDirectoryItemInfo: " + err);
        EDSDKAPI.EdsDownloadCancel(dirItem); // 僵尸请求自愈：遗留传输只能 Cancel 清除（否则下次会话下载必 0x2）
        EDSDKAPI.EdsRelease(dirItem);
        _downloading = false;
        _main.Enqueue(() => OnPhotoFailed?.Invoke("GetDirectoryItemInfo err=0x" + err.ToString("X8")));
        return;
      }
      // Debug.Log(@$"[Canon Session] DownloadPhoto: dirItem={dirItem}, size={info.Size}, file={info.szFileName} at {DateTime.Now}.");

      string fileName = info.szFileName;
      if (string.IsNullOrEmpty(fileName)) { fileName = "IMG_" + DateTime.Now.Ticks + ".JPG"; }

      // SaveTo=Host 时照片不落存储卡，相机文件编号不随卡推进：场景重启 → 会话/SDK 重建，
      // 编号每次都从 IMG_0001 重新开始，而下方 CreateAlways 会覆盖同名旧图。
      // 主机侧加“yyyyMMddHHmmss_”时间戳前缀保证跨重启唯一；时间在最前，按名排序即按拍摄时间排序。
      fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;

      string destPath = fileName;
      try
      {
        Directory.CreateDirectory(StreamingAssetsPath);
        destPath = Path.Combine(StreamingAssetsPath, fileName);
      }
      catch (Exception e)
      {
        Debug.Log(@$"[Canon Session] DownloadPhoto: PersistPath {e.Message} at {DateTime.Now}.");
        PostError("PersistPath: " + e.Message);
        EDSDKAPI.EdsDownloadCancel(dirItem);
        EDSDKAPI.EdsRelease(dirItem);
        _downloading = false;
        _main.Enqueue(() => OnPhotoFailed?.Invoke("PersistPath: " + e.Message));
        return;
      }

      IntPtr stream = IntPtr.Zero;
      err = EDSDKAPI.EdsCreateFileStream(destPath, EdsFileCreateDisposition.CreateAlways, EdsAccess.ReadWrite, out stream);
      if (err != EDSDKConsts.EDS_ERR_OK) { Debug.Log(@$"[Canon Session] DownloadPhoto: CreateFileStream err=0x{err:X8} at {DateTime.Now}."); PostError("CreateFileStream: " + err); }
      if (err == EDSDKConsts.EDS_ERR_OK)
      {
        err = EDSDKAPI.EdsSetProgressCallback(stream, _progressHandler, EdsProgressOption.Periodically, stream);
        if (err != EDSDKConsts.EDS_ERR_OK) { Debug.Log(@$"[Canon Session] DownloadPhoto: SetProgressCallback err=0x{err:X8} at {DateTime.Now}."); }
      }
      if (err == EDSDKConsts.EDS_ERR_OK) { err = EDSDKAPI.EdsDownload(dirItem, info.Size, stream); }
      if (err != EDSDKConsts.EDS_ERR_OK) { Debug.Log(@$"[Canon Session] DownloadPhoto: EdsDownload err=0x{err:X8} at {DateTime.Now}."); PostError("Download: " + err); }
      if (err == EDSDKConsts.EDS_ERR_OK) { err = EDSDKAPI.EdsDownloadComplete(dirItem); }
      if (err != EDSDKConsts.EDS_ERR_OK) { Debug.Log(@$"[Canon Session] DownloadPhoto: DownloadComplete err=0x{err:X8} at {DateTime.Now}."); PostError("DownloadComplete: " + err); }

      if (stream != IntPtr.Zero) { EDSDKAPI.EdsRelease(stream); }
      if (err != EDSDKConsts.EDS_ERR_OK)
      {
        // 传输未正常完成（含僵尸请求的 0x2）：按文档 §4.2.12 必须 Cancel 清除相机侧传输状态
        EDSDKAPI.EdsDownloadCancel(dirItem);
      }
      EDSDKAPI.EdsRelease(dirItem);

      if (err == EDSDKConsts.EDS_ERR_OK)
      {
        string path = destPath;
        Debug.Log(@$"[Canon Session] Photo downloaded in {path} at {DateTime.Now}.");
        _main.Enqueue(() => OnPhotoSaved?.Invoke(path));
      }
      else
      {
        Debug.Log(@$"[Canon Session] Photo download failed, code err=0x{err:X8} at {DateTime.Now}.");
        _main.Enqueue(() => OnPhotoFailed?.Invoke("Download err=0x" + err.ToString("X8")));
      }
      _downloading = false;
    }

    // ================= EDSDK 回调（在 SDK 线程，只投递，不碰 Unity API） =================

    private uint HandleObjectEvent(uint inEvent, IntPtr inRef, IntPtr inContext)
    {
      if (inEvent == EDSDKConsts.ObjectEvent_DirItemRequestTransfer ||
          inEvent == EDSDKConsts.ObjectEvent_DirItemRequestTransferDT)
      {
        if (_downloading)
        {
          // 已有传输进行中：取消并释放这次的，避免缓冲冲突。该照片被丢弃，发出失败信号避免上层空等
          EDSDKAPI.EdsDownloadCancel(inRef);
          EDSDKAPI.EdsRelease(inRef);
          Debug.Log(@$"[Canon Session] ObjectEvent transfer 但已有下载进行中，取消本次 (0x{inEvent:X}) at {DateTime.Now}.");
          _main.Enqueue(() => OnPhotoFailed?.Invoke("Superseded by concurrent transfer"));
          return EDSDKConsts.EDS_ERR_OK;
        }
        _downloading = true;
        _pendingItem = inRef;
        // Debug.Log(@$"[Canon Session] ObjectEvent transfer (0x{inEvent:X}) -> enqueue download (dirItem={inRef}) at {DateTime.Now}.");
        _jobs.Add(DownloadPhoto);
      }
      else
      {
        Debug.Log(@$"[Canon Session] ObjectEvent 0x{inEvent:X} (unhandled) at {DateTime.Now}.");
        if (inRef != IntPtr.Zero)
        {
          EDSDKAPI.EdsRelease(inRef);
        }
      }
      return EDSDKConsts.EDS_ERR_OK;
    }

    private uint HandlePropertyEvent(uint inEvent, uint inPropertyID, uint inParam, IntPtr inContext)
    {
      // 属性变化通知（轻量，可选扩展）
      return EDSDKConsts.EDS_ERR_OK;
    }

    private uint HandleStateEvent(uint inEvent, uint inParameter, IntPtr inContext)
    {
      if (inEvent == EDSDKConsts.StateEvent_Shutdown)
      {
        Debug.Log(@$"[Canon Session] StateEvent Shutdown -> 拆会话以支持重连 at {DateTime.Now}.");
        _jobs.Add(HandleCameraShutdown);
      }
      else if (inEvent == EDSDKConsts.StateEvent_WillSoonShutDown)
      {
        // 相机即将自动关机（inParameter=剩余秒数；仅当机身菜单开启了自动关机才会发）。
        // 按文档以 ExtendShutDownTimer 续期取消关机；经任务队列执行，不在 SDK 回调内嵌套 SDK 调用。
        Debug.Log(@$"[Canon Session] StateEvent WillSoonShutDown（剩余 {inParameter} 秒）-> 立即续期 at {DateTime.Now}.");
        _jobs.Add(DoKeepAliveBeat);
      }
      else if (inEvent == EDSDKConsts.StateEvent_JobStatusChanged)
      {
        // 是否有照片等待传回主机（文档 §4.2.11：应用关闭前应确保全部传完；type1 协议机型不发此事件）
        bool hasPending = inParameter != 0;
        Debug.Log(@$"[Canon Session] StateEvent JobStatusChanged: {(hasPending ? "有待传照片" : "无待传照片")} at {DateTime.Now}.");
        _main.Enqueue(() => OnJobStatusChanged?.Invoke(hasPending));
      }
      else if (inEvent == EDSDKConsts.StateEvent_CaptureError)
      {
        // 远程释放失败（文档 §4.2.17）。错误码：0x01 拍摄失败 / 0x02 镜头收回 / 0x03 机构类错误 /
        // 0x04 传感器清洁 / 0x05 静音操作 / 0x06 无卡 / 0x07 卡错误 / 0x08 写保护。
        // 释放失败意味着本次不会有照片回传，故同时并入 OnPhotoFailed。
        uint p = inParameter;
        Debug.Log(@$"[Canon Session] StateEvent CaptureError param=0x{p:X} at {DateTime.Now}.");
        _main.Enqueue(() =>
        {
          OnCaptureError?.Invoke(p);
          OnPhotoFailed?.Invoke("CaptureError 0x" + p.ToString("X"));
          OnError?.Invoke($"CaptureError 0x{p:X}");
        });
      }
      else if (inEvent == EDSDKConsts.StateEvent_InternalError)
      {
        // SDK 内部错误（文档 §4.2.18）：设备大概率无法继续正常工作，应取消远程连接。
        // 复用 Shutdown 的拆会话路径（不终止 SDK，保留重连能力）；随后若真实 Shutdown 接踵而至，门闸防重复。
        uint p = inParameter;
        Debug.Log(@$"[Canon Session] StateEvent InternalError param=0x{p:X} -> 拆会话（文档要求取消远程连接） at {DateTime.Now}.");
        _main.Enqueue(() => OnError?.Invoke($"InternalError 0x{p:X}"));
        _jobs.Add(HandleCameraShutdown);
      }
      else
      {
        Debug.Log(@$"[Canon Session] StateEvent 0x{inEvent:X} (unhandled) at {DateTime.Now}.");
      }
      return EDSDKConsts.EDS_ERR_OK;
    }

    /// <summary>相机关机/断开（含 InternalError 主动拆连接）：清理会话与相机引用，但不终止 SDK，便于下次 Open() 直接重连。</summary>
    private void HandleCameraShutdown()
    {
      // 门闸：InternalError 拆除后真实 Shutdown 常会接踵而至，避免重复清理与重复 OnCameraShutdown
      if (!_sessionOpen && _camera == IntPtr.Zero && _pendingItem == IntPtr.Zero) { return; }

      _liveViewActive = false;
      _downloading = false;
      if (_pendingItem != IntPtr.Zero)
      {
        // 文档 §4.2.12：不打算下载的对象必须 EdsDownloadCancel 释放相机侧资源，否则遗留僵尸请求
        EDSDKAPI.EdsDownloadCancel(_pendingItem);
        EDSDKAPI.EdsRelease(_pendingItem);
        _pendingItem = IntPtr.Zero;
      }

      if (_sessionOpen && _camera != IntPtr.Zero) { EDSDKAPI.EdsCloseSession(_camera); }
      _sessionOpen = false;
      if (_camera != IntPtr.Zero) { EDSDKAPI.EdsRelease(_camera); _camera = IntPtr.Zero; }

      _main.Enqueue(() => { IsCameraConnected = false; IsLiveViewActive = false; OnCameraShutdown?.Invoke(); });
    }

    private uint HandleCameraAdded(IntPtr inContext)
    {
      _main.Enqueue(() => OnCameraAdded?.Invoke());
      return EDSDKConsts.EDS_ERR_OK;
    }

    /// <summary>下载进度回调（在工作线程/下载线程触发，勿碰 Unity API）</summary>
    private uint HandleProgress(uint inPercent, IntPtr inContext, ref bool outCancel)
    {
      return 0;
    }

    // ================= 工具 =================



    /// <summary>
    /// 防御式初始化：EDSDK.dll 在编辑器进程内不随 Play 进出卸载，若上次会话未干净 terminate，
    /// 再次 EdsInitializeSDK 会返回 EDS_ERR_INTERNAL_ERROR(0x2)。此方法在遇到该情况时先 terminate 再 init 恢复。
    /// </summary>
    private bool EnsureSdkInitialized()
    {
      if (_sdkReady) { return true; }

      uint err = EDSDKAPI.EdsInitializeSDK();
      if (err == EDSDKConsts.EDS_ERR_OK) { _sdkReady = true; return true; }

      if (err == EDSDKConsts.EDS_ERR_INTERNAL_ERROR)
      {
        Debug.Log(@$"[Canon Session] EdsInitializeSDK 返回 0x2，疑似已初始化，尝试 terminate+reinit 恢复 at {DateTime.Now}.");
        EDSDKAPI.EdsTerminateSDK();
        err = EDSDKAPI.EdsInitializeSDK();
        if (err == EDSDKConsts.EDS_ERR_OK) { _sdkReady = true; return true; }
      }

      PostError("EdsInitializeSDK: " + err);
      return false;
    }

    private void PostError(string msg) => _main.Enqueue(() => OnError?.Invoke(msg));
  }
}