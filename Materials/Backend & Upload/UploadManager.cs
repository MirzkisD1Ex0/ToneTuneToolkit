using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.IO;
using System;
using ToneTuneToolkit.Common;

public class UploadManager : SingletonMaster<UploadManager>
{
  public static UnityAction<Texture2D> OnUpdateFinishedBackTexture;
  public static UnityAction<string> OnUpdateFinishedBackString;

  private int appID = 78;
  private float retryWaitTime = 30f; // 重新上传尝试间隔

  [Header("Token")]
  [SerializeField] private TokenCallbackJson tokenJson = new TokenCallbackJson();
  [Header("Cloud")]
  [SerializeField] private CloudCallbackJson cloudCallbackJson = new CloudCallbackJson();
  [Header("Server")]
  [SerializeField] private ServerJson serverJson = new ServerJson();
  [SerializeField] private ServerCallbackJson serverCallbackJson = new ServerCallbackJson();

  private const string cloudTokenURL = @"https://h5.skyelook.com/api/qiniu/getAccessToken";
  private const string qiniuURL = @"https://upload.qiniup.com";
  private const string cloudURL = @"https://h5.skyelook.com/api/attachments";

  // ==================================================

  // private void EventNoticeAll()
  // {
  //   if (OnFinalCallbackUpdate == null) // 如果没人订阅
  //   {
  //     return;
  //   }
  //   OnFinalCallbackUpdate(serverCallbackJson.data.view_url); // 把viewurl丢出去
  //   return;
  // }

  // ==================================================

  [SerializeField] private string fileName;
  [SerializeField] private string filePath;

  public void UpdateFileInfo(string nameString, string pathString)
  {
    fileName = nameString;
    filePath = pathString;
    return;
  }

  // ==================================================
  #region Step 00 // 获取Token

  public void UploadData2Net() => StartCoroutine(nameof(GetTokenFromCloud));
  private IEnumerator GetTokenFromCloud()
  {
    using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(cloudTokenURL))
    {
      yield return unityWebRequest.SendWebRequest();
      if (unityWebRequest.result != UnityWebRequest.Result.Success)
      {
        Debug.Log($"[UploadManager] {unityWebRequest.error}");
        StartCoroutine(nameof(RetryUpload));
      }
      else
      {
        tokenJson = JsonConvert.DeserializeObject<TokenCallbackJson>(unityWebRequest.downloadHandler.text);
        Debug.Log($"[UploadManager] Get token sucessed: {tokenJson.data.token}");

        StartCoroutine(nameof(PoseFile2Cloud)); // 下一步
      }
    }
    yield break;
  }

  #endregion
  // ==================================================
  #region Step 01 // 上传文件到七牛云

  private IEnumerator PoseFile2Cloud()
  {
    byte[] bytes = File.ReadAllBytes(filePath); // 文件转流

    WWWForm wwwForm = new WWWForm();
    wwwForm.AddField("token", tokenJson.data.token);
    wwwForm.AddBinaryData("file", bytes, fileName);

    using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(qiniuURL, wwwForm))
    {
      yield return unityWebRequest.SendWebRequest();
      if (unityWebRequest.result != UnityWebRequest.Result.Success)
      {
        Debug.Log($"[UploadManager] {unityWebRequest.error}");
        StartCoroutine(nameof(RetryUpload));
      }
      else
      {
        cloudCallbackJson = JsonConvert.DeserializeObject<CloudCallbackJson>(unityWebRequest.downloadHandler.text);
        Debug.Log($"[UploadManager] Upload sucessed: {cloudCallbackJson.data.file_url}");

        StartCoroutine(SaveFile2Server()); // 下一步
      }
    }
    yield break;
  }

  #endregion
  // ==================================================
  #region Step 02 // 七牛云返回数据传至服务器

  private IEnumerator SaveFile2Server()
  {
    serverJson.file_url = cloudCallbackJson.data.file_url;
    serverJson.app_id = appID;

    string jsonString = JsonConvert.SerializeObject(serverJson);
    byte[] bytes = Encoding.Default.GetBytes(jsonString);

    // Debug.Log(jsonString);

    using (UnityWebRequest unityWebRequest = new UnityWebRequest(cloudURL, "POST"))
    {
      unityWebRequest.SetRequestHeader("Content-Type", "application/json");
      unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
      unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

      yield return unityWebRequest.SendWebRequest();
      if (unityWebRequest.result != UnityWebRequest.Result.Success)
      {
        Debug.Log($"[UploadManager] {unityWebRequest.error}");
        StartCoroutine(nameof(RetryUpload));
      }
      else
      {
        serverCallbackJson = JsonConvert.DeserializeObject<ServerCallbackJson>(unityWebRequest.downloadHandler.text);
        // Debug.Log($"{unityWebRequest.downloadHandler.text}");
        Debug.Log($"[UploadManager] {serverCallbackJson.data.view_url}");

        // 返回链接
        if (OnUpdateFinishedBackString != null)
        {
          OnUpdateFinishedBackString(serverCallbackJson.data.view_url);
        }

        // 第三步 搞图
        sunCodeURL = $"https://h5.skyelook.com/api/wechat/getQrcodeApp/{serverCallbackJson.data.code}/wx039a4c76d8788bb0";

        // EventNoticeAll(); // 钩子在此
        StartCoroutine(nameof(GetSunCode4Server));
      }
    }
    yield break;
  }

  #endregion
  // ==================================================
  #region Step 03 // 从服务器上获取码

  [SerializeField] private string sunCodeURL;
  [SerializeField] private Texture2D finalSunCode;
  private IEnumerator GetSunCode4Server()
  {
    using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(sunCodeURL)) // new UnityWebRequest(sunCodeURL, "GET"))
    {
      yield return unityWebRequest.SendWebRequest();
      if (unityWebRequest.result != UnityWebRequest.Result.Success)
      {
        Debug.Log("[UM] " + unityWebRequest.error);
      }
      else
      {
        // td = new Texture2D(600, 600);
        // td.LoadImage(unityWebRequest.tex);
        finalSunCode = ((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture;

        // 返回图
        if (OnUpdateFinishedBackTexture != null)
        {
          OnUpdateFinishedBackTexture(((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture);
        }
      }
    }
    yield break;
  }

  #endregion
  // ==================================================

  /// <summary>
  /// 传不上去硬传
  /// </summary>
  /// <returns></returns>
  private IEnumerator RetryUpload()
  {
    yield return new WaitForSeconds(retryWaitTime);
    PoseFile2Cloud();
    yield break;
  }

  // ==================================================
  // Json解析类

  // 七牛云Token回执
  [Serializable]
  public class TokenCallbackJson
  {
    public int status;
    public int code;
    public TokenDataJson data;
    public string message;
  }
  [Serializable]
  public class TokenDataJson
  {
    public string token;
  }

  // 七牛云文件上传回执
  [Serializable]
  public class CloudCallbackJson
  {
    public int code;
    public CloudCallbackDataJson data;
    public int status;
  }
  [Serializable]
  public class CloudCallbackDataJson
  {
    public string file_name;
    public string file_url;
  }

  // 向服务器发送的json
  [Serializable]
  public class ServerJson
  {
    public string file_url;
    public int app_id;
    // public string options;
  }
  [Serializable]
  public class ServerCallbackJson
  {
    public int status;
    public int code;
    public ServerCallbackDataJson data;
  }

  [Serializable]
  public class ServerCallbackDataJson
  {
    public string file_url;
    public int app_id;
    public string code;
    public string view_url;
    public string updated_at;
    public string created_at;
    public int id;
  }
}