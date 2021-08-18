/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using System.Net.NetworkInformation;
using System;

namespace ToneTuneToolkit.Verification
{
  /// <summary>
  /// 反向验证工具
  /// </summary>
  public class AntiVerifier : MonoBehaviour
  {
    private GameObject dtGO;
    private TextMesh dtTMC;

    private void Start()
    {
      this.PreloadDebugInfo();
      this.AntiVerifikadoSystem();
    }

    /// <summary>
    /// 预创建Debug文字
    /// </summary>
    private void PreloadDebugInfo()
    {
      this.dtGO = new GameObject("Debug Text");
      this.dtGO.transform.position = Vector3.zero;
      this.dtGO.AddComponent<TextMesh>();

      this.dtGO.GetComponent<MeshRenderer>().enabled = true; // 关闭检测文字

      this.dtTMC = dtGO.GetComponent<TextMesh>();
      this.dtTMC.characterSize = .25f;
      this.dtTMC.fontSize = 24;
      this.dtTMC.anchor = TextAnchor.MiddleCenter;
      this.dtTMC.alignment = TextAlignment.Left;
      this.dtTMC.text = "> AntiVerifying...";
      return;
    }

    private void AntiVerifikadoSystem()
    {
      this.dtTMC.text += "\n> UC: <color=#FF0000>" + SystemInfo.deviceUniqueIdentifier + "</color>"; // uc
      NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
      for (int i = 0; i < nis.Length; i++)
      {
        if (nis[i].NetworkInterfaceType.ToString() == "Ethernet")
        {
          this.dtTMC.text += "\n> MC: <color=#FF0000>" + nis[i].GetPhysicalAddress().ToString() + "</color>"; // mc
        }
      }
      this.dtTMC.text += "\n> MC: <color=#FF0000>" + ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000).ToString() + "</color>"; // ts
      this.dtTMC.text += "\n> Done.";
      return;
    }
  }
}