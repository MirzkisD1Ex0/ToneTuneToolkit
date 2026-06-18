/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using ToneTuneToolkit.Common;
using UnityEngine.Events;
using System;

namespace ToneTuneToolkit.Networking.Upload
{
  /// <summary>
  /// 对志城宝具
  /// </summary>
  public class Upload2ZCManager : SingletonMaster<Upload2ZCManager>
  {
    #region 2026.03 LYNKCO

    public static UnityAction<Texture2D> OnLYNKCOUploadFinished;
    public static UnityAction<Texture2D> OnLYNKCOFinalUploadFinished;

    private const string LYNKCOUPLOADURL = @"https://linkco-ai.studiocapsule.cn/api/device/submitTask";
    private const string LYNKCOQUERYURL = @"https://linkco-ai.studiocapsule.cn/api/device/queryTask";
    private const string LYNKCOFINALUPLOADURL = @"https://linkco-ai.studiocapsule.cn/api/device/finalUpload";


    [Header("上传")][SerializeField] private LYNKCOUserInfo lcUserInfo;
    [Header("上传回执")][SerializeField] private LYNKCOUploadRespon lcUploadRespon;
    [Header("轮询回执")][SerializeField] private LYNKCOQueryRespon lcQueryRespon;
    [Header("最终上传回执")][SerializeField] private LYNCOFinalUploadRespon lcFinalRespon;


    public void UpdateLYNKCOUserInfo(string code, Texture2D t2d)
    {
      lcUserInfo = new LYNKCOUserInfo();
      lcUserInfo.prompt_code = code;
      lcUserInfo.file = t2d.EncodeToPNG();
    }

    // 上传图片
    public void UploadLCUserInfo() => StartCoroutine(UploadLCUserInfoAction());
    private IEnumerator UploadLCUserInfoAction()
    {
      Debug.Log("[U2ZCM] 开始上传");
      WWWForm wwwForm = new WWWForm();
      wwwForm.AddField("prompt_code", lcUserInfo.prompt_code);
      wwwForm.AddBinaryData("file", lcUserInfo.file);

      using (UnityWebRequest www = UnityWebRequest.Post(LYNKCOUPLOADURL, wwwForm))
      {
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        { Debug.LogWarning($"[U2ZCM] {www.error}"); yield break; }



        Debug.Log($"[U2ZCM] 上传回调:{www.downloadHandler.text}");
        // lcUploadRespon = JsonConvert.DeserializeObject<LYNKCOUploadRespon>(www.downloadHandler.text);
        lcUploadRespon = JsonUtility.FromJson<LYNKCOUploadRespon>(www.downloadHandler.text);

        if (lcUploadRespon.code != 0)
        { Debug.LogWarning($"[U2ZCM] Code错误"); yield break; }

        try
        { StartCoroutine(QueryLCTask()); }
        catch (Exception)
        { Debug.LogWarning($"[U2ZCM] 解析错误"); yield break; }
      }
    }

    // 查询 // 直到图片处理完成
    private IEnumerator QueryLCTask()
    {
      WWWForm wwwForm = new WWWForm();
      wwwForm.AddField("task_code", lcUploadRespon.data.task_code);
      string fullURL = @$"{LYNKCOQUERYURL}?task_code={UnityWebRequest.EscapeURL(lcUploadRespon.data.task_code)}";
      int index = 0;

      while (true)
      {
        Debug.Log(@$"[U2ZCM] 第{++index}次查询");

        using (UnityWebRequest www = UnityWebRequest.Get(fullURL))
        {
          www.downloadHandler = new DownloadHandlerBuffer();
          yield return www.SendWebRequest();
          if (www.result != UnityWebRequest.Result.Success)
          { Debug.LogWarning($"[U2ZCM] {www.error}"); yield break; }



          Debug.Log($"[U2ZCM] 查询回执:{www.downloadHandler.text}");

          // lcQueryRespon = JsonConvert.DeserializeObject<LYNKCOQueryRespon>(www.downloadHandler.text);
          lcQueryRespon = JsonUtility.FromJson<LYNKCOQueryRespon>(www.downloadHandler.text);
          if (lcQueryRespon.code != 0)
          {
            Debug.LogWarning($"[U2ZCM] Code错误");
            // if (lcRespon.code == 4) { Reupload(); }
            yield break;
          }



          if (lcQueryRespon.data.status != 3) // 没完成就再查询
          { yield return new WaitForSeconds(4f); continue; }

          if (lcQueryRespon.data.thumb_url != null) { StartCoroutine(DownloadLCUserImage()); break; }
        }

      }
    }

    private void Reupload()
    {
      StopAllCoroutines();
      UploadLCUserInfo();
    }



    // 下载图片
    private IEnumerator DownloadLCUserImage()
    {
      using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(lcQueryRespon.data.thumb_url))
      {
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        { Debug.LogWarning($"[U2ZCM] {unityWebRequest.error}"); yield break; }
        else
        {
          Debug.Log($"[U2ZCM] 获取图片成功");
          OnLYNKCOUploadFinished?.Invoke(((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture); // 返回图
        }
      }
    }



    public void UploadFinalImage(Texture2D t2d) => StartCoroutine(UploadFinalImageAction(t2d));
    private IEnumerator UploadFinalImageAction(Texture2D t2d)
    {
      Debug.Log("[U2ZCM] 开始上传最终图片");
      WWWForm wwwForm = new WWWForm();
      wwwForm.AddField("log_code", lcUploadRespon.data.log_code);
      wwwForm.AddBinaryData("file", t2d.EncodeToPNG());

      using (UnityWebRequest www = UnityWebRequest.Post(LYNKCOFINALUPLOADURL, wwwForm))
      {
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        { Debug.LogWarning($"[U2ZCM] {www.error}"); yield break; }



        Debug.Log($"[U2ZCM] 最终上传回执: {www.downloadHandler.text}");
        try
        {
          // lcFinalRespon = JsonConvert.DeserializeObject<LYNCOFinalUploadRespon>(www.downloadHandler.text);
          lcFinalRespon = JsonUtility.FromJson<LYNCOFinalUploadRespon>(www.downloadHandler.text);
          if (lcFinalRespon.data.qr_url != null)
          {
            StartCoroutine(DownloadQRCodeAction(lcFinalRespon.data.qr_url));
          }
        }
        catch
        { Debug.LogWarning($"[U2ZCM] 解析错误"); yield break; }
      }
    }

    private IEnumerator DownloadQRCodeAction(string url)
    {
      using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
      {
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        { Debug.Log($"[U2ZCM] {www.error}"); yield break; }

        OnLYNKCOFinalUploadFinished?.Invoke(DownloadHandlerTexture.GetContent(www));
      }
    }

    // ==================================================

    // 上传用户信息
    [Serializable]
    public class LYNKCOUserInfo
    {
      public string prompt_code;
      public byte[] file;
    }



    // 用户信息回执
    [Serializable]
    public class LYNKCOUploadRespon
    {
      public int code;
      public string message;
      public LYNKCOUploadResponData data;
    }
    [Serializable]
    public class LYNKCOUploadResponData
    {
      public string log_code;
      public string task_code;
    }



    // 查询回执
    [Serializable]
    public class LYNKCOQueryRespon
    {
      public int code;
      public string message;
      public LYNKCOQueryResponData data;
    }
    [Serializable]
    public class LYNKCOQueryResponData
    {
      public int status;
      public string status_text;
      public string thumb_url;
    }



    // 最终上传回执
    [Serializable]
    public class LYNCOFinalUploadRespon
    {
      public int code;
      public string message;
      public LYNKCOFinalUploadResponData data;
    }
    [Serializable]
    public class LYNKCOFinalUploadResponData
    {
      public string qr_url;
      public string thumb_url;
    }

    #endregion
    // ==================================================
    // ==================================================
    // ==================================================
    #region 获取QR图片

    // public static UnityAction<Texture2D> OnQRImageDownloaded;

    // [SerializeField] private Texture2D debug_peekQRCode;

    // public void DownloadQRCode(string url) => StartCoroutine(nameof(DownloadQRCodeAction), url);
    // private IEnumerator DownloadQRCodeAction(string url)
    // {
    //   using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
    //   {
    //     yield return www.SendWebRequest();
    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log($"[U2ZCM] {www.error}");
    //       yield break;
    //     }

    //     debug_peekQRCode = DownloadHandlerTexture.GetContent(www); // DEBUG
    //     OnQRImageDownloaded?.Invoke(DownloadHandlerTexture.GetContent(www));
    //   }
    //   yield break;
    // }

    #endregion
  }
}
