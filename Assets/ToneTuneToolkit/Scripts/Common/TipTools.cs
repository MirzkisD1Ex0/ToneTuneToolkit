using UnityEngine;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// OK
  /// TTT专属提示
  /// </summary>
  public class TipTools : MonoBehaviour
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