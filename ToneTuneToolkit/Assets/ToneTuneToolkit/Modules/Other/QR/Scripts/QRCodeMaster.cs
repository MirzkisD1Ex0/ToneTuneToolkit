/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using ToneTuneToolkit.Common;
using UnityEngine;
using UnityEngine.Networking;
using ZXing;

namespace ToneTuneToolkit.Other.QR
{
  /// <summary>
  /// 扫码助手
  /// </summary>
  public class QRCodeMaster : SingletonMaster<QRCodeMaster>
  {
    public void GetQRContent(string url) => StartCoroutine(GetQRPicture(url));

    /// <summary>
    /// 获取图片
    /// </summary>
    /// <param name="url">地址</param>
    /// <returns></returns>
    private IEnumerator GetQRPicture(string url)
    {
      //  string[] paths = Directory.GetFiles(Application.streamingAssetsPath + "/", "*.jpg"); // 获取所有贴图
      UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
      yield return webRequest.SendWebRequest();

      if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
      {
        Debug.Log(webRequest.error);
        yield break;
      }
      Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

      string result = DecodeQRCode(texture);
      Debug.Log($"[QRCodeHelper] Result is [{result}], from [{url}]...[OK]");
      yield break;
    }


    /// <summary>
    /// 二维码解码
    /// </summary>
    /// <param name="texture">QR码图片</param>
    /// <returns>返回结果或空</returns>
    public static string DecodeQRCode(Texture2D texture)
    {
      BarcodeReader barcodeReader = new BarcodeReader();
      barcodeReader.AutoRotate = true;
      barcodeReader.TryInverted = true;
      Result result = barcodeReader.Decode(texture.GetPixels32(), texture.width, texture.height);
      if (result == null)
      {
        Debug.Log("[QRCodeHelper] Decode failed...[Er]");
        return null;
      }
      return result.Text;
    }

    /// <summary>
    /// 形成二维码
    /// </summary>
    /// <param name="qrText"></param>
    /// <param name="qrSize"></param>
    /// <param name="qrHeight"></param>
    /// <returns></returns>
    public static Texture2D GenerateQRCode(string qrText, int qrSize = 256, Color qrColor = default, Color bgColor = default)
    {
      BarcodeWriter writer = new BarcodeWriter();
      writer.Format = BarcodeFormat.QR_CODE;
      writer.Options.Width = qrSize;
      writer.Options.Height = qrSize;
      writer.Options.Margin = 1;
      writer.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");

      Color32[] colors = writer.Write(qrText);
      Texture2D qrTexture = new Texture2D(qrSize, qrSize, TextureFormat.RGBA32, false);

      if (qrColor == default) { qrColor = Color.white; } // 二维码默认白色
      if (bgColor == default) { bgColor = Color.clear; } // 背景默认透明

      for (int i = 0; i < colors.Length; i++) // 遍历像素数据，将二维码模块设置为黑色，背景设置为透明
      {
        if (colors[i].r == 0 && colors[i].g == 0 && colors[i].b == 0) // 黑色模块
        {
          colors[i] = qrColor;
        }
        else // 背景
        {
          colors[i] = bgColor;
        }
      }

      qrTexture.SetPixels32(colors);
      qrTexture.Apply();

      Debug.Log("[QRM] QR Code generate successd.");
      return qrTexture;
    }
  }
}
