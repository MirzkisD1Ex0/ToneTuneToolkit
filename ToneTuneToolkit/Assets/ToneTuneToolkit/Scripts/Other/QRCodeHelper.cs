/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using ToneTuneToolkit.Common;
using ZXing;

namespace ToneTuneToolkit.Other
{
  public class QRCodeHelper : MonoBehaviour
  {
    public static QRCodeHelper Instance;

    private void Awake()
    {
      Instance = this;
    }

    public void GetQRContent(string url)
    {
      StartCoroutine(GetQRPicture(url));
      return;
    }

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
        TipTools.Error(webRequest.error);
        yield break;
      }
      Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

      string result = DecodeQRCode(texture);
      TipTools.Notice($"[QRCodeHelper] Result is [{result}], from [{url}].");
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
        TipTools.Error("[QRCodeHelper] Decode failed.");
        return null;
      }
      return result.Text;
    }
  }
}