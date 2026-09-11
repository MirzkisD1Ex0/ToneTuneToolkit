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
  public class Session : SingletonMaster<Session>
  {
    // ---------------- 对外事件（均在主线程回调） ----------------
    public event Action OnCameraAdded;
    public event Action OnCameraNotConnected;
    public event Action<string> OnCameraConnected;
    public event Action OnCameraShutdown;
    public event Action OnLiveViewStarted;
    public event Action OnLiveViewStopped;
    public event Action<string> OnPhotoSaved;       // 拍照完成，参数为保存路径
    public event Action<byte[]> OnLiveViewFrame;    // LiveView JPEG 原始字节，参数为 JPEG
    public event Action<string> OnError;

    // ---------------- 对外状态 ----------------
    public bool IsCameraConnected { get; private set; }
    public bool IsLiveViewActive { get; private set; }
    public string DeviceDescription { get; private set; }
    public string StreamingAssetsPath { get; private set; }

    /// <summary>仅在工作线程内使用；主线程勿直接调用 EDSDK。读取句柄用（只读）。</summary>
    public IntPtr Camera { get { return _camera; } }

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
                Log("事件泵已启动（EdsGetEvent 循环运行中）");
              }
              uint ev = EDSDKAPI.EdsGetEvent();
              if (ev != EDSDKConsts.EDS_ERR_OK && !_eventPumpLoggedError)
              {
                _eventPumpLoggedError = true;
                _main.Enqueue(() => OnError?.Invoke("EdsGetEvent: " + ev));
              }
            }

            // 2) 处理任务：空则阻塞至多 50ms（即上面 20Hz 的由来）；有任务则批处理并继续。
            if (_jobs.TryTake(out Action job, 50))
            {
              try { job(); }
              catch (Exception e) { _main.Enqueue(() => OnError?.Invoke("CanonWorker: " + e.Message)); }

              while (_jobs.TryTake(out Action j, 0))
              {
                try { j(); }
                catch (Exception e) { _main.Enqueue(() => OnError?.Invoke("CanonWorker: " + e.Message)); }
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
      _workerThread.Start();
      _workerStarted = true;
      _running = true;
    }

    // ================= 公开操作（异步，投递到工作线程） =================

    public void Open()
    {
      if (_camera != IntPtr.Zero || _sessionOpen) { return; }
      _jobs.Add(DoOpenSession);
    }

    public void OpenAsync()
    {
      if (_camera != IntPtr.Zero || _sessionOpen) { return; }
      _jobs.Add(DoOpenSession);
    }

    public void Close()
    {
      _jobs.Add(DoClose);
    }

    public void StartLiveView()
    {
      _jobs.Add(DoStartLiveView);
    }

    public void StopLiveView()
    {
      _jobs.Add(DoStopLiveView);
    }

    public void TakePhoto()
    {
      _jobs.Add(DoTakePhoto);
    }

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
      Log("SetObjectEventHandler err=0x" + err.ToString("X8"));
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
      Log($"SaveTo=Host set err=0x{saveToErr:X8}, SetCapacity err=0x{capErr:X8}, SaveTo readback={saveToRead} ({(EdsSaveTo)saveToRead})");

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
      Log("TakePhoto: shutter(completely) err=0x" + shutterErr.ToString("X8") + " availShots(前)=" + beforeShot);
      uint offErr = EDSDKAPI.EdsSendCommand(_camera, EDSDKConsts.CameraCommand_PressShutterButton,
        (int)EdsShutterButton.CameraCommand_ShutterButton_OFF);
      Log("TakePhoto: shutter(off) err=0x" + offErr.ToString("X8"));
      uint afterShot = 0;
      EDSDKAPI.EdsGetPropertyData(_camera, EDSDKConsts.PropID_AvailableShots, 0, out afterShot);
      Log("TakePhoto: availShots(后)=" + afterShot + " (若后<前则相机确实拍了)");
      if (shutterErr != EDSDKConsts.EDS_ERR_OK) { PostError("TakePhoto: " + shutterErr); return; }
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
        _jobs.Add(LiveViewTick);  // 排到队尾，让拍照等指令可插队
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
      if (err != EDSDKConsts.EDS_ERR_OK) { Log($"DownloadPhoto: GetDirectoryItemInfo err=0x{err:X8}"); PostError("GetDirectoryItemInfo: " + err); EDSDKAPI.EdsRelease(dirItem); _downloading = false; return; }
      Log($"DownloadPhoto: dirItem={dirItem}, size={info.Size}, file={info.szFileName}");

      string fileName = info.szFileName;
      if (string.IsNullOrEmpty(fileName)) { fileName = "IMG_" + DateTime.Now.Ticks + ".JPG"; }

      string destPath = fileName;
      try
      {
        Directory.CreateDirectory(StreamingAssetsPath);
        destPath = Path.Combine(StreamingAssetsPath, fileName);
      }
      catch (Exception e) { Log("DownloadPhoto: PersistPath " + e.Message); PostError("PersistPath: " + e.Message); EDSDKAPI.EdsRelease(dirItem); _downloading = false; return; }

      IntPtr stream = IntPtr.Zero;
      err = EDSDKAPI.EdsCreateFileStream(destPath, EdsFileCreateDisposition.CreateAlways, EdsAccess.ReadWrite, out stream);
      if (err != EDSDKConsts.EDS_ERR_OK) { Log($"DownloadPhoto: CreateFileStream err=0x{err:X8}"); PostError("CreateFileStream: " + err); }
      if (err == EDSDKConsts.EDS_ERR_OK)
      {
        err = EDSDKAPI.EdsSetProgressCallback(stream, _progressHandler, EdsProgressOption.Periodically, stream);
        if (err != EDSDKConsts.EDS_ERR_OK) { Log($"DownloadPhoto: SetProgressCallback err=0x{err:X8}"); }
      }
      if (err == EDSDKConsts.EDS_ERR_OK) { err = EDSDKAPI.EdsDownload(dirItem, info.Size, stream); }
      if (err != EDSDKConsts.EDS_ERR_OK) { Log($"DownloadPhoto: EdsDownload err=0x{err:X8}"); PostError("Download: " + err); }
      if (err == EDSDKConsts.EDS_ERR_OK) { err = EDSDKAPI.EdsDownloadComplete(dirItem); }
      if (err != EDSDKConsts.EDS_ERR_OK) { Log($"DownloadPhoto: DownloadComplete err=0x{err:X8}"); PostError("DownloadComplete: " + err); }

      if (stream != IntPtr.Zero) { EDSDKAPI.EdsRelease(stream); }
      EDSDKAPI.EdsRelease(dirItem);

      if (err == EDSDKConsts.EDS_ERR_OK)
      {
        string path = destPath;
        Log("DownloadPhoto: SUCCESS -> " + path);
        _main.Enqueue(() => OnPhotoSaved?.Invoke(path));
      }
      else { Log("DownloadPhoto: FAILED err=0x" + err.ToString("X8")); }
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
          // 已有传输进行中：取消并释放这次的，避免缓冲冲突
          EDSDKAPI.EdsDownloadCancel(inRef);
          EDSDKAPI.EdsRelease(inRef);
          Log("ObjectEvent transfer 但已有下载进行中，取消本次 (0x" + inEvent.ToString("X") + ")");
          return EDSDKConsts.EDS_ERR_OK;
        }
        _downloading = true;
        _pendingItem = inRef;
        Log($"ObjectEvent transfer (0x{inEvent:X}) -> enqueue download (dirItem={inRef})");
        _jobs.Add(DownloadPhoto);
      }
      else
      {
        Log($"ObjectEvent 0x{inEvent:X} (unhandled)");
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
        Log("StateEvent Shutdown -> 拆会话以支持重连");
        _jobs.Add(HandleCameraShutdown);
      }
      else if (inEvent == EDSDKConsts.StateEvent_CaptureError || inEvent == EDSDKConsts.StateEvent_InternalError)
      {
        uint p = inParameter;
        _main.Enqueue(() => OnError?.Invoke($"Camera state 0x{inEvent:X} param 0x{p:X}"));
      }
      return EDSDKConsts.EDS_ERR_OK;
    }

    /// <summary>相机关机/断开：清理会话与相机引用，但不终止 SDK，便于下次 Open() 直接重连。</summary>
    private void HandleCameraShutdown()
    {
      _liveViewActive = false;
      _downloading = false;
      if (_pendingItem != IntPtr.Zero) { EDSDKAPI.EdsRelease(_pendingItem); _pendingItem = IntPtr.Zero; }

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

    private void PostError(string msg)
    {
      _main.Enqueue(() => OnError?.Invoke(msg));
    }

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
        Log("EdsInitializeSDK 返回 0x2，疑似已初始化，尝试 terminate+reinit 恢复");
        EDSDKAPI.EdsTerminateSDK();
        err = EDSDKAPI.EdsInitializeSDK();
        if (err == EDSDKConsts.EDS_ERR_OK) { _sdkReady = true; return true; }
      }

      PostError("EdsInitializeSDK: " + err);
      return false;
    }

    private static void Log(string msg)
    {
      Debug.Log("[Canon] " + msg);
    }
  }
}
