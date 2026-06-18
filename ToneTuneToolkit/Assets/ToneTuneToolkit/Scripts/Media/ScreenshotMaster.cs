/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
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
      // 1. 等待渲染完成，确保能抓取到当前帧内容
      yield return new WaitForEndOfFrame();

      // 2. 计算 RectTransform 在屏幕上的四个角（世界坐标）
      Vector3[] corners = new Vector3[4];
      screenshotArea.GetWorldCorners(corners);

      // corners[0] 是左下角，corners[2] 是右上角
      Vector2 screenLeftBottom;
      Vector2 screenRightTop;

      // 根据 Canvas 类型确定转换方式
      if (canvasType == CanvasType.ScreenSpaceOverlay)
      {
        // Overlay 模式下世界坐标直接对应屏幕像素
        screenLeftBottom = corners[0];
        screenRightTop = corners[2];
      }
      else
      {
        // Camera 模式下需要将世界坐标转换为屏幕空间坐标
        // 假设您的 UI 对应的相机是 Camera.main，或者您可以传入特定的 UI 相机
        Camera uiCamera = screenshotArea.GetComponentInParent<Canvas>().worldCamera;
        if (uiCamera == null) uiCamera = Camera.main;

        screenLeftBottom = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[0]);
        screenRightTop = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[2]);
      }

      // 3. 计算实际像素宽度和高度
      int width = Mathf.RoundToInt(screenRightTop.x - screenLeftBottom.x);
      int height = Mathf.RoundToInt(screenRightTop.y - screenLeftBottom.y);

      // 4. 读取像素
      Texture2D t2d = new Texture2D(width, height, TextureFormat.RGBA32, false);

      // ReadPixels 的 Rect 参数：x, y 是左下角起点
      t2d.ReadPixels(new Rect(screenLeftBottom.x, screenLeftBottom.y, width, height), 0, 0);
      t2d.Apply();

      if (fullFilePath != null) { SaveTexture2File(t2d, fullFilePath); }

      peekTexture = t2d;
      OnScreenshotFinished?.Invoke(t2d, flag);
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

      if (fullFilePath != null) { SaveTexture2File(t2d, fullFilePath); }

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
      Texture2D t2d = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA64, false); // TextureFormat.RGB24

      // 从RenderTexture中读取像素数据
      RenderTexture.active = renderTexture;
      t2d.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
      t2d.Apply();

      if (fullFilePath != null) { SaveTexture2File(t2d, fullFilePath); }

      peekTexture = t2d;

      // 清理
      renderCamera.targetTexture = null;
      RenderTexture.active = null;
      Destroy(renderTexture);
      // Destroy(texture2D);
      return t2d;
    }

    public static Texture2D TakeScreenshot2T2d(Camera screenshotCamera, RectTransform screenshotArea, string fullFilePath = null)
    {
      Vector2 size = Vector2.Scale(screenshotArea.rect.size, screenshotArea.lossyScale);
      RenderTexture rt = new RenderTexture((int)size.x, (int)size.y, 24);

      screenshotCamera.targetTexture = rt;
      screenshotCamera.Render();

      Texture2D t2d = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
      Graphics.CopyTexture(rt, t2d);

      screenshotCamera.targetTexture = null;

      if (fullFilePath != null) { SaveTexture2File(t2d, fullFilePath); }

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

    public static string SpawnTimeStamp() => $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-{new System.Random().Next(0, 100)}";

    public static void SaveTexture2File(Texture2D t2d, string fullFilePath)
    {
      byte[] bytes = t2d.EncodeToPNG();
      File.WriteAllBytes(fullFilePath, bytes);
      Debug.Log($"[SM] <color=green>{fullFilePath}</color>");
    }

    public static Texture2D LoadTexture4Path(string path)
    {
      if (!File.Exists(path))
      {
        Debug.LogError($"[SM] Cant find <color=red>{path}</color>");
        return null;
      }
      byte[] t2dBytes = File.ReadAllBytes(path);
      Texture2D t2d = new Texture2D(2, 2, TextureFormat.RGBA32, false);
      t2d.LoadImage(t2dBytes);
      t2d.Apply();
      return t2d;
    }

    #endregion
  }
}
