/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
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
    public static UnityAction<Texture2D> OnScreenshotFinished;

    [Header("DEBUG - Peek")]
    [SerializeField] private Texture2D peekTexture;

    // ==================================================

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
    /// <param name="canvasType">截图类型</param>
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

          // leftBottomX = Screen.width / 2;
          // leftBottomY = Screen.height / 2;
          // 相机画幅如果是1920x1080，设置透视、Size540可让UI缩放为111

          // Debug.Log(Screen.width / 2 + "/" + Screen.height / 2);
          break;
      }

      texture2D.ReadPixels(new Rect(leftBottomX, leftBottomY, width, height), 0, 0);
      texture2D.Apply();

      // 保存至本地
      byte[] bytes = texture2D.EncodeToPNG();
      File.WriteAllBytes(fullFilePath, bytes);
      Debug.Log($"[ScreenshotMasterLite] <color=green>{fullFilePath}</color>...[OK]");
      // Destroy(texture2D);

      peekTexture = texture2D;

      if (OnScreenshotFinished != null)
      {
        OnScreenshotFinished(texture2D);
      }
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
    /// <param name="screenshotRT">新建的RT宽高色彩模式都要设置妥当</param>
    public static Texture2D OffScreenshot(Camera screenshotCamera, RenderTexture screenshotRT, string fullFilePath)
    {
      screenshotCamera.clearFlags = CameraClearFlags.SolidColor;
      screenshotCamera.backgroundColor = Color.clear;
      screenshotCamera.targetTexture = screenshotRT;
      screenshotCamera.Render(); // 渲染到纹理

      RenderTexture.active = screenshotRT;
      Texture2D t2d = new Texture2D(screenshotRT.width, screenshotRT.height, TextureFormat.RGBA32, false);
      t2d.ReadPixels(new Rect(0, 0, screenshotRT.width, screenshotRT.height), 0, 0);
      t2d.Apply();

      byte[] bytes = t2d.EncodeToPNG();
      File.WriteAllBytes(fullFilePath, bytes);
      Debug.Log(@$"[SM] <color=green>{fullFilePath}</color>");

      RenderTexture.active = null;
      screenshotRT.Release();
      return t2d;
    }

    #endregion
    // ==================================================
    #region 实验性功能
    public Texture2D InstantTakeScreenshot(Camera renderCamera, string fullFilePath)
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

      // 保存至本地
      byte[] bytes = texture2D.EncodeToPNG();
      File.WriteAllBytes(fullFilePath, bytes);
      Debug.Log($"[ScreenshotMasterLite] <color=green>{fullFilePath}</color>...[OK]");

      peekTexture = texture2D;

      // 清理
      renderCamera.targetTexture = null;
      RenderTexture.active = null;
      Destroy(renderTexture);
      // Destroy(texture2D);
      return texture2D;
    }

    #endregion
    // ==================================================
    #region Tools

    public static string SpawnTimeStamp()
    {
      return $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-{new System.Random().Next(0, 100)}";
    }

    public enum CanvasType
    {
      ScreenSpaceOverlay = 0,
      ScreenSpaceCamera = 1,
      WorldSpace = 2
    }

    #endregion
  }
}