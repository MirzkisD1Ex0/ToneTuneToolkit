/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.IO;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// 获取某个目录下指定类型的文件名
  /// </summary>
  public class FileNameCapturer
  {
    /// <summary>
    /// 获取路径下全部指定类型的文件名
    ///
    /// string[] dd = Directory.GetFiles(url, "*.jpg");
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="suffix">后缀名</param>
    /// <param name="files">用以存储文件名的数组</param>
    public static string[] GetFileName(string path, string suffix)
    {
      if (!Directory.Exists(path)) // 如果路径不存在 // 返回 空
      {
        TipTools.Error("[FileNameCapturer] Cant find [" + path + "].");
        return null;
      }
      DirectoryInfo directoryInfo = new DirectoryInfo(path); // 获取文件信息
      FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

      // 统计有多少符合条件的文件
      int arraySize = 0;
      for (int i = 0; i < fileInfos.Length; i++)
      {
        if (fileInfos[i].Name.EndsWith(suffix))
        {
          arraySize++;
          continue;
        }
      }

      string[] filesArray = new string[arraySize]; // 新建数组

      // 筛选符合条件的文件并储名进数组
      int arrayIndex = 0;
      for (int i = 0; i < fileInfos.Length; i++)
      {
        if (fileInfos[i].Name.EndsWith(suffix))
        {
          filesArray[arrayIndex++] = fileInfos[i].Name; // 把符合要求的文件名存储至数组中
          continue;
        }
      }
      return filesArray;
    }
  }
}