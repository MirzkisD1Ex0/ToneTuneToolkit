using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Rocworks.Mqtt;

namespace ToneTuneToolkit.MQTT
{
  public class MQTTManager : MonoBehaviour
  {
    public static MQTTManager Instance;

    #region Path
    private string configPath = $"{Application.streamingAssetsPath}/configs/mqttconfig.json";
    #endregion

    public MqttClient MqttClient;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      Init();
    }

    // private void OnApplicationQuit()
    // {
    //   Uninit();
    // }

    // ==================================================

    private void Init()
    {
      // MqttClient.Host = JsonManager.GetJson(configPath, "host");
      // MqttClient.Port = JsonManager.GetJson(configPath, "port");
      return;
    }

    // private void Uninit()
    // {
    //   return;
    // }

    // ==================================================

    public void SetMQTTClientHost(string value)
    {
      MqttClient.Host = value;
      return;
    }

    public void SetMQTTClientPort(int value)
    {
      MqttClient.Port = value;
      return;
    }

    // ==================================================

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    public void SendMessageOut(string topic, string message)
    {
      MqttClient.Connection.Publish(topic, message);
      Debug.Log($"[MQTT Manager] Message [<color=white>{message}</color>] send to [<color=white>{MqttClient.Host}:{MqttClient.Port}</color>].");
      return;
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="value"></param>
    public void OnMessageArrived(MqttMessage value)
    {
      Debug.Log($"[MQTT Manager] Message [<color=white>{value}</color>] received.");
      return;
    }
  }
}