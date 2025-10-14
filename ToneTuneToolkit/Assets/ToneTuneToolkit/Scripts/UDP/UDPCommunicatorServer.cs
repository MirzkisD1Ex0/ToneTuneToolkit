/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;

namespace ToneTuneToolkit.UDP
{
  /// <summary>
  /// UDP通讯器轻量版 // 服务端
  /// 单端口play // 发送的消息会粘连/定向接收连续的消息
  /// 测试前务必关闭所有防火墙 // 设备之间需要互相ping通
  /// </summary>
  public class UDPCommunicatorServer : MonoBehaviour
  {
    public static UDPCommunicatorServer Instance;

    #region Path
    private string udpConfigPath = Application.streamingAssetsPath + "/udpconfig.json";
    #endregion

    #region Config
    private string targetIP = null;
    private int targetPort = 0;
    private int localPort = 0;
    private float reciveFrequency = .5f; // 循环检测间隔
    #endregion

    #region Other
    private UdpClient udpClient; // 单端口
    private Thread receiveThread;
    private IPEndPoint remoteClient; // 客户端的IP和端口信息
    #endregion


    #region Value
    private string udpMessage; // 接受到的消息
    private event UnityAction<string> OnMessageRecive;
    #endregion

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      Init();
    }

    private void OnDisable()
    {
      Uninit();
    }

    // ==================================================

    private void Init()
    {
      LoadConfig();

      udpClient = new UdpClient(localPort); // 创建UDP客户端并绑定到指定端口
      Debug.Log($"<color=white>[TTT UDPCommunicatorServer]</color> UDP Server started on port : <color=white>[{localPort}]</color>...[OK]");
      remoteClient = new IPEndPoint(IPAddress.Any, 0); // 初始化客户端端点

      receiveThread = new Thread(new ThreadStart(MessageReceive)) // 创建并启动接收线程
      {
        IsBackground = true
      };
      receiveThread.Start();

      InvokeRepeating("RepeatHookMessage", 0f, reciveFrequency); // 每隔一段时间检测一次是否有消息传入
      return;
    }

    /// <summary>
    /// 卸载
    /// 退出套接字
    /// </summary>
    public void Uninit()
    {
      CancelInvoke("RepeatHookMessage");
      if (receiveThread != null)
      {
        receiveThread.Abort();
      }
      if (udpClient != null)
      {
        udpClient.Close();
      }
      return;
    }

    // ==================================================
    // 接收消息事件订阅

    public void AddEventListener(UnityAction<string> unityAction)
    {
      OnMessageRecive += unityAction;
      return;
    }

    public void RemoveEventListener(UnityAction<string> unityAction)
    {
      OnMessageRecive -= unityAction;
      return;
    }

    // ==================================================

    /// <summary>
    /// 加载配置文件
    /// </summary>
    private void LoadConfig()
    {
      string json = File.ReadAllText(udpConfigPath, Encoding.UTF8);
      Dictionary<string, string> keys = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
      localPort = int.Parse(keys["local_port"]);
      targetIP = keys["target_ip"];
      targetPort = int.Parse(keys["target_port"]);
      reciveFrequency = float.Parse(keys["recive_frequency"]);
      return;
    }

    /// <summary>
    /// 重复钩出回执消息
    /// </summary>
    private void RepeatHookMessage()
    {
      if (string.IsNullOrEmpty(udpMessage)) // 如果消息为空
      {
        return;
      }
      Debug.Log($"<color=white>[TTT UDPCommunicatorServer]</color> Recived message: <color=white>[{udpMessage}]</color>...[OK]");

      if (OnMessageRecive != null) // 如果有订阅
      {
        OnMessageRecive(udpMessage); // 把数据丢出去
      }
      udpMessage = null; // 清空接收结果
      return;
    }

    /// <summary>
    /// 接收消息
    /// 独立线程
    /// </summary>
    private void MessageReceive()
    {
      while (true)
      {
        byte[] result = udpClient.Receive(ref remoteClient);
        udpMessage = Encoding.UTF8.GetString(result);
      }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void MessageSend(string ip, int port, string message)
    {
      if (message == null)
      {
        return;
      }
      byte[] messageBytes = Encoding.UTF8.GetBytes(message);
      IPEndPoint sendEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
      udpClient.Send(messageBytes, messageBytes.Length, sendEndPoint);
      return;
    }

    /// <summary>
    /// 向预设地址发消息
    /// 偷懒方法
    /// </summary>
    /// <param name="message"></param>
    public void MessageSendOut(string message)
    {
      MessageSend(targetIP, targetPort, message);
      Debug.Log($"<color=white>[TTT UDPCommunicatorServer]</color> Lazy send [<color=white>{message}</color> to <color=white>{targetIP}:{targetPort}</color>]...[OK]");
      return;
    }
  }
}
