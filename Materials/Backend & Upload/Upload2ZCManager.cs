using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ToneTuneToolkit.Common;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 对志城法宝
/// </summary>
public class Upload2ZCManager : SingletonMaster<Upload2ZCManager>
{
  private const string uploadURL = @"https://vw-aud.studiocapsule.cn/api/device/uploadWall";

  public UnityAction<string> OnUploadFinished;

  // ==================================================
  #region 上传文件流

  public void UploadData(byte[] fileBytes) => StartCoroutine(nameof(UploadDataAction), fileBytes);
  private IEnumerator UploadDataAction(byte[] fileBytes)
  {
    WWWForm wwwForm = new WWWForm();
    wwwForm.AddBinaryData("file", fileBytes);

    using (UnityWebRequest www = UnityWebRequest.Post(uploadURL, wwwForm))
    {

      // www.SetRequestHeader("Content-Type", "multipart/form-data"); // wwwForm不要手动设置避免boundary消失
      www.downloadHandler = new DownloadHandlerBuffer();
      yield return www.SendWebRequest();

      if (www.result != UnityWebRequest.Result.Success)
      {
        Debug.Log($"[U2ZCM] {www.error}");
        yield break;
      }

      Debug.Log($"[U2ZCM] {www.downloadHandler.text}");
      ResponData responData = JsonConvert.DeserializeObject<ResponData>(www.downloadHandler.text);

      // // 解析方案A 动态类型
      // dynamic data = responData.data;
      // string qr = data.qr_url;

      // 解析方案B
      JObject data = JObject.FromObject(responData.data);
      string qr_url = data["qr_url"].ToString();

      // Debug.Log(qr_url);
      DownloadQRCode(qr_url);
    }
    yield break;
  }

  #endregion
  // ==================================================
  #region 获取QR图片

  public static UnityAction<Texture2D> OnDownloadQRCodeFinished;

  [SerializeField] private Texture2D debug_peekQRCode;

  public void DownloadQRCode(string url) => StartCoroutine(nameof(DownloadQRCodeAction), url);
  private IEnumerator DownloadQRCodeAction(string url)
  {
    using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
    {
      yield return www.SendWebRequest();
      if (www.result != UnityWebRequest.Result.Success)
      {
        Debug.Log($"[U2ZCM] {www.error}");
        yield break;
      }

      debug_peekQRCode = DownloadHandlerTexture.GetContent(www); // DEBUG

      if (OnDownloadQRCodeFinished != null)
      {
        OnDownloadQRCodeFinished(DownloadHandlerTexture.GetContent(www));
      }
    }
    yield break;
  }

  #endregion
  // ==================================================

  public class ResponData
  {
    public int code;
    public string message;
    public object data;
  }
}