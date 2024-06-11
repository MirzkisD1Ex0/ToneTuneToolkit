/// <summary>
/// Copyright (c) 2024 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ToneTuneToolkit.Media
{
  /// <summary>
  /// 图像旋转
  /// https://blog.csdn.net/qq_35030499/article/details/135174016?utm_medium=distribute.pc_relevant.none-task-blog-2~default~baidujs_baidulandingword~default-1-135174016-blog-78890247.235^v43^control&spm=1001.2101.3001.4242.2&utm_relevant_index=4
  /// </summary>
  public class TextureProcessor : MonoBehaviour
  {
    public static TextureProcessor Instance;

    private void Awake()
    {
      Instance = this;
    }

    // ==================================================
    // 读写

    /// <summary>
    /// 读取图片转为t2d
    /// </summary>
    /// <param name="picturePath">图片路径</param>
    /// <returns></returns>
    public static Texture2D ReadTexture(string picturePath)
    {
      Texture2D tempTexture = new Texture2D(2, 2);
      tempTexture.LoadImage(File.ReadAllBytes(picturePath));
      tempTexture.Apply();
      return tempTexture;
    }

    /// <summary>
    /// 写入图片
    /// </summary>
    /// <param name="texture2D">写入的图片</param>
    /// <param name="picturePath">图片路径</param>
    /// <returns></returns>
    public static bool WirteTexture(Texture2D texture2D, string picturePath, PictureFormat pictureFormat)
    {
      byte[] data;
      switch (pictureFormat)
      {
        default:
        case PictureFormat.PNG: data = texture2D.EncodeToPNG(); break;
        case PictureFormat.JPG: data = texture2D.EncodeToJPG(); break;
      }
      File.WriteAllBytes(picturePath, data);
      return true;
    }
    public enum PictureFormat
    {
      PNG,
      JPG,
    }

    // ==================================================
    // 旋转翻转

    /// <summary>
    /// 旋转t2d
    /// </summary>
    /// <param name="originalTexture">原t2d</param>
    /// <param name="clockwise">顺时针/逆时针</param>
    /// <returns></returns>
    public static Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
      Color32[] original = originalTexture.GetPixels32();
      Color32[] rotated = new Color32[original.Length];
      int w = originalTexture.width;
      int h = originalTexture.height;

      int iRotated, iOriginal;

      for (int j = 0; j < h; ++j)
      {
        for (int i = 0; i < w; ++i)
        {
          iRotated = (i + 1) * h - j - 1;
          iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
          rotated[iRotated] = original[iOriginal];
        }
      }

      Texture2D rotatedTexture = new Texture2D(h, w);
      rotatedTexture.SetPixels32(rotated);
      rotatedTexture.Apply();
      return rotatedTexture;
    }

    /// <summary>
    /// 水平翻转t2d
    /// </summary>
    /// <param name="originalTexture"></param>
    /// <returns></returns>
    public static Texture2D HorizontalFlipTexture(Texture2D originalTexture)
    {
      int width = originalTexture.width;
      int height = originalTexture.height;

      Texture2D flipTexture = new Texture2D(width, height);

      for (int i = 0; i < width; i++)
      {
        flipTexture.SetPixels(i, 0, 1, height, originalTexture.GetPixels(width - i - 1, 0, 1, height));
      }
      flipTexture.Apply();
      return flipTexture;
    }

    /// <summary>
    /// 垂直翻转t2d
    /// </summary>
    /// <param name="originalTexture"></param>
    /// <returns></returns>
    public static Texture2D VerticalFlipTexture(Texture2D originalTexture)
    {
      int width = originalTexture.width;
      int height = originalTexture.height;

      Texture2D flipTexture = new Texture2D(width, height);
      for (int i = 0; i < height; i++)
      {
        flipTexture.SetPixels(0, i, width, 1, originalTexture.GetPixels(0, height - i - 1, width, 1));
      }
      flipTexture.Apply();
      return flipTexture;
    }

    /// <summary>
    /// 压缩t2d尺寸
    /// </summary>
    /// <param name="originalTexture"></param>
    /// <param name="targetWidth"></param>
    /// <param name="targetHeight"></param>
    /// <returns></returns>
    public static Texture2D ScaleTexture(Texture2D originalTexture, float targetWidth, float targetHeight)
    {
      Texture2D scaleTexutre2D = new Texture2D((int)targetWidth, (int)targetHeight, originalTexture.format, false);

      for (int i = 0; i < scaleTexutre2D.height; ++i)
      {
        for (int j = 0; j < scaleTexutre2D.width; ++j)
        {
          Color newColor = originalTexture.GetPixelBilinear(j / (float)scaleTexutre2D.width, i / (float)scaleTexutre2D.height);
          scaleTexutre2D.SetPixel(j, i, newColor);
        }
      }

      scaleTexutre2D.Apply();
      return scaleTexutre2D;
    }
  }
}