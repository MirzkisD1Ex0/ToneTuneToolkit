/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using ToneTuneToolkit.Data;

namespace ToneTuneToolkit.Verification
{
  /// <summary>
  /// Verifikado
  /// 验证系统
  ///
  /// 需要正确的配置文件
  /// TS ms
  /// http://www.txttool.com/
  /// https://tool.lu/timestamp
  /// </summary>
  public class Verifier : MonoBehaviour
  {
    private string stampURL = "http://api.m.taobao.com/rest/api3.do?api=mtop.common.getTimestamp";
    private string verifikadoCode;
    private string verifikadoMAC;
    private string verifikadoStamp;

    private bool checker = false;

    // 弹窗
    [DllImport("user32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, String message, String title, int type);

    // 网络
    [DllImport("wininet.dll")]
    private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

    #region DEBUG
    private GameObject dtGO;
    private TextMesh dtTMC;
    #endregion

    // ==================================================

    private void Start()
    {
      PreloadDebugInfo();
      VerifikadoSystem();
    }

    // ==================================================

    /// <summary>
    /// 预创建Debug文字
    /// </summary>
    private void PreloadDebugInfo()
    {
      dtGO = new GameObject("DebugText");
      dtGO.transform.position = Vector3.zero;
      dtGO.AddComponent<TextMesh>();

      dtGO.GetComponent<MeshRenderer>().enabled = true; // 关闭检测文字

      dtTMC = dtGO.GetComponent<TextMesh>();
      dtTMC.characterSize = .25f;
      dtTMC.fontSize = 24;
      dtTMC.anchor = TextAnchor.MiddleCenter;
      dtTMC.alignment = TextAlignment.Left;
      dtTMC.text = "> Verifying...";
      return;
    }

    /// <summary>
    /// Verifikado系统
    /// </summary>
    private void VerifikadoSystem()
    {
      checker = CheckFileExist(VerifierHandler.AuthorizationFilePath); // s1 file
      dtTMC.text += "\n> Check the Files Exists: <color=#FF0000>" + checker + "</color>"; // DEBUG
      if (!checker) // 如果为否
      {
        ApplicationError("无效的程序验证流程。");
        return;
      }

      checker = CheckNetwork(); // s2 net
      dtTMC.text += "\n> Check the Network: <color=#FF0000>" + checker + "</color>"; // DEBUG
      if (!checker)
      {
        ApplicationError("无网络链接，请检查后重试。");
        return;
      }

      verifikadoCode = DataConverter.Binary2String(JsonManager.GetJson(VerifierHandler.AuthorizationFilePath, VerifierHandler.UCName));
      checker = CheckUniqueCode(verifikadoCode); // s3 uc
      dtTMC.text += "\n> Check the Code: <color=#FF0000>" + checker + "</color>"; // DEBUG
      if (!checker)
      {
        ApplicationError("无效的授权。");
        return;
      }

      verifikadoMAC = DataConverter.Binary2String(JsonManager.GetJson(VerifierHandler.AuthorizationFilePath, VerifierHandler.MCName));
      checker = CheckMACCode(verifikadoMAC); // s4 mc
      dtTMC.text += "\n> Check the Address: <color=#FF0000>" + checker + "</color>"; // DEBUG
      if (!checker)
      {
        ApplicationError("无效的地址。");
        return;
      }

      verifikadoStamp = DataConverter.Binary2String(JsonManager.GetJson(VerifierHandler.AuthorizationFilePath, VerifierHandler.TSName));
      StartCoroutine(CheckTimeStampChain(stampURL)); // s5 ts
      return;
    }

    #region Check
    private bool CheckFileExist(string filePath)
    {
      if (File.Exists(filePath))
      {
        return true;
      }
      return false;
    }

    private bool CheckNetwork()
    {
      int i = 0;
      if (InternetGetConnectedState(out i, 0)) // C#网络判断
      {
        return true;
      }
      return false;
    }

    private bool CheckUniqueCode(string vC)
    {
      if (vC == SystemInfo.deviceUniqueIdentifier)
      {
        return true;
      }
      return false;
    }

    private bool CheckMACCode(string vM)
    {
      NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces(); // Get全部网卡
      for (int i = 0; i < nis.Length; i++)
      {
        if (nis[i].NetworkInterfaceType.ToString() == "Ethernet") // Get以太网
        {
          if (vM == nis[i].GetPhysicalAddress().ToString()) // Mac地址确认
          {
            return true;
          };
        }
      }
      return false;
    }

    /// <summary>
    /// 时间验证链
    /// </summary>
    /// <param name="stampURL"></param>
    /// <returns></returns>
    private IEnumerator CheckTimeStampChain(string stampURL)
    {
      UnityWebRequest webRequest = UnityWebRequest.Get(stampURL);
      yield return webRequest.SendWebRequest();
      // if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
      // {
      //     // Debug.Log(webRequest.error);
      // }
      JObject jb = JObject.Parse(webRequest.downloadHandler.text);
      long networkStamp = long.Parse(jb["data"]["t"].ToString());
      // Debug.Log(jb["data"]["t"]); // 时间戳

      long localStamp = long.Parse(verifikadoStamp); // 转long
      if (networkStamp > localStamp)
      {
        checker = false;
      }
      else
      {
        checker = true;
      }

      dtTMC.text += "\n> Check the Authorization Date: <color=#FF0000>" + checker + "</color>"; // DEBUG
      if (!checker)
      {
        ApplicationError("授权可能已过期。");
        yield break;
      }

      // 跳转
      dtTMC.text += "\n> Done.";
      LoadNextScene();
      yield break;
    }
    #endregion



    #region Other
    private void ApplicationError(string message)
    {
      MessageBox(IntPtr.Zero, message + "\n您可能是盗版程序的受害者，程序即将退出。\n\n<" + DateTime.Now.ToString() + ">", "Verify - Application Error", 0);
      Invoke("ApplicationQuit", 3f);
      return;
    }

    private void LoadNextScene()
    {
      if (SceneManager.sceneCountInBuildSettings >= 2) // 场景大于2加载下一个场景
      {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
      }
      return;
    }
    #endregion
  }
}
