/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using ToneTuneToolkit.Common;
using ZXing;
using ZXing.Common;
using UnityEngine.UIElements;

namespace ToneTuneToolkit.Other
{
  /// <summary>
  /// 扫码助手
  /// </summary>
  public class QRCodeMaster : MonoBehaviour
  {
    public static QRCodeMaster Instance;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    // ==================================================

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
        Debug.Log(webRequest.error);
        yield break;
      }
      Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

      string result = DecodeQRCode(texture);
      Debug.Log($"[QRCodeHelper] Result is [{result}], from [{url}].");
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
        Debug.Log("[QRCodeHelper] Decode failed.");
        return null;
      }
      return result.Text;
    }

    /// <summary>
    /// 形成二维码
    /// </summary>
    /// <param name="qrText"></param>
    /// <param name="qrWidth"></param>
    /// <param name="qrHeight"></param>
    /// <returns></returns>
    public Texture2D GenerateQRCode(string qrText, int qrWidth, int qrHeight)
    {
      BarcodeWriter writer = new BarcodeWriter();
      writer.Format = BarcodeFormat.QR_CODE;
      writer.Options.Width = qrWidth;
      writer.Options.Height = qrHeight;
      writer.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");

      Color32[] colors = writer.Write(qrText);

      Texture2D qrTexture = new Texture2D(qrWidth, qrHeight);
      qrTexture.SetPixels32(colors);
      qrTexture.Apply();

      return qrTexture;
    }
  }
}