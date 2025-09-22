using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Data;
using UnityEngine;

namespace ToneTuneToolkit.OSC
{
  public class UniOSCConfiger : MonoBehaviour
  {
    private string configPath = $"{Application.streamingAssetsPath}/Configs/oscconfig.json";

    // ==================================================

    private void Start() => Init();

    // ==================================================

    private void Init()
    {
      UniOSCManager.Instance.UpdateInIPAddress(JsonManager.GetJson(configPath, "local_ip"), JsonManager.GetJson(configPath, "local_port"));
      UniOSCManager.Instance.UpdateOutIPAddress(JsonManager.GetJson(configPath, "target_ip"), JsonManager.GetJson(configPath, "target_port"));
      return;
    }
  }
}