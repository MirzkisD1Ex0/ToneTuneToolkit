/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.IO.CamFi
{
  public class LiveViewer : SingletonMaster<LiveViewer>
  {
    [SerializeField] private Texture2D t2dLiveView;
    private TcpClient tcpClient;

    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    private void Init()
    {
      tcpClient = new TcpClient();
      t2dLiveView = new Texture2D(2, 2);
    }

    private void UnInit()
    {
      Disconnect();
    }

    // ==================================================

    public void SwitchLiveView(bool isOn)
    {
      if (isOn)
      {
        Debug.Log("[CamFi LVM] 开始接收串流");
        Debug.Log(Configer.CamFiLiveViewAddress);

        try
        {
          tcpClient.Connect(new IPEndPoint(
            IPAddress.Parse(Configer.CamFiIP),
            int.Parse(Configer.CamFiLiveViewPort)));
          Debug.Log("[CamFi LVM] TCP已连接...[OK]");
        }
        catch (SocketException e)
        {
          Debug.LogWarning($"[CamFi LVM] 连接失败: {e.Message}");
        }
        catch (Exception e)
        {
          Debug.LogWarning($"[CamFi LVM] 串流启动异常: {e.Message}");
        }
      }
      else
      {
        Disconnect();
      }
    }

    // ==================================================

    private void Disconnect()
    {
      if (tcpClient != null)
      {
        if (tcpClient.Connected)
        {
          tcpClient.Close();
        }
        tcpClient.Dispose();
        tcpClient = null;
        Debug.Log("[CamFi LVM] TCP已断开");
      }
    }
  }
}
