/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.UDP
{
  /// <summary>
  /// UDP通讯器
  ///
  /// 需要助手
  /// 测试前务必关闭所有防火墙
  /// 设备间需要ping通
  /// </summary>
  public class UDPCommunicator : MonoBehaviour
  {
    public static UDPCommunicator Instance;



    #region Settings
    private byte[] localIP = new byte[] { 0, 0, 0, 0 };
    private int localPort = 0;
    private byte[] targetIP = new byte[] { 0, 0, 0, 0 };
    private int targetPort = 0;

    private float detectSpacing = 1f; // 循环检测间隔
    private Encoding ReciveMessageEncoding = Encoding.UTF8; // 接收消息字符编码
    private Encoding SendMessageEncoding = Encoding.ASCII; // 发出消息字符编码
    #endregion



    #region Others
    public static string UDPMessage; // 接受到的消息
    private UdpClient udpClient; // UDP客户端
    private Thread thread = null; // 单开线程
    private IPEndPoint remoteAddress;
    #endregion

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      this.LoadConfig();
      this.Presetting();
    }

    private void OnDestroy()
    {
      this.SocketQuit();
    }

    private void OnApplicationQuit()
    {
      this.SocketQuit();
    }

    /// <summary>
    /// 加载地址
    /// </summary>
    private void LoadConfig()
    {
      string[] localIPString = TextLoader.GetJson(UDPHandler.UDPConfigPath, UDPHandler.LocalIPName).Split('.');
      for (int i = 0; i < 4; i++)
      {
        this.localIP[i] = (byte)int.Parse(localIPString[i]);
      }
      this.localPort = int.Parse(TextLoader.GetJson(UDPHandler.UDPConfigPath, UDPHandler.LocalPortName));

      string[] targetIPString = TextLoader.GetJson(UDPHandler.UDPConfigPath, UDPHandler.TargetIPName).Split('.');
      for (int i = 0; i < 4; i++)
      {
        this.targetIP[i] = (byte)int.Parse(targetIPString[i]);
      }
      this.targetPort = int.Parse(TextLoader.GetJson(UDPHandler.UDPConfigPath, UDPHandler.TargetPortName));
      this.detectSpacing = float.Parse(TextLoader.GetJson(UDPHandler.UDPConfigPath, UDPHandler.DetectSpacingName));
      return;
    }

    /// <summary>
    /// 预设置
    /// </summary>
    private void Presetting()
    {
      this.remoteAddress = new IPEndPoint(IPAddress.Any, 0);
      this.thread = new Thread(this.MessageReceive); // 单开线程接收消息
      this.thread.Start();
      InvokeRepeating("RepeatDetect", 0f, this.detectSpacing); // 每隔一段时间检测一次是否有消息传入
      return;
    }

    /// <summary>
    /// 重复检测
    /// </summary>
    private void RepeatDetect()
    {
      if (string.IsNullOrEmpty(UDPMessage)) // 如果消息为空
      {
        return;
      }
      TipTools.Notice(UDPMessage);
      UDPMessage = null; // 清空接收结果
      return;
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    private void MessageReceive()
    {
      while (true)
      {
        this.udpClient = new UdpClient(this.localPort);
        byte[] receiveData = this.udpClient.Receive(ref this.remoteAddress); // 接收数据
        UDPMessage = ReciveMessageEncoding.GetString(receiveData);
        this.udpClient.Close();
      }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="sendMessage"></param>
    public void MessageSend(byte[] ip, int port, string sendMessage)
    {
      if (sendMessage == null)
      {
        return;
      }

      IPAddress tempIPAddress = new IPAddress(ip);
      IPEndPoint tempRemoteAddress = new IPEndPoint(tempIPAddress, port); // 实例化一个远程端点
      byte[] sendData = SendMessageEncoding.GetBytes(sendMessage);
      UdpClient client = new UdpClient(); // this.localPort + 1 // 端口不可复用 // 否则无法区分每条消息
      client.Send(sendData, sendData.Length, tempRemoteAddress); // 将数据发送到远程端点
      client.Close(); // 关闭连接
      return;
    }

    /// <summary>
    /// 退出套接字
    /// </summary>
    private void SocketQuit()
    {
      this.thread.Abort();
      this.thread.Interrupt();
      this.udpClient.Close();
      return;
    }

    /// <summary>
    /// 向固定地址和IP发消息
    /// 偷懒方法
    /// </summary>
    /// <param name="message"></param>
    public void SendMessageOut(string message)
    {
      this.MessageSend(this.targetIP, this.targetPort, message);
      TipTools.Notice("Send <<color=#FFFFFF>" + message + "</color>> to <<color=#FFFFFF>" + this.targetIP[0] + "." + this.targetIP[1] + "." + this.targetIP[2] + "." + this.targetIP[3] + ":" + this.targetPort + "</color>>");
      return;
    }
  }
}