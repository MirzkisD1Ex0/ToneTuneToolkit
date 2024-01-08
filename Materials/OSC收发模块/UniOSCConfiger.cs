using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.Data;

public class UniOSCConfiger : MonoBehaviour
{
  private string configPath;

  private void Start()
  {
    Init();
  }

  // ==================================================

  private void Init()
  {
    configPath = $"{Application.streamingAssetsPath}/oscconfig.json";

    UniOSCManager.Instance.UpdateInIPAddress(JsonManager.GetJson(configPath, "Local IP"), JsonManager.GetJson(configPath, "Local Port"));
    UniOSCManager.Instance.UpdateOutIPAddress(JsonManager.GetJson(configPath, "Target IP"), JsonManager.GetJson(configPath, "Target Port"));
    return;
  }

}