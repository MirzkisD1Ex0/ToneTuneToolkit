/// <summary>
/// Copyright (c) 2024 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.2
/// </summary>

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Newtonsoft.Json;

namespace ToneTuneToolkit.UDP
{
  /// <summary>
  /// UDP通讯器轻量版 // 客户端
  /// 收发端口即用即删 // 次次不一样
  /// 测试前务必关闭所有防火墙 // 设备之间需要互相ping通
  /// 广播不安全udpClient.EnableBroadcast = true;
  /// </summary>
  public class UDPCommunicatorLite : MonoBehaviour
  {
    public static UDPCommunicatorLite Instance;

    #region Path
    private string udpConfigPath = $"{Application.streamingAssetsPath}/configs/udpconfig.json";
    #endregion

    #region Config
    private string localIP = null;
    private int localPort = 0;
    private string targetIP = null;
    private int targetPort = 0;
    private float reciveFrequency = .5f; // 循环检测间隔
    private Encoding ReciveMessageEncoding = Encoding.ASCII; // 接收消息字符编码
    private Encoding SendMessageEncoding = Encoding.ASCII; // 发出消息字符编码
    #endregion

    #region Receive
    private UdpClient receiveUDPClient; // UDP客户端
    private Thread receiveThread = null; // 单开线程
    private IPEndPoint remoteAddress; // 收
    #endregion

    #region Values
    private string udpMessage; // 接受到的消息
    private event UnityAction<string> OnMessageRecive;
    #endregion

    // ==================================================

    private void Awake() => Instance = this;
    private void Start() => Init();
    private void Update() => ShortcutKey();
    private void OnDestroy() => Uninit();

    // ==================================================

    public void Init()
    {
      LoadConfig();
      remoteAddress = new IPEndPoint(IPAddress.Any, 0);
      receiveThread = new Thread(new ThreadStart(MessageReceive))
      {
        IsBackground = true
      }; // 单开线程接收消息
      receiveThread.Start();
      StartCoroutine(nameof(RepeatHookMessage));
      // InvokeRepeating(nameof(RepeatHookMessage), 0f, reciveFrequency); // 每隔一段时间检测一次是否有消息传入
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
      if (receiveUDPClient != null)
      {
        receiveUDPClient.Close();
      }
      return;
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    private void LoadConfig()
    {
      string json = File.ReadAllText(udpConfigPath, Encoding.UTF8);
      Dictionary<string, string> keys = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

      localIP = keys["local_ip"];
      localPort = int.Parse(keys["local_port"]);
      targetIP = keys["target_ip"];
      targetPort = int.Parse(keys["target_port"]);
      reciveFrequency = float.Parse(keys["recive_frequency"]);
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
    /// 重复钩出回执消息
    /// </summary>
    private IEnumerator RepeatHookMessage()
    {
      while (true)
      {
        yield return new WaitForSeconds(reciveFrequency);

        if (string.IsNullOrEmpty(udpMessage)) // 如果消息为空
        {
          continue;
        }

        Debug.Log($"<color=white>[TTT UDPCommunicatorLite]</color> Recived message: <color=white>[{udpMessage}]</color> form <color=white>[{remoteAddress}]</color>...[OK]");
        if (OnMessageRecive != null) // 如果有订阅
        {
          OnMessageRecive(udpMessage); // 把数据丢出去
        }
        udpMessage = null; // 清空接收结果
      }
    }

    /// <summary>
    /// 接收消息
    /// 独立线程
    /// </summary>
    private void MessageReceive()
    {
      while (true)
      {
        receiveUDPClient = new UdpClient(localPort); // 新建客户端
        byte[] receiveData = receiveUDPClient.Receive(ref remoteAddress);
        udpMessage = ReciveMessageEncoding.GetString(receiveData);
        receiveUDPClient.Close(); // 关闭客户端
      }
    }

    /// <summary>
    /// 发送消息
    /// 为何不将远程端点提出,因为可能需要用此方法1对多发消息
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="message"></param>
    public void MessageSend(string ip, int port, string message)
    {
      if (message == null)
      {
        return;
      }

      byte[] sendData = SendMessageEncoding.GetBytes(message);

      IPEndPoint tempRemoteAddress = new IPEndPoint(IPAddress.Parse(ip), port); // 实例化一个远程端点

      UdpClient sendClient = new UdpClient(); // localPort + 1 // 端口不可复用 // 否则无法区分每条消息 // 接收端消息粘连
      sendClient.Send(sendData, sendData.Length, tempRemoteAddress); // 将数据发送到远程端点
      sendClient.Close(); // 关闭连接
      Debug.Log($"<color=white>[TTT UDPCommunicatorLite]</color> Send [<color=white>{message}</color> to <color=white>{targetIP}:{targetPort}</color>]...[OK]");
      return;
    }

    /// <summary>
    /// 向预设地址发消息
    /// 偷懒方法
    /// </summary>
    /// <param name="message"></param>
    public void SendMessageOut(string message)
    {
      MessageSend(targetIP, targetPort, message);
      return;
    }

    // ==================================================

    private void ShortcutKey()
    {
      // if (Input.GetKeyDown(KeyCode.Q))
      // {
      //   SendMessageOut("sdasd");
      // }
      return;
    }
  }
}