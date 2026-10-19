/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Z CAM MJPEG 取景源：GET /mjpeg_stream 长连接收流（相机内固定 720p 编码器，调试通道）。
/// 切帧逻辑由 ZCAMJpegFrameAccumulator 承担（与 RTSP 源共用）。
/// 设计：收流线程只做「累积→切帧→存最新帧」；解码在主线程（Manager.Update）执行，
/// 积压帧直接被覆盖丢弃（丢帧优先，保延迟不保完整）。
/// </summary>

using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ToneTuneToolkit.IO.ZCAM
{
  /// <summary>
  /// MJPEG 取景流。生命周期：Start()（主线程调用）→ 主线程每帧 TryConsumeLatestFrame() → Stop()
  /// </summary>
  public class ZCAMMjpegViewSource : IZCAMViewSource
  {
    private readonly string _url;
    private readonly ZCAMJpegFrameAccumulator _frames = new ZCAMJpegFrameAccumulator();

    private UnityWebRequest _request;
    private volatile bool _stopRequested;

    public ZCAMMjpegViewSource(string baseUrl)
    {
      _url = baseUrl + "/mjpeg_stream";
    }

    public string Url => _url;

    public Task FirstFrameTask => _frames.FirstFrameTask;

    /// <summary>发起取景流请求（必须在主线程调用）。返回后由调用方在 Update 中轮询。</summary>
    public void Start()
    {
      _request = UnityWebRequest.Get(_url);
      _request.timeout = 0; // 长连接不设超时
      _request.downloadHandler = new MjpegDownloadHandler(this, new byte[64 * 1024]);
      _request.SendWebRequest();
    }

    /// <summary>停止取景流并释放请求。可重复调用。</summary>
    public void Stop()
    {
      _stopRequested = true;
      if (_request != null)
      {
        _request.Abort();
        _request.Dispose();
        _request = null;
      }
      _frames.SignalFailure("取景流在首帧前被停止");
    }

    public bool IsCompleted => _request == null || _request.isDone;

    public string Error
    {
      get
      {
        if (!_stopRequested && _request != null && _request.isDone)
        {
          return $"取景流中断: {_request.error}";
        }
        return null;
      }
    }

    public byte[] TryConsumeLatestFrame()
    {
      return _frames.TryTakeLatest();
    }

    public bool IsRawRgba => false;
    public int FrameWidth => 0;
    public int FrameHeight => 0;

    private void OnChunkReceived(byte[] data, int dataLength)
    {
      _frames.Feed(data, dataLength);
    }

    // ================================ 下载处理器 ================================

    /// <summary>收流回调可能在非主线程，全部转交给帧组装器（其内部加锁）</summary>
    private class MjpegDownloadHandler : DownloadHandlerScript
    {
      private readonly ZCAMMjpegViewSource _owner;

      public MjpegDownloadHandler(ZCAMMjpegViewSource owner, byte[] preallocatedBuffer) : base(preallocatedBuffer)
      {
        _owner = owner;
      }

      protected override bool ReceiveData(byte[] data, int dataLength)
      {
        if (dataLength > 0) _owner.OnChunkReceived(data, dataLength);
        return true; // false 会中止请求
      }
    }
  }
}
