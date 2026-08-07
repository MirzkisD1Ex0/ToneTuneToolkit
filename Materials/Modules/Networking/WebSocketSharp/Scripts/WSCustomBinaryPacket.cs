/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System;

namespace ToneTuneToolkit.Networking.WebSocket
{
  /// <summary>
  /// 图片二进制包（WebSocket binary frame 专用）
  ///
  /// 包格式（定长头 + 变长体）：
  /// [1 字节 类型]            —— 0x01 = 图片
  /// [4 字节 编号 (Int32)]
  /// [4 字节 图片长度 (Int32)]
  /// [N 字节 图片数据]        —— N = 上面声明的长度
  ///
  /// 为什么不用 JSON / Base64：
  /// 1. WebSocket text frame 默认 1MB 上限，Base64 后图片必然炸
  /// 2. 一帧 = 一图，避免和 text frame 错位
  /// 3. binary frame 无协议大小限制，承载 MB 级图片无压力
  /// </summary>
  public static class WSCustomBinaryPacket
  {
    public const byte TYPE_IMAGE = 0x01; // 类型常量（首字节，方便扩展其他帧类型）
    public const int HEADER_SIZE = 9; // 头部长度：1 (类型) + 4 (code) + 4 (length) = 9 字节

    /// <summary>
    /// 打包：(code, imageBytes) → 单一 binary frame 字节流
    /// </summary>
    public static byte[] Pack(int code, byte[] imageBytes)
    {
      if (imageBytes == null) { imageBytes = Array.Empty<byte>(); }

      int payloadLength = imageBytes.Length;
      byte[] packet = new byte[HEADER_SIZE + payloadLength];

      packet[0] = TYPE_IMAGE;

      // 小端字节序（BitConverter 默认）
      Buffer.BlockCopy(BitConverter.GetBytes(code), 0, packet, 1, 4);
      Buffer.BlockCopy(BitConverter.GetBytes(payloadLength), 0, packet, 5, 4);
      Buffer.BlockCopy(imageBytes, 0, packet, HEADER_SIZE, payloadLength);

      return packet;
    }

    /// <summary>
    /// 解包：binary frame 字节流 → (code, imageBytes)
    /// 长度不足或类型不匹配时返回 (0, null)，调用方应忽略并 log。
    /// </summary>
    public static (int code, byte[] imageBytes) Unpack(byte[] packet)
    {
      if (packet == null || packet.Length < HEADER_SIZE) { return (0, null); }
      if (packet[0] != TYPE_IMAGE) { return (0, null); }

      int code = BitConverter.ToInt32(packet, 1);
      int payloadLength = BitConverter.ToInt32(packet, 5);

      // 防御性检查：长度字段和实际包长对不上，说明帧被截断或格式错误
      if (payloadLength < 0
          || payloadLength > packet.Length - HEADER_SIZE)
      {
        return (0, null);
      }

      byte[] imageBytes = new byte[payloadLength];
      Buffer.BlockCopy(packet, HEADER_SIZE, imageBytes, 0, payloadLength);
      return (code, imageBytes);
    }
  }
}