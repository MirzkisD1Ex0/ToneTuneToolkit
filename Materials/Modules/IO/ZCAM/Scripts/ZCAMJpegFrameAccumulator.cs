/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// JPEG 帧组装器：喂入任意分片的字节流（MJPEG HTTP 收包），
/// 按 SOI/EOI(FFD8/FFD9) 标记切出完整 JPEG 帧，只保留最新一帧（丢帧优先）。
/// 线程模型：Feed 可在任意线程调用（内部加锁）；TryTakeLatest 由主线程消费。
/// </summary>

using System.Threading;
using System.Threading.Tasks;

namespace ToneTuneToolkit.IO.ZCAM
{
  internal class ZCAMJpegFrameAccumulator
  {
    private const int InitialCapacity = 1024 * 1024; // 1MB（1080p JPEG 帧约 200~500KB）
    private const int ChunkMargin = 256 * 1024;      // 剩余空间低于此值时扩容
    private const int MinFrameBytes = 64;            // 小于该长度的「帧」视为噪声丢弃

    private const byte SOI_LO = 0xD8; // FFD8 = JPEG Start Of Image
    private const byte EOI_LO = 0xD9; // FFD9 = JPEG End Of Image

    private readonly object _sync = new object();

    // —— 收流累积缓冲（仅 Feed 线程写，_sync 保护）——
    private byte[] _acc = new byte[InitialCapacity];
    private int _accLen = 0;
    private int _soiIndex = -1; // 当前帧起始（SOI）位置，-1 表示尚未找到

    // —— 最新帧槽位（主线程消费）——
    private byte[] _latestFrame;
    private bool _hasNewFrame;

    private readonly TaskCompletionSource<bool> _firstFrame =
        new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    private bool _firstFrameSignalled;

    /// <summary>首帧到达即完成；被 SignalFailure 置为 Faulted</summary>
    public Task FirstFrameTask => _firstFrame.Task;

    /// <summary>喂入一段字节（任意线程）</summary>
    public void Feed(byte[] data, int length)
    {
      lock (_sync)
      {
        if (length <= 0) return;
        EnsureCapacity(_accLen + length);
        System.Buffer.BlockCopy(data, 0, _acc, _accLen, length);
        _accLen += length;
        ParseFrames();
      }
    }

    /// <summary>取走最新帧（主线程）。没有新帧返回 null；旧帧被丢弃只留最新。</summary>
    public byte[] TryTakeLatest()
    {
      lock (_sync)
      {
        if (!_hasNewFrame || _latestFrame == null) return null;
        _hasNewFrame = false;
        var frame = _latestFrame;
        _latestFrame = null;
        return frame;
      }
    }

    /// <summary>首帧前失败：置 Faulted（如进程退出/流被停止）；已有首帧则忽略</summary>
    public void SignalFailure(string message)
    {
      lock (_sync)
      {
        if (!_firstFrameSignalled)
        {
          _firstFrame.TrySetException(new ZCAMException(message));
        }
      }
    }

    // ================================ 切帧 ================================

    private void ParseFrames()
    {
      while (true)
      {
        if (_soiIndex < 0)
        {
          int soi = FindMarker(0, SOI_LO);
          if (soi < 0)
          {
            // 没有 SOI：丢弃全部，只留最后 1 字节（标记可能跨包）
            if (_accLen > 1)
            {
              _acc[0] = _acc[_accLen - 1];
              _accLen = 1;
            }
            return;
          }
          _soiIndex = soi;
          if (_soiIndex > 0) ShiftOut(_soiIndex); // 帧对齐到 0，丢弃帧头之前的杂项字节
        }

        int eoi = FindMarker(2, EOI_LO); // 从 SOI+2 起找 EOI
        if (eoi < 0)
        {
          // 帧未收完；空间不足则扩容
          if (_acc.Length - _accLen < ChunkMargin) Grow();
          return;
        }

        int frameLen = eoi + 2; // 含 EOI 两字节
        if (frameLen >= MinFrameBytes)
        {
          var frame = new byte[frameLen];
          System.Buffer.BlockCopy(_acc, 0, frame, 0, frameLen);
          StoreLatest(frame);
        }
        ShiftOut(frameLen);
        _soiIndex = -1;
      }
    }

    /// <summary>从 offset 起查找 FF markerLo 标记对，返回标记首字节位置，找不到返回 -1</summary>
    private int FindMarker(int offset, byte markerLo)
    {
      for (int i = offset; i < _accLen - 1; i++)
      {
        if (_acc[i] == 0xFF && _acc[i + 1] == markerLo) return i;
      }
      return -1;
    }

    private void StoreLatest(byte[] frame)
    {
      _latestFrame = frame;      // 覆盖旧帧 = 丢帧优先
      _hasNewFrame = true;
      if (!_firstFrameSignalled)
      {
        _firstFrameSignalled = true;
        _firstFrame.TrySetResult(true);
      }
    }

    /// <summary>把 _acc 前 count 字节移出，剩余内容前移</summary>
    private void ShiftOut(int count)
    {
      _accLen -= count;
      System.Buffer.BlockCopy(_acc, count, _acc, 0, _accLen);
    }

    private void EnsureCapacity(int needed)
    {
      if (needed <= _acc.Length) return;
      int newSize = _acc.Length;
      while (newSize < needed) newSize *= 2;
      var bigger = new byte[newSize];
      System.Buffer.BlockCopy(_acc, 0, bigger, 0, _accLen);
      _acc = bigger;
    }

    private void Grow()
    {
      var bigger = new byte[_acc.Length * 2];
      System.Buffer.BlockCopy(_acc, 0, bigger, 0, _accLen);
      _acc = bigger;
    }
  }
}
