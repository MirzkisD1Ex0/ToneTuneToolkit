/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.WOL
{
  /// <summary>
  /// 开机小助手
  /// 并不是
  /// </summary>
  [RequireComponent(typeof(ToolkitManager))]
  [RequireComponent(typeof(WakeOnLan))]
  public class WakeOnLanHandler : MonoBehaviour
  {
    #region Paths
    public static string WOLAppPath = ToolkitManager.AdditionalToolsPath + "WolCmd/";
    public static string WOLConfigPath = ToolkitManager.ConfigsPath + "wolconfig.json";
    #endregion

    #region KeyNames
    public static string TargetMACName = "Target MAC";
    public static string TargetIPName = "Target IP";
    public static string TargetMaskName = "Target Mask";
    public static string TargetPortName = "Target Port";
    #endregion

    private void Awake()
    {
      PathChecker.FolderIntegrityCheck(WOLAppPath);
      PathChecker.FileIntegrityCheck(WOLConfigPath);
    }
  }
}