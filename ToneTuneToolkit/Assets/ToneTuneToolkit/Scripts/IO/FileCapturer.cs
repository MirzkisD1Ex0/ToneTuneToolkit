/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>



using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ToneTuneToolkit.IO
{
  public class FileCapturer
  {
    // private string targetFolderPath = @"\\192.168.50.56\Screenshot\"; // 网络路径文件夹需要正确且需要设置无密码共享
    // private const string fileSuffix = "*.jpg";

    /// <summary>
    /// 获取文件完整路径
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="fileSuffix"></param>
    /// <returns></returns>
    public static List<string> GetFileFullPaths2List(string folderPath, string fileSuffix)
    {
      if (!Directory.Exists(folderPath))
      {
        Debug.Log($"[File Capturer] Folder {folderPath} dose not exist");
        return null;
      }

      return Directory.GetFiles(folderPath, fileSuffix).ToList(); // 完整路径
    }

    /// <summary>
    /// 获取文件名
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="fileSuffix"></param>
    /// <returns></returns>
    public static List<string> GetFileNames2List(string folderPath, string fileSuffix)
    {
      if (!Directory.Exists(folderPath))
      {
        Debug.LogError($"[File Capturer] Folder {folderPath} dose not exist");
        return null;
      }

      List<string> filePaths = Directory.GetFiles(folderPath, fileSuffix).ToList(); // 完整路径
      List<string> fileNames = new List<string>(); // 文件名
      foreach (string item in filePaths)
      {
        fileNames.Add(Path.GetFileName(item));
      }

      return fileNames;
    }

    public static byte[] GetFileBytes(string filePath)
    {
      if (!File.Exists(filePath))
      {
        Debug.LogError($"[File Capturer] File {filePath} dose not exist");
        return null;
      }

      return File.ReadAllBytes(filePath);
    }

    #region Old
    // /// <summary>
    // /// 获取路径下全部指定类型的文件名
    // /// </summary>
    // /// <param name="folderPath">路径</param>
    // /// <param name="fileSuffix">后缀名</param>
    // public static List<string> GetFileName2List(string folderPath, string fileSuffix)
    // {
    //   if (!Directory.Exists(folderPath))
    //   {
    //     Debug.Log($"[File Capturer] Path {folderPath} dose not exist");
    //     return null;
    //   }
    //   DirectoryInfo directoryInfo = new DirectoryInfo(folderPath); // 获取文件信息
    //   FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

    //   List<string> filesList = new List<string>();

    //   // 筛选符合条件的文件并储名进数组
    //   for (int i = 0; i < fileInfos.Length; i++)
    //   {
    //     if (fileInfos[i].Name.EndsWith(fileSuffix))
    //     {
    //       filesList.Add(fileInfos[i].Name); // 把符合要求的文件名存储至数组中
    //     }
    //   }
    //   return filesList;
    // }
    #endregion
  }
}
