/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System;
using System.Collections.Concurrent;
using ToneTuneToolkit.Common;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ToneTuneToolkit.Networking.WebSocket
{
  /// <summary>
  /// 通用 WebSocket 服务端（仅负责字节搬运 + 包结构解析，不耦合任何业务）
  ///
  /// 帧类型（仅作为二进制帧的路由标签）：
  ///   - 文本帧：原样上抛为 OnTextReceived(sessionId, text)
  ///   - 二进制帧：
  ///       首字节 == WSCustomBinaryPacket.TYPE_IMAGE (0x01)
  ///         → OnBinaryImageReceived(sessionId, code, payload)
  ///       其它自定义类型
  ///         → OnCustomDataReceived(sessionId, type, code, payload)
  ///       长度不足以构成有效包头
  ///         → 视为 0x00 类型透传给 OnCustomDataReceived
  ///
  /// 特点：
  ///   - 协议层只解析包结构（type/code/length），不假设 payload 含义；
  ///   - 业务层（你的项目）自行决定：是否还原图片、如何存盘、是否回 ACK、ACK 文案；
  ///   - 后台线程只负责入队，所有 Unity API 在主线程 (Update) 消费；
  ///   - 会话独立：每个客户端连接对应一个 MessageService 实例，互不干扰。
  /// </summary>
  public class WSServer : SingletonMaster<WSServer>
  {
    private WebSocketServer wss;

    private int serverPort = 1000;
    private const string ServicePath = "/Message";



    /// <summary>C# 事件，业务层代码订阅</summary>
    public event Action<string, string> OnTextReceived;
    public event Action<string, int, byte[]> OnImageReceived;
    public event Action<string, byte, int, byte[]> OnCustomDataReceived;

    // 线程安全队列：后台线程 (OnMessage) 收到的数据先入队，主线程 (Update) 再逐条安全处理
    private readonly ConcurrentQueue<IncomingData> msgQueue = new ConcurrentQueue<IncomingData>();

    // sessionId → MessageService 映射，供业务层通过 sessionId 反查并主动回包
    private readonly ConcurrentDictionary<string, MessageService> sessionMap = new ConcurrentDictionary<string, MessageService>();

    // ==================================================

    private void Start() => Init();

    private void Update()
    {
      // Unity 主线程消费队列，安全操作所有 Unity API
      while (msgQueue.TryDequeue(out IncomingData data))
      {
        ProcessMessageOnMainThread(data);
      }
    }

    private void OnDestroy() => UnInit();

    // ==================================================

    private void Init()
    {
      SwitchServer(true);
    }

    private void UnInit()
    {
      SwitchServer(false);
    }

    // ==================================================
    #region 服务器控制

    public void SwitchServer(bool value)
    {
      if (wss != null) { return; }

      if (value) // 启动服务器
      {
        try
        {
          wss = new WebSocketServer(serverPort);
          wss.AddWebSocketService(ServicePath, () => new MessageService(this, msgQueue));
          wss.Start();

          Debug.Log(@$"[WSServer] <color=green>WebSocketSharp started.</color>");
          Debug.Log(@$"[WSServer] Listening at <color=white>ws://127.0.0.1:{serverPort}{ServicePath}</color>.");
        }
        catch (Exception e)
        {
          Debug.LogError(@$"[WSServer] Start failed, because: {e.Message}");
          wss = null;
        }
      }
      else // 停止服务器
      {
        try
        {
          wss.Stop();
          Debug.Log(@$"[WSServer] Server stopped. Port released.");
        }
        catch (Exception e) { Debug.LogError(@$"[WSServer] Server stop failed, because: {e.Message}"); }
        finally { wss = null; }
      }
    }

    public void RegisterSession(string sessionId, MessageService session)
    {
      if (sessionMap.TryAdd(sessionId, session))
      {
        Debug.Log($"[WSServer] Session register: {sessionId}");
      }
    }

    public void UnregisterSession(string sessionId)
    {
      if (sessionMap.TryRemove(sessionId, out _))
      {
        Debug.Log($"[WSServer] Session unregister: {sessionId}");
      }
    }

    public bool SendTextToSession(string sessionId, string message)
    {
      if (sessionMap.TryGetValue(sessionId, out MessageService session))
      {
        session.SendBack(message);
        return true;
      }

      Debug.LogWarning($"[WSServer] SendTextToSession 失败：找不到会话 {sessionId}");
      return false;
    }

    #endregion
    // ==================================================
    #region 主线程业务分发（仅做包结构解析，不假设 payload 含义）

    /// <summary>
    /// ★★★ 主线程消费入口。只做帧类型分发，不做任何业务处理 ★★★
    /// </summary>
    private void ProcessMessageOnMainThread(IncomingData incoming)
    {
      if (incoming.IsText)
      {
        HandleText(incoming);
        return;
      }

      // 二进制帧：先按首字节路由
      byte[] packet = incoming.BinaryData;
      if (packet != null && packet.Length >= WSCustomBinaryPacket.HEADER_SIZE && packet[0] == WSCustomBinaryPacket.TYPE_IMAGE)
      {
        HandleBinaryImage(incoming);
      }
      else { HandleCustomData(incoming); }
    }

    /// <summary>
    /// 通用文本帧处理：原样上抛，不做任何协议解析（ID:xxx 这类业务约定由业务层自行处理）
    /// </summary>
    private void HandleText(IncomingData incoming)
    {
      string text = incoming.TextData ?? string.Empty;
      Debug.Log($"[WSServer] 收到文本 (会话 {incoming.SessionId}): {text}");
      OnTextReceived?.Invoke(incoming.SessionId, text);
    }

    /// <summary>
    /// 通用图片帧处理（type=0x01）：
    ///   - 仅按 WSCustomBinaryPacket 协议解析包头，把 (code, payload) 原样交给业务层
    ///   - 是否还原为 Texture2D、是否存盘、是否回 ACK——一律由业务层决定
    /// </summary>
    private void HandleBinaryImage(IncomingData incoming)
    {
      (int code, byte[] imageBytes) = WSCustomBinaryPacket.Unpack(incoming.BinaryData);
      if (imageBytes == null)
      {
        Debug.LogWarning($"[WSServer] 会话 {incoming.SessionId} 的图片包解析失败 (长度或类型不匹配)。");
        return;
      }

      Debug.Log($"<color=lime>[WSServer] 收到图片帧 (会话 {incoming.SessionId}, code={code}), payload={imageBytes.Length} 字节</color>");

      OnImageReceived?.Invoke(incoming.SessionId, code, imageBytes);
    }

    /// <summary>
    /// 通用自定义数据帧处理（非 0x01 的二进制帧）：
    ///   - 头部足够时按协议解析 (type, code, payload)；
    ///   - 头部不足时 type=0x00，整包作为 payload 透传；
    ///   - 长度字段非法 / 包被截断时整包透传并 warn。
    /// </summary>
    private void HandleCustomData(IncomingData incoming)
    {
      byte[] packet = incoming.BinaryData;
      string sessionId = incoming.SessionId;

      byte type;
      int code;
      byte[] payload;

      if (packet != null && packet.Length >= WSCustomBinaryPacket.HEADER_SIZE)
      {
        type = packet[0];
        code = BitConverter.ToInt32(packet, 1);
        int payloadLength = BitConverter.ToInt32(packet, 5);

        if (payloadLength < 0 || payloadLength > packet.Length - WSCustomBinaryPacket.HEADER_SIZE)
        {
          Debug.LogWarning($"[WSServer] 会话 {sessionId} 的自定义包长度字段非法 (type=0x{type:X2}, len={payloadLength})，将整包透传。");
          payload = packet;
        }
        else
        {
          payload = new byte[payloadLength];
          Buffer.BlockCopy(packet, WSCustomBinaryPacket.HEADER_SIZE, payload, 0, payloadLength);
        }
      }
      else
      {
        type = 0x00;
        code = 0;
        payload = packet ?? Array.Empty<byte>();
      }

      Debug.Log($"[WSServer] 收到自定义数据 (会话 {sessionId}, type=0x{type:X2}, code={code}, payloadLen={payload.Length})");

      OnCustomDataReceived?.Invoke(sessionId, type, code, payload);
    }

    #endregion
  }

  /// <summary>
  /// 专用于管理连接与数据接收的自定义行为类（每个客户端连接对应一个实例）。
  /// </summary>
  public class MessageService : WebSocketBehavior
  {
    private readonly ConcurrentQueue<IncomingData> queue;
    private readonly WSServer server;

    public MessageService(WSServer owner, ConcurrentQueue<IncomingData> mainThreadQueue)
    {
      server = owner;
      queue = mainThreadQueue;
    }

    protected override void OnOpen()
    {
      server.RegisterSession(ID, this);
      Debug.Log($"[WSServer] 有新的客户端连入，会话ID: {ID}");
    }

    protected override void OnMessage(MessageEventArgs e)
    {
      if (e.IsText)
      {
        queue.Enqueue(IncomingData.Text(this, ID, e.Data));
      }
      else if (e.IsBinary)
      {
        queue.Enqueue(IncomingData.Binary(this, ID, e.RawData));
      }
    }

    protected override void OnClose(CloseEventArgs e)
    {
      server.UnregisterSession(ID);
      Debug.Log($"[WSServer] 客户端连接关闭 (会话 {ID})。原因: {e.Reason}");
    }

    protected override void OnError(WebSocketSharp.ErrorEventArgs e) { Debug.LogError($"[WSServer] 会话 {ID} 发生错误: {e.Message}"); }

    /// <summary>
    /// 供主线程回发文本（内部封装，避免外部直接触碰 WebSocketBehavior）
    /// </summary>
    public void SendBack(string message)
    {
      if (State != WebSocketState.Open) { return; }

      SendAsync(message, completed =>
      {
        if (completed)
        {
          Debug.Log($"[Server -> Client] 文本已发送 (会话 {ID})");
        }
      });
    }
  }

  /// <summary>
  /// 辅助结构体：在后台线程与 Unity 主线程之间安全传递数据包。
  /// </summary>
  public struct IncomingData
  {
    private readonly MessageService session;

    public string SessionId { get; }
    public bool IsText { get; }
    public bool IsBinary => !IsText;
    public string TextData { get; }
    public byte[] BinaryData { get; }

    private IncomingData(MessageService session, string sessionId, bool isText, string text, byte[] binary)
    {
      this.session = session;
      SessionId = sessionId;
      IsText = isText;
      TextData = text;
      BinaryData = binary;
    }

    public static IncomingData Text(MessageService session, string sessionId, string text)
    {
      return new IncomingData(session, sessionId, true, text, null);
    }

    public static IncomingData Binary(MessageService session, string sessionId, byte[] binary)
    {
      return new IncomingData(session, sessionId, false, null, binary);
    }

    /// <summary>主线程安全地向发来该消息的客户端回发文本</summary>
    public void SendBack(string message)
    {
      session?.SendBack(message);
    }
  }
}