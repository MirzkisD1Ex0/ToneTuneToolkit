/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace ToneTuneToolkit.Data
{
  /// <summary>
  /// 数据加工
  /// </summary>
  public class DataProcessor
  {
    /// <summary>
    /// 使部分字体高亮
    /// </summary>
    /// <param name="originString"></param>
    /// <param name="highlightString"></param>
    /// <param name="highlightColor"></param>
    /// <returns></returns>
    public static string DoRichTextHighlight(string originString, string highlightSting, Color highlightColor)
    {
      string newString = null;

      // // 方案A // 强匹配
      // newString = originString;
      // newString = newString.Replace(highlightString.ToString(), $"<color={DataConverter.Color2Hex(highlightColor)}>{highlightString}</color>");

      // 方案B // 全字符匹配
      newString = new Regex(@$"{string.Join('|', highlightSting.ToCharArray())}")
         .Replace(originString, m => $"<color={DataConverter.Color2Hex(highlightColor)}>{m}</color>");

      return newString;
    }
  }
}
