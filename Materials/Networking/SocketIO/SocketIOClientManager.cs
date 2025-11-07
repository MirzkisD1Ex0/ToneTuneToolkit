/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using Best.SocketIO;
using Best.SocketIO.Events;
using ToneTuneToolkit.Common;
using UnityEngine;
using UnityEngine.Events;

namespace ToneTuneToolkit.Networking
{
  /// <summary>
  /// SocketIO通信
  /// </summary>
  public class SocketIOClientManager : SingletonMaster<SocketIOClientManager>
  {
    public static UnityAction<string> OnDeviceStart;

    private const string Address = @"wss://node.skyelook.com"; // 开头为wss且结尾并非/socket.io
                                                               // private const string Address = "ws://192.168.50.130:3500";

    private SocketManager socketManager;

    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    private void Init()
    {
      socketManager = new SocketManager(new Uri(Address));
      socketManager.Options.AutoConnect = false;
      socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
      socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Error, OnError);
      socketManager.Socket.On<string>("michelin-start", OnStart);

      socketManager.Open();
    }

    private void UnInit()
    {
      if (socketManager != null)
      {
        socketManager.Close();
        socketManager = null;
      }
    }

    // ==================================================

    private void OnConnected(ConnectResponse resp)
    {
      Debug.Log("[SocketIOM] Connected.");
    }

    private void OnError(ConnectResponse resp)
    {
      Debug.Log(@$"[SocketIOM] {resp}");
    }

    private void OnStart(string value)
    {
      Debug.Log(@$"[SocketIOM] Start. {value}");
      OnDeviceStart?.Invoke(value);
    }
  }
}