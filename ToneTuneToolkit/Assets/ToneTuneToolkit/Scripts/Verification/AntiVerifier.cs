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
      PreloadDebugInfo();
      AntiVerifikadoSystem();
    }

    /// <summary>
    /// 预创建Debug文字
    /// </summary>
    private void PreloadDebugInfo()
    {
      dtGO = new GameObject("Debug Text");
      dtGO.transform.position = Vector3.zero;
      dtGO.AddComponent<TextMesh>();

      dtGO.GetComponent<MeshRenderer>().enabled = true; // 关闭检测文字

      dtTMC = dtGO.GetComponent<TextMesh>();
      dtTMC.characterSize = .25f;
      dtTMC.fontSize = 24;
      dtTMC.anchor = TextAnchor.MiddleCenter;
      dtTMC.alignment = TextAlignment.Left;
      dtTMC.text = "> AntiVerifying...";
      return;
    }

    private void AntiVerifikadoSystem()
    {
      dtTMC.text += "\n> UC: <color=#FF0000>" + SystemInfo.deviceUniqueIdentifier + "</color>"; // uc
      NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
      for (int i = 0; i < nis.Length; i++)
      {
        if (nis[i].NetworkInterfaceType.ToString() == "Ethernet")
        {
          dtTMC.text += "\n> MC: <color=#FF0000>" + nis[i].GetPhysicalAddress().ToString() + "</color>"; // mc
        }
      }
      dtTMC.text += "\n> MC: <color=#FF0000>" + ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000).ToString() + "</color>"; // ts
      dtTMC.text += "\n> Done.";
      return;
    }
  }
}