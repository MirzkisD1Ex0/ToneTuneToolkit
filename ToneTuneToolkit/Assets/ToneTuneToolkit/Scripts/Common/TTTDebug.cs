/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// ToneTuneToolkit专属提示
  /// </summary>
  public static class TTTDebug
  {
    /// <summary>
    /// 提示
    /// </summary>
    /// <param name="message"></param>
    public static void Log(object message)
    {

      Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(Color.white)}>[{nameof(ToneTuneToolkit)}] -> </color>{message}");
      return;
    }

    /// <summary>
    /// 警告
    /// </summary>
    /// <param name="message"></param>
    public static void LogWarning(string message)
    {
      Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGB(Color.yellow)}>[TTT Warning] -> </color>{message}");
      return;
    }

    /// <summary>
    /// 错误
    /// </summary>
    /// <param name="message"></param>
    public static void LogError(string message)
    {
      Debug.LogError($"<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>[TTT Error] -> </color>{message}");
      return;
    }
  }
}
