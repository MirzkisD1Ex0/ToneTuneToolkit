using UnityEngine;

namespace ToneTuneToolkit
{
  /// <summary>
  /// OK
  /// UDP工具助手
  /// 需要正确的配置文件
  /// TTTUDPCommunicator.Instance.SendMessageOut("Text");
  /// </summary>
  [RequireComponent(typeof(TTTManager))]
  [RequireComponent(typeof(TTTUDPCommunicator))]
  public class TTTUDPHandler : MonoBehaviour
  {
    #region Paths
    public static string UDPConfigPath = TTTManager.ConfigsPath + "/udpconfig.json";
    #endregion

    #region KeyNames
    public static string LocalIPName = "Local IP";
    public static string LocalPortName = "Local Port";
    public static string TargetIPName = "Target IP";
    public static string TargetPortName = "Target Port";
    public static string DetectSpacingName = "Detect Spacing";
    #endregion

    private void Awake()
    {
      TTTManager.FileIntegrityCheck(UDPConfigPath);
    }
  }
}