using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using BestHTTP;
using BestHTTP.SocketIO;

namespace MartellGroupPhoto
{
  /// <summary>
  /// CamFiSocket控制模块
  /// 记得退订事件
  /// </summary>
  public class CamFiSocketManager : MonoBehaviour
  {
    public static CamFiSocketManager Instance;

    private SocketManager socketManager;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      Init();
    }

    private void OnApplicationQuit()
    {
      UnInit();
    }

    // ==================================================

    private void Init()
    {
      socketManager = new SocketManager(new Uri(CamFiStorage.CamFiEventAddress));
      socketManager.Socket.On("connect", ConnectedCallback);
      socketManager.Socket.On("disconnect", DisconnectedCallBack);
      socketManager.Socket.On("camera_add", CameraAddCallback);
      socketManager.Socket.On("camera_remove", CameraRemoveCallback);
      socketManager.Socket.On("file_added", FileAddedCallback);
      socketManager.Open();
      return;
    }

    private void UnInit()
    {
      // if (socketManager != null)
      // {
      //   socketManager.Close();
      // }
      return;
    }

    // ==================================================

    public event UnityAction OnConnected;
    private void ConnectedCallback(Socket socket, Packet packet, params object[] args)
    {
      Debug.Log("[CamFiSocketManager]SokectIO已建立连接...[Done]");
      if (OnConnected != null)
      {
        OnConnected();
      }
      return;
    }

    private void DisconnectedCallBack(Socket socket, Packet packet, params object[] args)
    {
      Debug.Log("[CamFiSocketManager]SokectIO已停止连接...[Done]");
      return;
    }

    private void CameraAddCallback(Socket socket, Packet packet, params object[] args)
    {
      Debug.Log("[CamFiSocketManager]CamFi已连接相机...[Done]");
      return;
    }

    private void CameraRemoveCallback(Socket socket, Packet packet, params object[] args)
    {
      Debug.Log("[CamFiSocketManager]CamFi已停止连接相机...[Done]");
      return;
    }

    public event UnityAction<string> OnFileAdded;
    private void FileAddedCallback(Socket socket, Packet packet, params object[] args)
    {
      Debug.Log("[CamFiSocketManager]新照片路径已接收...[Done]");
      if (OnFileAdded != null)
      {
        OnFileAdded(args[0].ToString());
      }
      return;
    }
  }
}