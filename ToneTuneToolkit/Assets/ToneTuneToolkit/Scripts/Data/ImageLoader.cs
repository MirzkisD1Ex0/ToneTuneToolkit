/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>


using UnityEngine;
using NativeFileBrowser;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ToneTuneToolkit.Data
{
  public class ImageLoader : MonoBehaviour
  {
    /// <summary>
    /// 弹窗获取图片路径
    /// </summary>
    /// <returns>图片路径</returns>
    public static string GetImagePath()
    {
      string title = "Select Image";
      ExtensionFilter[] extensions = new ExtensionFilter[]
      {
      new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
      new ExtensionFilter("JPG ", "jpg", "jpeg"),
      new ExtensionFilter("PNG ", "png"),
      };

      // 标题、类型筛选器、是否允许选择多个文件
      List<string> paths = StandaloneFileBrowser.OpenFilePanel(title, extensions, false).ToList();

      if (paths.Count == 0)
      {
        return null;
      }
      // Debug.Log(paths[0]);
      return paths[0];
    }



    /// <summary>
    /// 获取图片
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Texture2D GetImageTexture(string path)
    {
      if (!File.Exists(path))
      {
        return null;
      }

      byte[] bytes = File.ReadAllBytes(path);
      Texture2D texture2D = new Texture2D(2, 2);
      texture2D.LoadImage(bytes);
      texture2D.Apply();
      return texture2D;
    }
  }
}
