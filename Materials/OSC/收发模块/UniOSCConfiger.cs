using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit.OSC
{
  public class UniOSCConfiger : MonoBehaviour
  {
    private string configPath = $"{Application.streamingAssetsPath}/configs/oscconfig.json";

    // ==================================================

    private void Start()
    {
      Init();
    }

    // ==================================================

    private void Init()
    {
      UniOSCManager.Instance.UpdateInIPAddress(JsonHelper.GetJson(configPath, "local_ip"), JsonHelper.GetJson(configPath, "local_port"));
      UniOSCManager.Instance.UpdateOutIPAddress(JsonHelper.GetJson(configPath, "target_ip"), JsonHelper.GetJson(configPath, "target_port"));
      return;
    }
  }
}