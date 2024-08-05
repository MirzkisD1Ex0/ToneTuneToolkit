using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace ToneTuneToolkit.MQTT
{
  public class MQTTHelper : MonoBehaviour
  {
    public static MQTTHelper Instance;

    private string SolidMessage = "{\"data_type\":\"03\",\"data_content\":{\"msg_id\":\"3ab7d42c-e959-4855-a73e-0675b86f3297\",\"msg_level\":0,\"op_type\":\"02\",\"op_data\":\"\",\"op_target\":[\"E65E\"]},\"timestamp\":1535361775271}";

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    // private void Update()
    // {
    //   if (Input.GetKeyDown(KeyCode.Q))
    //   {
    //     SpeedSendMQTT();
    //   }
    // }

    // ==================================================

    public void SpeedSendMQTT()
    {
      MQTTManager.Instance.SendMessageOut("PREFIX/uwb/message/send/engine_id", SolidMessage.ToString(CultureInfo.InvariantCulture));
      return;
    }
  }
}