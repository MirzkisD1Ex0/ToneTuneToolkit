/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System;
using UnityEngine;
using UnityEngine.Events;
using Best.SocketIO;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.IO.CamFi
{
  /// <summary>
  /// 指令接收方
  /// </summary>
  public class SocketListener : SingletonMaster<SocketListener>
  {
    private SocketManager socketManager;

    public static UnityAction OnCamFiConnected;
    public static UnityAction OnCamFiDisconnected;
    public static UnityAction<string> OnCamFiFileAdded;

    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    private void Init()
    {
      socketManager = new SocketManager(new Uri(Configer.CamFiEventAddress));

      socketManager.Socket.On(SocketIOEventTypes.Connect, ConnectedCallback);
      socketManager.Socket.On(SocketIOEventTypes.Disconnect, DisconnectedCallback);
      socketManager.Socket.On<string>("camera_add", CameraAddCallback);
      socketManager.Socket.On<string>("camera_remove", CameraRemoveCallback);
      socketManager.Socket.On<string>("file_added", FileAddedCallback);

      socketManager.Open();
    }

    private void UnInit()
    {
      if (socketManager != null) { socketManager.Close(); }
    }

    // ==================================================
    #region Callback

    private void ConnectedCallback()
    {
      Debug.Log("<color=green>[CamFi Manager]</color> SocketIO已建立连接...[OK]");
      OnCamFiConnected?.Invoke();
    }

    private void DisconnectedCallback()
    {
      Debug.LogWarning("[CamFi Manager] SocketIO已停止连接");
      OnCamFiDisconnected?.Invoke();
    }

    private void CameraAddCallback(string cameraInfo)
    {
      Debug.Log($"[CamFi Manager] CamFi已连接相机，附加参数: {cameraInfo}");
    }

    private void CameraRemoveCallback(string cameraInfo)
    {
      Debug.Log($"[CamFi Manager] CamFi已停止连接相机，附加参数: {cameraInfo}");
    }

    private void FileAddedCallback(string filePath)
    {
      Debug.Log($"[CamFi Manager] 新照片路径已接收: {filePath}");
      OnCamFiFileAdded?.Invoke(filePath);
    }

    #endregion
  }
}
