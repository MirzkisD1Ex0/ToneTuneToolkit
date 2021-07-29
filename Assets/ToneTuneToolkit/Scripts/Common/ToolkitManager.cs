using UnityEngine;
using System.IO;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// OK
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

      FolderIntegrityCheck(MainPath);
      FolderIntegrityCheck(ConfigsPath);
      FolderIntegrityCheck(DataPath);
      FolderIntegrityCheck(AdditionalToolsPath);
    }

    /// <summary>
    /// 文件夹完整性检查
    /// </summary>
    /// <param name="url"></param>
    public static bool FolderIntegrityCheck(string url)
    {
      if (File.Exists(url))
      {
        return true;
      }
      Directory.CreateDirectory(url);
      return false;
    }

    /// <summary>
    /// 文件完整性检查
    /// </summary>
    /// <param name="url"></param>
    public static bool FileIntegrityCheck(string url)
    {
      if (File.Exists(url))
      {
        return true;
      }
      FileInfo fi = new FileInfo(url);
      StreamWriter sw = fi.CreateText();
      sw.Close();
      sw.Dispose();
      return false;
    }
  }
}