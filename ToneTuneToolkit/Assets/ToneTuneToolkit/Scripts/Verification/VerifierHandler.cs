/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Verification
{
  /// <summary>
  /// 验证系统助手
  /// 需要正确的配置文件
  /// </summary>
  [RequireComponent(typeof(ToolkitManager))]
  [RequireComponent(typeof(Verifier))]
  public class VerifierHandler : MonoBehaviour
  {
    #region Paths
    public static string AuthorizationFilePath = ToolkitManager.DataPath + "/authorizationcode.json";
    #endregion

    #region KeyNames
    public static string UCName = "U C";
    public static string MCName = "M C";
    public static string TSName = "T S";
    #endregion

    // ==================================================

    private void Awake()
    {
      PathChecker.FileIntegrityCheck(AuthorizationFilePath);
    }
  }
}
