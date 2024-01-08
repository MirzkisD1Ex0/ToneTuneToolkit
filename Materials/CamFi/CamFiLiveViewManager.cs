using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Net.Sockets;

namespace MartellGroupPhoto
{
  public class CamFiLiveViewManager : MonoBehaviour
  {
    public static CamFiLiveViewManager Instance;

    public Texture2D CameraTexture;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      Init();
    }

    private void Update()
    {
      if (tcpClient.Connected)
      {
        Debug.Log("TCP已连接");
        // tcpClient.
        //   Stream serverstream = tcpClient.GetStream();

        //   if (serverstream.CanRead)
        //   {
        //     Debug.Log($"串流可读,尺寸为:{tcpClient.ReceiveBufferSize}");


        //     // using (MemoryStream writer = new MemoryStream())
        //     // {
        //     //   byte[] readBuffer = new byte[tcpClient.ReceiveBufferSize];


        //     //   // while (serverstream)

        //     // }

        //     // using (FileStream fs = new FileStream())

        //     byte[] readBuffer = new byte[tcpClient.ReceiveBufferSize];
        //     CameraTexture.LoadImage(readBuffer);

        //   }
      }

      if (Input.GetKeyDown(KeyCode.T))
      {

      }
    }

    // ==================================================

    public TcpClient tcpClient;

    private void Init()
    {
      tcpClient = new TcpClient();
      // tcpListener = new TcpListener(new IPEndPoint(IPAddress.Parse(CamFiStorage.CamFiIP), int.Parse(CamFiStorage.CamFiLiveViewPort)));
      CameraTexture = new Texture2D(2880, 1920);

      // socketManager = new SocketManager(new Uri( CamFiStorage.CamFiLiveViewAddress)); // DEBUG // https://(x) ws://(√)

      return;
    }

    public void StartLiveView()
    {
      Debug.Log("开始接收串流");
      Debug.Log(CamFiStorage.CamFiLiveViewAddress);
      tcpClient.Connect(new IPEndPoint(IPAddress.Parse(CamFiStorage.CamFiIP), int.Parse(CamFiStorage.CamFiLiveViewPort)));
      return;
    }
  }
}