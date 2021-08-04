/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// 文字加载工具
  ///
  /// 后续会增加覆写功能
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
        TipTools.Error("[TextLoader] Cant find [" + url + "].");
        return null;
      }
      string[] tempStringArray = File.ReadAllLines(url);
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
    /// <param name="fileName">路径</param>
    /// <param name="keyName">字段名</param>
    public static string GetJson(string url, string keyName)
    {
      if (!File.Exists(url))
      {
        TipTools.Error("[TextLoader] Cant find [" + url + "].");
        return null;
      }
      string json = File.ReadAllText(url);
      Dictionary<string, string> keys = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
      if (!keys.ContainsKey(keyName))
      {
        return null;
      }
      return keys[keyName];
    }
  }
}