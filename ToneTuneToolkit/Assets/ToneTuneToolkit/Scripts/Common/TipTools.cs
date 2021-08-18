/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// ToneTuneToolkit专属提示
  /// </summary>
  public static class TipTools
  {
    /// <summary>
    /// 提示
    /// </summary>
    /// <param name="text"></param>
    public static void Notice(string text)
    {
      Debug.Log(@"<color=#" + ColorUtility.ToHtmlStringRGB(Color.white) + ">[TTT Notice] -> </color>" + text);
      return;
    }

    /// <summary>
    /// 警告
    /// </summary>
    /// <param name="text"></param>
    public static void Warning(string text)
    {
      Debug.Log(@"<color=#" + ColorUtility.ToHtmlStringRGB(Color.yellow) + ">[TTT Warning] -> </color>" + text);
      return;
    }

    /// <summary>
    /// 错误
    /// </summary>
    /// <param name="text"></param>
    public static void Error(string text)
    {
      Debug.Log(@"<color=#" + ColorUtility.ToHtmlStringRGB(Color.red) + ">[TTT Error] -> </color>" + text);
      return;
    }
  }
}