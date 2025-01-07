/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using System.IO;
using System.Text;
using UnityEngine;

namespace ToneTuneToolkit.Data
{
  /// <summary>
  /// 文字加载工具
  ///
  /// Get
  /// </summary>
  public static class TextLoader
  {
    /// <summary>
    /// 读取文本内容
    /// </summary>
    /// <param name="url">文件路径</param>
    /// <param name="line">要读取的文件行数</param>
    /// <returns></returns>
    public static string GetText(string url, int line)
    {
      if (!File.Exists(url))
      {
        Debug.Log($"[TextLoader] Cant find [<color=red>{url}</color>]...[Er]");
        return null;
      }
      string[] tempStringArray = File.ReadAllLines(url, Encoding.UTF8);
      if (line > 0)
      {
        return tempStringArray[line - 1]; // .Split('=')[1]; // 等号分隔 // 读取第二部分
      }
      else
      {
        return null;
      }
    }
  }
}
