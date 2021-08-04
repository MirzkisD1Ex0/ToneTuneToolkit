/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.IO;
using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// 文件/文件夹检查器
  /// </summary>
  public class PathChecker : MonoBehaviour
  {
    /// <summary>
    /// 文件夹完整性检查
    /// 不存在则创建空文件夹
    /// </summary>
    /// <param name="url"></param>
    public static bool FolderIntegrityCheck(string url)
    {
      if (File.Exists(url))
      {
        return true;
      }
      Directory.CreateDirectory(url);
      TipTools.Warning("[PathChecker] Folder created.");
      return false;
    }

    /// <summary>
    /// 文件完整性检查
    /// 不存在则创建空的文件
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
      TipTools.Warning("[PathChecker] File created.");
      return false;
    }
  }
}