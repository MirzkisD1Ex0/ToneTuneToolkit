/// <summary>
/// Copyright (c) 2023 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using System.IO;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Media
{
  /// <summary>
  /// 截图大师
  /// </summary>
  public class ScreenshotMaster : SingletonMaster<ScreenshotMaster>
  {
    public Camera ScreenshotCamera;

    [SerializeField]
    private int textureHight = 1024, textureWidth = 1024; // 贴图尺寸

    public RawImage PreviewImage; // 预览用UI
    private RenderTexture _renderTexture;

    // ==================================================

    private void Awake()
    {
      _renderTexture = InitRenderTexture();
      SettingCamera(ScreenshotCamera);

      if (PreviewImage)
      {
        PreviewImage.texture = _renderTexture;
      }
    }

    // ==================================================

    /// <summary>
    /// 初始化RT
    /// </summary>
    /// <returns></returns>
    private RenderTexture InitRenderTexture()
    {
      RenderTexture _tempRenderTexture = new RenderTexture(textureWidth, textureHight, 16)
      {
        name = "TempRenderTexutre",
        dimension = TextureDimension.Tex2D,
        antiAliasing = 1,
        graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat
      };
      return _tempRenderTexture;
    }

    /// <summary>
    /// 设置相机
    /// </summary>
    /// <param name="tempCamera"></param>
    private void SettingCamera(Camera tempCamera)
    {
      tempCamera.backgroundColor = Color.clear;
      tempCamera.targetTexture = _renderTexture;
      return;
    }

    public void SaveRenderTexture(string filePath, string fileName)
    {
      RenderTexture active = RenderTexture.active;
      RenderTexture.active = _renderTexture;
      Texture2D png = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.ARGB32, false);
      png.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
      png.Apply();
      RenderTexture.active = active;
      byte[] bytes = png.EncodeToPNG();
      if (!Directory.Exists(filePath))
      {
        Directory.CreateDirectory(filePath);
      }
      FileStream fs = File.Open(filePath + fileName, FileMode.Create);
      BinaryWriter writer = new BinaryWriter(fs);
      writer.Write(bytes);
      writer.Flush();
      writer.Close();
      fs.Close();
      Destroy(png);
      _renderTexture.Release();
      Debug.Log($"[ScreenshotMaster] <color=green>{filePath}{fileName}</color>...[OK]");
      return;
    }

    // ==================================================

    /// <summary>
    /// 传入用于标定范围的Image
    /// 独立功能
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
      Debug.Log($"[ScreenshotMaster] <color=green>{fullFilePath}</color>...[OK]");
      yield break;
    }
  }
}