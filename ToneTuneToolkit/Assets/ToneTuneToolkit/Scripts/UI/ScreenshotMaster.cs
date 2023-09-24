/// <summary>
/// Copyright (c) 2023 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace ToneTuneToolkit.UI
{
  /// <summary>
  /// 局部截图大师
  /// </summary>
  public class ScreenshotMaster : MonoBehaviour
  {
    public static ScreenshotMaster Instance;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    // ==================================================

    /// <summary>
    /// 传入用于标定范围的Image
    /// </summary>
    /// <param name="screenshotArea"></param>
    /// <param name="fullFilePath"></param>
    public void TakeScreenshot(RectTransform screenshotArea, string fullFilePath)
    {
      StartCoroutine(TakeScreenshotAction(screenshotArea, fullFilePath));
      return;
    }

    private IEnumerator TakeScreenshotAction(RectTransform screenshotArea, string fullFilePath)
    {
      yield return new WaitForEndOfFrame(); // 等待渲染帧结束
      int width = (int)screenshotArea.rect.width;
      int height = (int)screenshotArea.rect.height;

      Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA64, false);

      // 自定原点
      float leftBottomX = screenshotArea.transform.position.x + screenshotArea.rect.xMin;
      float leftBottomY = screenshotArea.transform.position.y + screenshotArea.rect.yMin;

      texture2D.ReadPixels(new Rect(leftBottomX, leftBottomY, width, height), 0, 0);
      texture2D.Apply();

      // 保存至本地
      byte[] bytes = texture2D.EncodeToPNG();
      File.WriteAllBytes(fullFilePath, bytes);
      yield break;
    }
  }
}