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
using System;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Media
{
  public class ScreenshotMaster : SingletonMaster<ScreenshotMaster>
  {
    [SerializeField]
    public int _textureHight = 1024, _textureWidth = 1024;

    public Camera ShootingCamera;
    public RawImage PreviewImage;

    private RenderTexture _renderTexture;


    private void Awake()
    {
      _renderTexture = InitRenderTexture();
      SettingCamera(ShootingCamera);

      if (PreviewImage)
      {
        PreviewImage.texture = _renderTexture;
      }
    }

    /// <summary>
    /// 初始化RT
    /// </summary>
    /// <returns></returns>
    private RenderTexture InitRenderTexture()
    {
      RenderTexture _tempRenderTexture = new RenderTexture(_textureWidth, _textureHight, 16);
      _tempRenderTexture.name = "TempRenderTexutre";
      _tempRenderTexture.dimension = TextureDimension.Tex2D;
      _tempRenderTexture.antiAliasing = 1;
      _tempRenderTexture.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
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
      png = null;
      Debug.Log("保存成功！" + filePath);
      return;
    }
  }
}