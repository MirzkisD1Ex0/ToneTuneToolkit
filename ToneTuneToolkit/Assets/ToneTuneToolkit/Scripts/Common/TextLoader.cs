/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace ToneTuneToolkit.Common
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

    /// <summary>
    /// 配置文件获取器
    /// json被读取时必须被序列化过
    /// </summary>
    /// <param name="url">路径</param>
    /// <param name="keyName">字段名</param>
    public static string GetJson(string url, string keyName)
    {
      if (!File.Exists(url))
      {
        Debug.Log($"[TextLoader] Cant find [<color=red>{url}</color>]...[Er]");
        return null;
      }
      string json = File.ReadAllText(url, Encoding.UTF8);
      Dictionary<string, string> keys = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
      if (!keys.ContainsKey(keyName))
      {
        return null;
      }
      return keys[keyName];
    }

    /// <summary>
    /// 配置写入
    /// </summary>
    /// <param name="url">文件路径</param>
    /// <param name="keyName">字段名</param>
    /// <param name="content">写入内容</param>
    /// <returns></returns>
    public static bool SetJson(string url, string keyName, string content)
    {
      if (!File.Exists(url))
      {
        Debug.Log($"[TextLoader] Cant find [<color=red>{url}</color>]...[Er]");
        return false;
      }
      string json = File.ReadAllText(url, Encoding.UTF8);
      Dictionary<string, string> keys = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
      keys[keyName] = content;
      json = JsonConvert.SerializeObject(keys);
      File.WriteAllText(url, json, Encoding.UTF8);
      return true;
    }
  }
}