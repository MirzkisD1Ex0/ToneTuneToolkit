using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Best.SocketIO;
using Best.HTTP.JSON;
using Best.HTTP.JSON.LitJson;
using ToneTuneToolkit.Data;

public class SocketIOManager : MonoBehaviour
{
  public static SocketIOManager Instance;

  #region 路径
  private string socketioConfigPath = $"{Application.streamingAssetsPath}/configs/socketioconfig.json";
  #endregion

  #region 配置
  private string targetIP;
  private string targetPort;
  #endregion

  private SocketManager manager;

  // ==================================================

  private void Awake() => Instance = this;
  private void Start() => Init();
  private void OnDestroy() => Uninit();

  // ==================================================

  private void Init()
  {
    targetIP = JsonManager.GetJson(socketioConfigPath, "target_ip");
    targetPort = JsonManager.GetJson(socketioConfigPath, "target_port");

    manager = new SocketManager(
      new Uri($"https://{targetIP}"),
      // new Uri($"https://{targetIP}:{targetPort}"),
      new SocketOptions
      {
        // AdditionalQueryParams = new ObservableDictionary<string, string> { { "type", "machine" } }, // 识别用途
        AutoConnect = false
      });

    manager.Socket.On(SocketIOEventTypes.Connect, OnConnected);
    manager.Socket.On<BrandStartData>(@"brand-start", ReceiveData);
    SwitchSocketIO(true);
    return;
  }

  private void Uninit()
  {
    SwitchSocketIO(false);
    return;
  }

  // ==================================================
  // 绑定事件

  private void OnConnected()
  {
    Debug.Log("[SocketIO Manager] Connected!");
    return;
  }

  private void ReceiveData(BrandStartData value)
  {
    GameManager.Instance.SetInData(value);
    // JsonData jd = JsonMapper.ToObject(receive);
    // Debug.Log(jd["video_code"]);
    // MessageProcessor.Instance.SendMessageOut(jd["video_code"].ToString());
    return;
  }

  // ==================================================

  /// <summary>
  /// 发送消息
  /// </summary>
  /// <param name="eventName"></param>
  /// <param name="message"></param>
  public void MessageSend(string eventName, object message)
  {
    manager.Socket.Emit(eventName, message);
    Debug.Log($"<color=white>[Socket IO]</color> Message [<color=white>{message}</color>].");
    return;
  }

  // ==================================================

  /// <summary>
  /// 端口开关
  /// </summary>
  /// <param name="value"></param>
  public void SwitchSocketIO(bool value)
  {
    if (value)
    {
      manager.Open();
    }
    else
    {
      manager.Close();
    }
    return;
  }

  // ==================================================

  // 数据类
  [Serializable]
  public class BrandStartData
  {
    public string uuid;
    public string clock;
    public string start_code;
  }

  [Serializable]
  public class BrandStopData
  {
    public string clock;
    public string start_code;
  }
}