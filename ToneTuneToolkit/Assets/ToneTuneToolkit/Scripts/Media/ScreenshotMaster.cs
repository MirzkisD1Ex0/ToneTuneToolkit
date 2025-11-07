/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Media
{
  /// <summary>
  /// 截图大师
  /// </summary>
  public class ScreenshotMaster : SingletonMaster<ScreenshotMaster>
  {
    public static UnityAction<Texture2D, int> OnScreenshotFinished;

    [Header("DEBUG - Peek")]
    [SerializeField] private Texture2D peekTexture;

    // ==================================================

    /// <summary>
    /// 传入用于标定范围的Image
    /// 独立功能
    /// </summary>
    /// <param name="screenshotArea">标定范围</param>
    /// <param name="fullFilePath">保存路径</param>
    /// <param name="canvasType">截图类型</param>
    public void TakeScreenshot(RectTransform screenshotArea, CanvasType canvasType, int flag = 0, string fullFilePath = null) => StartCoroutine(TakeScreenshotAction(screenshotArea, canvasType, flag, fullFilePath));
    private IEnumerator TakeScreenshotAction(RectTransform screenshotArea, CanvasType canvasType, int flag = 0, string fullFilePath = null)
    {
      yield return new WaitForEndOfFrame(); // 等待渲染帧结束

      int width = (int)screenshotArea.rect.width;
      int height = (int)screenshotArea.rect.height;

      Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);

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
        case CanvasType.ScreenSpaceCamera: // 如果是camera需要额外加上偏移值 // 相机画幅如果是1920x1080，设置透视、Size=540可让UI缩放为111
          leftBottomX = screenshotArea.transform.position.x + (Screen.width / 2 + screenshotArea.rect.xMin);
          leftBottomY = screenshotArea.transform.position.y + (Screen.height / 2 + screenshotArea.rect.yMin);
          break;
      }

      texture2D.ReadPixels(new Rect(leftBottomX, leftBottomY, width, height), 0, 0);
      texture2D.Apply();

      if (fullFilePath != null)
      {
        // 保存至本地
        byte[] bytes = texture2D.EncodeToPNG();
        File.WriteAllBytes(fullFilePath, bytes);
        Debug.Log($"[SM] <color=green>{fullFilePath}</color>");
        // Destroy(texture2D);
      }

      peekTexture = texture2D;

      OnScreenshotFinished?.Invoke(texture2D, flag);
      yield break;
    }

    // ==================================================
    #region RenderTexture - 屏外渲染

    /// <summary>
    /// 屏外RT截图
    /// Camera Size = Canvas高度的1/2
    /// Canvas Render mode = Screen Space - Camera
    /// </summary>
    /// <param name="screenshotCamera"></param>
    /// <param name="screenshotRT">新建的RT宽高色彩模式都要设置妥当 // RGBA8_SRGB</param>
    public static Texture2D OffScreenshot(Camera screenshotCamera, RenderTexture screenshotRT, RotateType rt = RotateType.RotateNone, string fullFilePath = null)
    {
      screenshotCamera.clearFlags = CameraClearFlags.SolidColor;
      screenshotCamera.backgroundColor = Color.clear;
      screenshotCamera.targetTexture = screenshotRT;
      screenshotCamera.Render(); // 渲染到纹理

      RenderTexture.active = screenshotRT;
      Texture2D t2d = new Texture2D(screenshotRT.width, screenshotRT.height, TextureFormat.RGBA32, false);
      t2d.ReadPixels(new Rect(0, 0, screenshotRT.width, screenshotRT.height), 0, 0);

      if (rt != RotateType.RotateNone) { t2d = RotateTexture(t2d, rt); } // 可能存在的旋转

      t2d.Apply();

      if (fullFilePath != null)
      {
        byte[] bytes = t2d.EncodeToPNG();
        File.WriteAllBytes(fullFilePath, bytes);
        Debug.Log(@$"[SM] <color=green>{fullFilePath}</color>");
      }

      RenderTexture.active = null;
      screenshotRT.Release();
      return t2d;
    }

    #endregion
    // ==================================================
    #region T2D旋转

    public static Texture2D RotateTexture(Texture2D original, RotateType rotation)
    {
      if (rotation == RotateType.RotateNone) { return original; }

      int width = original.width;
      int height = original.height;

      // 确定新纹理尺寸
      bool is90or270 = rotation == RotateType.Rotate90 || rotation == RotateType.Rotate270;
      Texture2D result = new Texture2D(is90or270 ? height : width, is90or270 ? width : height);

      Color32[] originalPixels = original.GetPixels32();
      Color32[] rotatedPixels = new Color32[originalPixels.Length];

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          int newX, newY;

          switch (rotation)
          {
            case RotateType.Rotate90:
              newX = height - 1 - y;
              newY = x;
              break;
            case RotateType.Rotate180:
              newX = width - 1 - x;
              newY = height - 1 - y;
              break;
            case RotateType.Rotate270:
              newX = y;
              newY = width - 1 - x;
              break;
            case RotateType.RotateNone:
            default:
              newX = x;
              newY = y;
              break;
          }

          rotatedPixels[newY * result.width + newX] = originalPixels[y * width + x];
        }
      }

      result.SetPixels32(rotatedPixels);
      result.Apply();
      return result;
    }

    public enum RotateType
    {
      RotateNone = 0,
      Rotate90 = 90,
      Rotate180 = 180,
      Rotate270 = 270
    }

    #endregion
    // ==================================================
    #region 实验性功能
    public Texture2D InstantTakeScreenshot(Camera renderCamera, string fullFilePath = null)
    {
      // 创建一个RenderTexture
      RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
      renderCamera.targetTexture = renderTexture;

      // 手动渲染Camera
      renderCamera.Render();

      // 创建一个Texture2D来保存渲染结果
      Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA64, false); // TextureFormat.RGB24

      // 从RenderTexture中读取像素数据
      RenderTexture.active = renderTexture;
      texture2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
      texture2D.Apply();

      if (fullFilePath != null)
      {
        // 保存至本地
        byte[] bytes = texture2D.EncodeToPNG();
        File.WriteAllBytes(fullFilePath, bytes);
        Debug.Log($"[SM] <color=green>{fullFilePath}</color>");
      }

      peekTexture = texture2D;

      // 清理
      renderCamera.targetTexture = null;
      RenderTexture.active = null;
      Destroy(renderTexture);
      // Destroy(texture2D);
      return texture2D;
    }

    public Texture2D TakeScreenshot2T2d(Camera screenshotCamera, RectTransform screenshotArea)
    {
      Vector2 size = Vector2.Scale(screenshotArea.rect.size, screenshotArea.lossyScale);
      RenderTexture rt = new RenderTexture((int)size.x, (int)size.y, 24);

      screenshotCamera.targetTexture = rt;
      screenshotCamera.Render();

      Texture2D t2d = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
      Graphics.CopyTexture(rt, t2d);

      screenshotCamera.targetTexture = null;

      return t2d;
    }

    #endregion
    // ==================================================
    #region Tools

    public enum CanvasType
    {
      ScreenSpaceOverlay = 0,
      ScreenSpaceCamera = 1,
      WorldSpace = 2
    }

    public static string SpawnTimeStamp()
    {
      return $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-{new System.Random().Next(0, 100)}";
    }

    public static void SaveTexture2File(Texture2D t2d, string fullFilePath)
    {
      byte[] bytes = t2d.EncodeToPNG();
      File.WriteAllBytes(fullFilePath, bytes);
      Debug.Log($"[SM] <color=green>{fullFilePath}</color>");
    }

    #endregion
  }
}
