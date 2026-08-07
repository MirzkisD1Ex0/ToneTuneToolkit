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
    #region Value

    public static UnityAction<Texture2D> OnGeneratedImageDownloaded;
    public static UnityAction<Texture2D> OnQRImageDownloaded;
    public static UnityAction OnUploadFailed; // 任意环节失败时触发，供 UI 层解除等待状态；细分错误码以后再扩展
    public static UnityAction OnQueryFinished;

    private const int MAXQUERYCOUNT = 30;
    private const float QUERYSPACETIME = 3f;



    private const string UPLOADURL = @"https://google-io-2026.studiocapsule.cn/api/index/upload";
    private const string QUERYURL = @"https://google-io-2026.studiocapsule.cn//api/index/search";

    [Header("Upload Payload")][SerializeField] private UploadPayload uploadPayload;
    [Header("Upload Callback")][SerializeField] private UploadCallbackPayload uploadCallbackPayload;
    [Header("Query Callback")][SerializeField] private QueryCallbackPayload queryCallbackPayload;

    #endregion
    // ==================================================
    #region Data Class

    [Serializable]
    public class UploadPayload
    {
      public int is_push;
      public int theme;
      [HideInInspector] public byte[] file;

      public WWWForm ToWWWForm()
      {
        WWWForm form = new WWWForm();
        form.AddField("is_push", is_push);
        form.AddField("theme", theme);
        form.AddBinaryData("file", file);
        return form;
      }
    }



    [Serializable]
    public class UploadCallbackPayload
    {
      public int code;
      public string message;
      public UploadCallbackPayloadData data;
    }
    [Serializable]
    public class UploadCallbackPayloadData
    {
      public string file_code;
      public string web_qr;
      public GeminiPayload gemini;
    }
    [Serializable]
    public class GeminiPayload
    {
      public int status;
      public string status_text;
      public string image_url;
    }



    [Serializable]
    public class QueryCallbackPayload
    {
      public int code;
      public string message;
      public QueryCallbackPayloadData data;
    }
    [Serializable]
    public class QueryCallbackPayloadData
    {
      public int file_code;
      public string web_qr;
      public GeminiPayload gemini;
    }

    #endregion
    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    public void Reset()
    {
      uploadPayload = null;
      uploadCallbackPayload = null;
      queryCallbackPayload = null;
      StopAllCoroutines();
    }

    private void Init()
    {
      OnUploadFailed += Reupload;
    }

    private void UnInit()
    {
      OnUploadFailed -= Reupload;
    }

    // ==================================================
    #region Main Function

    public void SetUploadPayload(int is_push, int theme, Texture2D t2d)
    {
      uploadPayload = new UploadPayload
      {
        is_push = is_push,
        theme = theme,
        file = t2d.EncodeToPNG()
      };
    }

    private void Reupload()
    {
      StopAllCoroutines();
      StartUpload();
    }

    // ==================================================

    /// <summary>
    /// 上传表单
    /// </summary>
    public void StartUpload() => StartCoroutine(UploadFormCoroutine());
    private IEnumerator UploadFormCoroutine()
    {
      yield break;
      Debug.Log("[U2ZCM] Begining upload.");

      using (UnityWebRequest www = UnityWebRequest.Post(UPLOADURL, uploadPayload.ToWWWForm()))
      {
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
          Debug.LogWarning($"[U2ZCM] {www.error}");
          OnUploadFailed?.Invoke();
          yield break;
        }

        Debug.Log($"[U2ZCM] Upload callback: <color=green>{www.downloadHandler.text}</color>");

        try
        {
          uploadCallbackPayload = JsonUtility.FromJson<UploadCallbackPayload>(www.downloadHandler.text);
          StartCoroutine(DownloadQRImage(uploadCallbackPayload.data.web_qr));
        }
        catch (Exception)
        {
          Debug.LogWarning($"[U2ZCM] Failed to parse Json.");
          OnUploadFailed?.Invoke();
          yield break;
        }
      }

      StartCoroutine(QueryTask());
    }



    /// <summary>
    /// 查询
    /// 直到图片处理完成
    /// </summary>
    private IEnumerator QueryTask()
    {
      string queryFullURL = @$"{QUERYURL}?{"file_code"}={UnityWebRequest.EscapeURL(uploadCallbackPayload.data.file_code)}";

      int queryIndex = 0;
      while (queryIndex < MAXQUERYCOUNT)
      {
        Debug.Log(@$"[U2ZCM] Query attempt <color=yellow>#{++queryIndex}</color>");

        using (UnityWebRequest www = UnityWebRequest.Get(queryFullURL))
        {
          www.downloadHandler = new DownloadHandlerBuffer();
          yield return www.SendWebRequest();
          if (www.result != UnityWebRequest.Result.Success)
          {
            Debug.LogWarning($"[U2ZCM] {www.error}");
            OnUploadFailed?.Invoke();
            yield break;
          }

          // Debug.Log($"[U2ZCM] Query callback: <color=green>{www.downloadHandler.text}</color>");

          try
          {
            queryCallbackPayload = JsonUtility.FromJson<QueryCallbackPayload>(www.downloadHandler.text);
          }
          catch (Exception)
          {
            Debug.LogWarning($"[U2ZCM] Failed to parse Json.");
            OnUploadFailed?.Invoke();
            yield break;
          }

          if (queryCallbackPayload.data.gemini.status == 3) // 有结果了就走
          {
            Debug.Log($"[U2ZCM] Query success.");
            StartCoroutine(DownloadGeneratedImage(queryCallbackPayload.data.gemini.image_url));
            // StartCoroutine(DownloadQRImage(queryCallbackPayload.data.web_qr));
            OnQueryFinished?.Invoke();
            yield break;
          }

          yield return new WaitForSeconds(QUERYSPACETIME);
        }
      }

      // 轮询超过上限仍未完成，判定为失败
      Debug.LogWarning("[U2ZCM] Query timeout, reached max poll count.");
      OnUploadFailed?.Invoke();
    }



    /// <summary>
    /// 下载最终生成图片
    /// </summary>
    [Space][SerializeField] private Texture2D t2dGeneratedImage;
    private IEnumerator DownloadGeneratedImage(string imageURL)
    {
      using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(imageURL))
      {
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.LogWarning($"[U2ZCM] {unityWebRequest.error}");
          OnUploadFailed?.Invoke();
          yield break;
        }

        Debug.Log($"[U2ZCM] Generated image downloaded.");
        t2dGeneratedImage = DownloadHandlerTexture.GetContent(unityWebRequest);
        OnGeneratedImageDownloaded?.Invoke(DownloadHandlerTexture.GetContent(unityWebRequest));
      }
    }

    [SerializeField] private Texture2D t2dQRImage;
    private IEnumerator DownloadQRImage(string qrURL)
    {
      using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(qrURL))
      {
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.Log($"[U2ZCM] {unityWebRequest.error}");
          OnUploadFailed?.Invoke();
          yield break;
        }

        Debug.Log($"[U2ZCM] QR image downloaded.");
        t2dQRImage = DownloadHandlerTexture.GetContent(unityWebRequest);
        OnQRImageDownloaded?.Invoke(DownloadHandlerTexture.GetContent(unityWebRequest));
      }
    }

    #endregion
  }
}