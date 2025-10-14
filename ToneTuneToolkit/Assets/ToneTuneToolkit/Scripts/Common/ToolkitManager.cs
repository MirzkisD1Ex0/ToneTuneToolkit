/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// MANAGER!
  /// </summary>
  public class ToolkitManager : MonoBehaviour
  {
    #region Paths
    private static string MainPath = Application.streamingAssetsPath + "/ToneTuneToolkit/";
    public static string ConfigsPath = MainPath + "configs/";
    public static string DataPath = MainPath + "data/";
    public static string AdditionalToolsPath = MainPath + "additionaltools/";
    #endregion

    private void Awake()
    {
      DontDestroyOnLoad(gameObject);

      PathChecker.FolderIntegrityCheck(MainPath);
      PathChecker.FolderIntegrityCheck(ConfigsPath);
      PathChecker.FolderIntegrityCheck(DataPath);
      PathChecker.FolderIntegrityCheck(AdditionalToolsPath);
    }
  }
}
