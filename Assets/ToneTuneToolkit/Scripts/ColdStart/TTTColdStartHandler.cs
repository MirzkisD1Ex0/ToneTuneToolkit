using UnityEngine;

namespace ToneTuneToolkit
{
  /// <summary>
  /// 开机小助手
  /// </summary>
  [RequireComponent(typeof(TTTManager))]
  [RequireComponent(typeof(TTTColdStart))]
  public class TTTColdStartHandler : MonoBehaviour
  {
    #region Paths
    public static string WOLAppPath = TTTManager.AdditionalToolsPath + "WolCmd/";
    public static string WOLConfigPath = TTTManager.ConfigsPath + "wolconfig.json";
    #endregion

    #region KeyNames
    public static string TargetMACName = "Target MAC";
    public static string TargetIPName = "Target IP";
    public static string TargetMaskName = "Target Mask";
    public static string TargetPortName = "Target Port";
    #endregion

    private void Awake()
    {
      TTTManager.FolderIntegrityCheck(WOLAppPath);
      TTTManager.FileIntegrityCheck(WOLConfigPath);
    }
  }
}