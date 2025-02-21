/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Media
{
  /// <summary>
  /// 截图大师Lite
  /// </summary>
  public class ScreenshotMasterLite : SingletonMaster<ScreenshotMasterLite>
  {
    // private void Update()
    // {
    //   if (Input.GetKeyDown(KeyCode.Q))
    //   {
    //     SaveTest();
    //   }
    // }

    // public RectTransform Area;//用来取景的ui，设置为透明的

    // public void SaveTest()
    // {
    //   string fullPath = $"{Application.streamingAssetsPath}/IMAGE/{SpawnTimeStamp()}.png";
    //   TakeScreenshot(Area, fullPath, CanvasType.ScreenSpaceOverlay);
    // }

    // ==================================================

    /// <summary>
    /// 传入用于标定范围的Image
    /// 独立功能
    /// </summary>
    /// <param name="screenshotArea">标定范围</param>
    /// <param name="fullFilePath">保存路径</param>
    public void TakeScreenshot(RectTransform screenshotArea, string fullFilePath, CanvasType canvasType)
    {
      StartCoroutine(TakeScreenshotAction(screenshotArea, fullFilePath, canvasType));
      return;
    }
    private IEnumerator TakeScreenshotAction(RectTransform screenshotArea, string fullFilePath, CanvasType canvasType)
    {
      yield return new WaitForEndOfFrame(); // 等待渲染帧结束

      int width = (int)screenshotArea.rect.width;
      int height = (int)screenshotArea.rect.height;

      Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA64, false);

      // 原点
      float leftBottomX = 0;
      float leftBottomY = 0;

      switch (canvasType)
      {
        default: break;
        case CanvasType.ScreenSpaceOverlay:
          leftBottomX = screenshotArea.transform.position.x + screenshotArea.rect.xMin;
          leftBottomY = screenshotArea.transform.position.y + screenshotArea.rect.yMin;
          break;
        case CanvasType.ScreenSpaceCamera: // 如果是camera需要额外加上偏移值
          leftBottomX = Screen.width / 2;
          leftBottomY = Screen.height / 2;
          Debug.Log(Screen.width / 2 + "/" + Screen.height / 2);
          break;
      }

      texture2D.ReadPixels(new Rect(leftBottomX, leftBottomY, width, height), 0, 0);
      texture2D.Apply();

      // 保存至本地
      byte[] bytes = texture2D.EncodeToPNG();
      File.WriteAllBytes(fullFilePath, bytes);
      Debug.Log($"[ScreenshotMasterLite] <color=green>{fullFilePath}</color>...[OK]");
      // Destroy(texture2D);
      yield break;
    }

    public static string SpawnTimeStamp()
    {
      return $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
    }

    public enum CanvasType
    {
      ScreenSpaceOverlay = 0,
      ScreenSpaceCamera = 1,
      WorldSpace = 2
    }

    // DateTime dateTime = DateTime.Now;
    // string fullPath = $"{Application.streamingAssetsPath}/{dateTime.Year}-{dateTime.Month}-{dateTime.Day}-{dateTime.Hour}-{dateTime.Minute}-{dateTime.Second}.png";
  }
}
