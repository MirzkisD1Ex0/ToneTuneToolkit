/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// SSP 取景源：P/Invoke zcam_ssp.dll（libssp 收流 + Media Foundation 解码，原生层产出 RGBA 槽位）。
/// 本类只做薄封装：轮询原生槽位（丢帧优先）+ 动态扩容缓冲 + 首帧信号。
/// 运行时依赖 Plugins 下的 zcam_ssp.dll 与 libssp.dll。
/// </summary>

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ToneTuneToolkit.IO.ZCAM
{
  public class ZCAMSspViewSource : IZCAMViewSource
  {
    private const string Dll = "zcam_ssp";
    private const ushort SspPort = 9999;

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr zssp_create([MarshalAs(UnmanagedType.LPStr)] string ip, ushort port, uint bufMb);

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern int zssp_start(IntPtr ctx);

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern int zssp_stop(IntPtr ctx);

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void zssp_destroy(IntPtr ctx);

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern int zssp_poll_frame(IntPtr ctx, byte[] dst, int dstCap,
        out int outW, out int outH, out ulong outPts, out uint outFrmNo);

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern int zssp_has_first_frame(IntPtr ctx);

    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr zssp_last_error(IntPtr ctx);

    private static string LastErrorString(IntPtr ctx)
    {
      var p = zssp_last_error(ctx);
      return p == IntPtr.Zero ? string.Empty : Marshal.PtrToStringAnsi(p) ?? string.Empty;
    }

    private readonly string _ip;
    private IntPtr _ctx = IntPtr.Zero;
    private volatile bool _stopRequested;

    private byte[] _buffer = new byte[1920 * 1080 * 4]; // 首帧后按 -2 反馈扩容
    private readonly TaskCompletionSource<bool> _firstFrame =
        new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    private bool _firstFrameSignalled;

    /// <summary>最近一帧的相机侧 pts（墙钟时间戳，多机对齐用）</summary>
    public ulong LatestPts { get; private set; }
    /// <summary>最近一帧的相机侧帧号</summary>
    public uint LatestFrameNo { get; private set; }

    public int FrameWidth { get; private set; }
    public int FrameHeight { get; private set; }

    /// <summary>SSP 源产出 RGBA 裸数据（非 JPEG），消费端走 LoadRawTextureData</summary>
    public bool IsRawRgba => true;

    public ZCAMSspViewSource(string cameraIp)
    {
      _ip = cameraIp.Replace("http://", string.Empty);
    }

    // ================================ IZCAMViewSource ================================

    public Task FirstFrameTask => _firstFrame.Task;

    public void Start()
    {
      _ctx = zssp_create(_ip, SspPort, 4);
      if (_ctx == IntPtr.Zero)
      {
        throw new ZCAMException($"zcam_ssp 初始化失败（MFStartup 失败或内存不足），ip={_ip}");
      }
      if (zssp_start(_ctx) != 0)
      {
        zssp_destroy(_ctx);
        _ctx = IntPtr.Zero;
        throw new ZCAMException($"SSP 启动失败，ip={_ip}:{SspPort}");
      }
    }

    public void Stop()
    {
      _stopRequested = true;
      if (_ctx != IntPtr.Zero)
      {
        zssp_stop(_ctx);
        zssp_destroy(_ctx);
        _ctx = IntPtr.Zero;
      }
      if (!_firstFrameSignalled)
      {
        _firstFrame.TrySetException(new ZCAMException("SSP 取景在首帧前被停止"));
      }
    }

    /// <summary>出错即视为流结束（原生层把断线/异常都记入 last_error）</summary>
    public bool IsCompleted => !_stopRequested && _ctx != IntPtr.Zero && LastErrorString(_ctx).Length > 0;

    public string Error
    {
      get
      {
        if (_stopRequested || _ctx == IntPtr.Zero) return null;
        var msg = LastErrorString(_ctx);
        return msg.Length > 0 ? $"SSP: {msg}" : null;
      }
    }

    public byte[] TryConsumeLatestFrame()
    {
      if (_ctx == IntPtr.Zero) return null;
      int n, w, h;
      ulong pts;
      uint frmNo;
      // -2 = 缓冲不够（w/h 已告知所需尺寸，帧仍在槽内）→ 扩容重试
      while ((n = zssp_poll_frame(_ctx, _buffer, _buffer.Length, out w, out h, out pts, out frmNo)) == -2)
      {
        Array.Resize(ref _buffer, w * h * 4);
      }
      if (n <= 0) return null;

      FrameWidth = w;
      FrameHeight = h;
      LatestPts = pts;
      LatestFrameNo = frmNo;

      if (!_firstFrameSignalled)
      {
        _firstFrameSignalled = true;
        _firstFrame.TrySetResult(true);
      }

      // 拷出 + 行序翻转：Unity 纹理的行 0 在底部（UV 原点左下），
      // 而图像数据的行 0 在顶部——不翻转则画面上下颠倒
      var frame = new byte[n];
      int rowBytes = FrameWidth * 4;
      int rows = n / rowBytes;
      for (int r = 0; r < rows; r++)
      {
        Buffer.BlockCopy(_buffer, (rows - 1 - r) * rowBytes, frame, r * rowBytes, rowBytes);
      }
      return frame;
    }
  }
}
