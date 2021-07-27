using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.WOL
{
  /// <summary>
  /// 开机小助手
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
      ToolkitManager.FolderIntegrityCheck(WOLAppPath);
      ToolkitManager.FileIntegrityCheck(WOLConfigPath);
    }
  }
}