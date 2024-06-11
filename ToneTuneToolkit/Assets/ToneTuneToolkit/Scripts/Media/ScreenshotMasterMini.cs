/// <summary>
/// Copyright (c) 2024 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

namespace ToneTuneToolkit.Media
{
  /// <summary>
  /// 截图大师Mini
  /// </summary>
  public class ScreenshotMasterMini : MonoBehaviour
  {
    public static ScreenshotMasterMini Instance;

    public static UnityAction<Texture2D> OnScreenshotCompelete;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    // ==================================================

    /// <summary>
    /// 传入用于标定范围的Image
    /// 独立功能
    /// </summary>
    /// <param name="screenshotArea">标定范围</param>
    /// <param name="fullFilePath">保存路径</param>
    public void TakeScreenshot(RectTransform screenshotArea, string fullFilePath)
    {
      StartCoroutine(TakeScreenshotAction(screenshotArea, fullFilePath));
      return;
    }

    // 新建overlayui确定截图范围
    private IEnumerator TakeScreenshotAction(RectTransform screenshotArea, string fullFilePath)
    {
      yield return new WaitForEndOfFrame(); // 等待渲染帧结束

      int width = (int)screenshotArea.rect.width;
      int height = (int)screenshotArea.rect.height;

      Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);

      // 原点
      float leftBottomX = screenshotArea.transform.position.x + screenshotArea.rect.xMin;
      float leftBottomY = screenshotArea.transform.position.y + screenshotArea.rect.yMin;

      texture2D.ReadPixels(new Rect(leftBottomX, leftBottomY, width, height), 0, 0);
      texture2D.Apply();

      // 保存至本地
      byte[] bytes = texture2D.EncodeToPNG();
      File.WriteAllBytes(fullFilePath, bytes);
      Debug.Log($"[ScreenshotMasterLite] <color=green>{fullFilePath}</color>...<color=green>[OK]</color>");

      if (OnScreenshotCompelete != null)
      {
        OnScreenshotCompelete(texture2D);
      }
      yield break;
    }
  }
}