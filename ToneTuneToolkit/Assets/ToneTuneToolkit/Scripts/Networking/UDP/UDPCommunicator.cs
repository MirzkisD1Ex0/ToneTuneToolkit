/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.UDP
{
  /// <summary>
  /// UDP通讯器轻量版 // 客户端
  /// 修正了端口占用报错与线程停止逻辑
  /// </summary>
  public class UDPCommunicator : SingletonMaster<UDPCommunicator>
  {
    #region Path
    private string udpConfigPath => $"{Application.streamingAssetsPath}/Configs/udpconfig.json";
    #endregion

    #region Config
    private string localIP;
    private string localPort;
    private string targetIP;
    private string targetPort;
    private float reciveFrequency = .5f;
    private static Encoding ReciveMessageEncoding = Encoding.UTF8;
    private static Encoding SendMessageEncoding = Encoding.UTF8;
    #endregion

    #region Receive
    private UdpClient receiveUDPClient;
    private Thread receiveThread = null;
    private IPEndPoint remoteAddress;
    #endregion

    #region Value
    private string udpMessage;
    private static event UnityAction<string> OnMessageRecive;
    #endregion

    [SerializeField] private ConfigData configData;

    [Serializable]
    private class ConfigData
    {
      public string local_ip;
      public string local_port;
      public string target_ip;
      public string target_port;
      public float recive_frequency;
    }

    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    public void Init()
    {
      LoadConfig();
      remoteAddress = new IPEndPoint(IPAddress.Any, 0);

      // 初始化接收线程
      receiveThread = new Thread(MessageReceive)
      {
        IsBackground = true
      };
      receiveThread.Start();

      StartCoroutine(RepeatHookMessage());
    }

    public void UnInit()
    {
      StopCoroutine(RepeatHookMessage());

      if (receiveUDPClient != null)
      {
        receiveUDPClient.Close();
        receiveUDPClient = null;
      }

      if (receiveThread != null)
      {
        if (receiveThread.IsAlive)
        {
          receiveThread.Join(500); // 最多等待500ms
        }
        receiveThread = null;
      }

      Debug.Log("<color=green>[TTT UDPC]</color> Communicator Unloaded.");
    }

    // ==================================================

    private void LoadConfig()
    {
      if (!File.Exists(udpConfigPath))
      {
        Debug.LogError($"[TTT UDPC] Config file not found at: {udpConfigPath}");
        return;
      }

      string json = File.ReadAllText(udpConfigPath, Encoding.UTF8);
      configData = JsonUtility.FromJson<ConfigData>(json);

      localIP = configData.local_ip;
      localPort = configData.local_port;
      targetIP = configData.target_ip;
      targetPort = configData.target_port;
      reciveFrequency = configData.recive_frequency;
    }

    // ==================================================

    public static void AddEventListener(UnityAction<string> unityAction) => OnMessageRecive += unityAction;
    public static void RemoveEventListener(UnityAction<string> unityAction) => OnMessageRecive -= unityAction;

    private IEnumerator RepeatHookMessage()
    {
      while (true)
      {
        yield return new WaitForSeconds(reciveFrequency);

        if (string.IsNullOrEmpty(udpMessage)) continue;

        Debug.Log($"<color=white>[TTT UDPC]</color> Received: <color=cyan>[{udpMessage}]</color> from <color=white>[{remoteAddress}]</color>");
        OnMessageRecive?.Invoke(udpMessage);
        udpMessage = null;
      }
    }

    private void MessageReceive()
    {
      try
      {
        int port = int.Parse(localPort);
        receiveUDPClient = new UdpClient();

        receiveUDPClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        receiveUDPClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

        while (true)
        {
          byte[] receiveData = receiveUDPClient.Receive(ref remoteAddress);
          udpMessage = ReciveMessageEncoding.GetString(receiveData);
        }
      }
      catch (SocketException ex)
      {
        // 当调用 UnInit 中的 Close() 时，由于阻塞被强行中断，这里会捕获到异常
        // 这是正常的退出路径，无需报错
        Debug.LogWarning(@$"[TTT UDPC] Receive Thread stopped (Socket Closed). {ex}");
      }
      catch (Exception e)
      { Debug.LogError($"[TTT UDPC] Receive Thread Error: {e.Message}"); }
    }

    // ==================================================

    private static void MessageSend(string ip, int port, byte[] bytes)
    {
      try
      {
        IPEndPoint tempRemoteAddress = new IPEndPoint(IPAddress.Parse(ip), port);
        using (UdpClient sendClient = new UdpClient())
        {
          sendClient.Send(bytes, bytes.Length, tempRemoteAddress);
        }
      }
      catch (Exception e)
      { Debug.LogError($"[TTT UDPC] Send Error: {e.Message}"); }
    }



    public static void MessageSendOut(string message) => MessageSendOut(Instance.targetIP, Instance.targetPort, message);
    public static void MessageSendOut(string ip, string port, string message)
    {
      if (string.IsNullOrEmpty(message)) return;
      byte[] bytes = SendMessageEncoding.GetBytes(message);
      MessageSend(ip, int.Parse(port), bytes);
      Debug.Log($"<color=white>[TTT UDPC]</color> Send [<color=white>{message}</color>] to <color=white>{ip}:{port}</color>");
    }

    public static void MessageSendOut(byte[] message) => MessageSendOut(Instance.targetIP, Instance.targetPort, message);
    public static void MessageSendOut(string ip, string port, byte[] message)
    {
      if (message == null || message.Length == 0) return;
      MessageSend(ip, int.Parse(port), message);
      Debug.Log($"<color=white>[TTT UDPC]</color> Send [Bytes:{message.Length}] to <color=white>{ip}:{port}</color>");
    }
  }
}
