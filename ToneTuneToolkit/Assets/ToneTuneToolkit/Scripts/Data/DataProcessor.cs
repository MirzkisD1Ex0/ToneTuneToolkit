/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using UnityEngine;

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
    public static string DoRichTextHighlight(string originString, string highlightString, Color highlightColor)
    {
      string newString = originString;
      for (int i = 0; i < highlightString.Length; i++)
      {
        newString = newString.Replace(highlightString[i].ToString(), $"<color={DataConverter.Color2Hex(highlightColor)}>{highlightString[i]}</color>");
      }
      return newString;
    }
  }
}
