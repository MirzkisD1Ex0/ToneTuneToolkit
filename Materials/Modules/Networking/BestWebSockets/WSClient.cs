/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using UnityEngine;
using Best.WebSockets;
using Best.HTTP.Shared.PlatformSupport.Memory;
using System;
using System.Collections;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Networking.WebSocket
{
  using WebSocket = Best.WebSockets.WebSocket;

  /// <summary>
  /// WebSocket客户端
  /// 设置服务器地址
  /// 打开Client
  /// 发送
  /// </summary>
  public class WSClient : SingletonMaster<WSClient>
  {
    private WebSocket ws;

    private string serverIP = "127.0.0.1";
    private string serverPort = "4399";



    private const float RECONNECT_INTERVAL = 30f;
    private const int MAX_RETRY_COUNT = 10;
    private int retryCount = 0;
    private Coroutine reconnectCoroutine;



    public static event Action OnConnect;
    public static event Action<int, string> OnDisconnect;
    public static event Action<string> OnStringReceive;
    public static event Action<byte[]> OnBinaryReceive;

    public bool IsConnected => ws != null && ws.IsOpen;

    // ==================================================

    // private void Start() => Init();
    private void OnDestroy()
    {
      CancelReconnect();
      UnInit();
    }

    // ==================================================

    // private void Init() { SwitchWebSocketClient(true); }

    private void UnInit()
    {
      SwitchClient(false);
    }

    // ==================================================

    /// <summary>
    /// 设置服务器地址和端口
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void SetServerInfo(string ip, string port)
    {
      serverIP = ip;
      serverPort = port;
    }

    /// <summary>
    /// 开关客户端
    /// </summary>
    /// <param name="value"></param>
    public void SwitchClient(bool value)
    {
      if (value)
      {
        CancelReconnect();
        if (ws != null)
        {
          ws.Close();
          ws = null;
        }

        ws = new WebSocket(new Uri(@$"ws://{serverIP}:{serverPort}/Message"));

        ws.OnOpen += WhenOpen;
        ws.OnMessage += WhenMessage;
        ws.OnBinary += WhenBinary;
        ws.OnClosed += WhenWSCClosed;

        ws.Open();
      }
      else
      {
        CancelReconnect();
        if (ws != null) { ws.Close(); }
      }
    }

    // ==================================================

    private void WhenOpen(WebSocket ws)
    {
      Debug.Log(@$"[WSClient] <color=white>{serverIP}:{serverPort}</color> connected.");
      retryCount = 0;
      OnConnect?.Invoke();
    }

    private void WhenMessage(WebSocket ws, string value)
    {
      Debug.Log(@$"[WSClient] Message <color=white>{value}</color> received.");
      OnStringReceive?.Invoke(value);
    }

    private void WhenBinary(WebSocket ws, BufferSegment data)
    {
      byte[] copy = new byte[data.Count];
      data.CopyTo(copy);

      Debug.Log(@$"[WSClient] Binary length <color=white>{data.Count}</color> received.");
      OnBinaryReceive?.Invoke(copy);
    }

    private void WhenWSCClosed(WebSocket ws, WebSocketStatusCodes code, string message)
    {
      if (code == WebSocketStatusCodes.NormalClosure)
      {
        Debug.Log(@$"[WSClient] Client closed. {message}");
        retryCount = 0;
      }
      else
      {
        Debug.LogWarning(@$"[WSClient] Client Error. Code {code}, because {message}");
        ScheduleReconnect();
      }
      OnDisconnect?.Invoke((int)code, message);
    }

    // ==================================================
    #region Auto Reconnect

    private void ScheduleReconnect()
    {
      if (retryCount >= MAX_RETRY_COUNT)
      {
        Debug.LogWarning(@$"[WSClient] Reconnect failed. Reached max retry count {MAX_RETRY_COUNT}. Stop reconnecting.");
        return;
      }

      retryCount++;
      Debug.Log(@$"[WSClient] Retry {retryCount}/{MAX_RETRY_COUNT} will be attempted in {RECONNECT_INTERVAL} seconds...");

      CancelReconnect();
      reconnectCoroutine = StartCoroutine(ReconnectCoroutine());
    }

    private IEnumerator ReconnectCoroutine()
    {
      yield return new WaitForSeconds(RECONNECT_INTERVAL);

      // 用户可能在等待期间关掉了组件
      if (this == null) { yield break; }

      Debug.Log(@$"[WSClient] Attempting retry {retryCount}/{MAX_RETRY_COUNT}...");
      reconnectCoroutine = null;
      SwitchClient(true);
      // 真正连上时会触发 OnOpen → WhenOpen（重置 _retryCount）
      // 再次失败会触发 OnClosed → ScheduleReconnect（继续下一次循环）
    }

    private void CancelReconnect()
    {
      if (reconnectCoroutine != null)
      {
        StopCoroutine(reconnectCoroutine);
        reconnectCoroutine = null;
      }
    }

    #endregion
    // ==================================================
    #region Send Message

    /// <summary>
    /// 发送字符串
    /// </summary>
    /// <param name="value"></param>
    public void SendWSMessage(string value)
    {
      if (!IsConnected)
      {
        Debug.LogWarning(@$"[WSClient] Connection not established.");
        return;
      }

      ws.Send(value);
      Debug.Log(@$"[WSClient] Send <color=white>{value}</color> to server.");
    }

    /// <summary>
    /// 发送字节流
    /// </summary>
    /// <param name="bytes"></param>
    public void SendWSMessage(byte[] bytes)
    {
      if (!IsConnected)
      {
        Debug.LogWarning(@$"[WSClient] Connection not established.");
        return;
      }

      if (bytes == null || bytes.Length == 0)
      {
        Debug.LogWarning(@$"[WSClient] Binary is null or empty.");
        return;
      }

      ws.Send(bytes);
      Debug.Log(@$"[WSClient] Send binary length <color=white>{bytes.Length}</color> to server.");
    }

    #endregion
  }
}