using UnityEngine;
using System.Net.NetworkInformation;
using System;

namespace ToneTuneToolkit
{
  /// <summary>
  /// OK
  /// 反向验证工具
  /// </summary>
  public class TTTAntiVerifier : MonoBehaviour
  {

    #region DEBUG
    private GameObject dtGO;
    private TextMesh dtTMCmpt;
    #endregion
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
      dtGO = new GameObject("DebugText");
      dtGO.transform.position = Vector3.zero;
      dtGO.AddComponent<TextMesh>();

      dtGO.GetComponent<MeshRenderer>().enabled = true; // 关闭检测文字

      dtTMCmpt = dtGO.GetComponent<TextMesh>();
      dtTMCmpt.characterSize = .25f;
      dtTMCmpt.fontSize = 24;
      dtTMCmpt.anchor = TextAnchor.MiddleCenter;
      dtTMCmpt.alignment = TextAlignment.Left;
      dtTMCmpt.text = "> AntiVerifying...";
      return;
    }

    private void AntiVerifikadoSystem()
    {
      dtTMCmpt.text += "\n> UC: <color=#FF0000>" + SystemInfo.deviceUniqueIdentifier + "</color>"; // uc

      NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
      for (int i = 0; i < nis.Length; i++)
      {
        if (nis[i].NetworkInterfaceType.ToString() == "Ethernet")
        {
          dtTMCmpt.text += "\n> MC: <color=#FF0000>" + nis[i].GetPhysicalAddress().ToString() + "</color>"; // mc
        }
      }

      dtTMCmpt.text += "\n> MC: <color=#FF0000>" + ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000).ToString() + "</color>"; // ts

      dtTMCmpt.text += "\n> Done.";
      return;
    }
  }
}