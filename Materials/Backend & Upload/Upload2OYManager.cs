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

/// <summary>
/// 对乔哥法宝
/// </summary>
public class Upload2OYManager : SingletonMaster<Upload2OYManager>
{
  public static UnityAction<Texture2D> OnUploadFinishedBackTexture;
  public static UnityAction<string> OnUploadFinishedBackString;

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
  #region Step 00 // 完善文件信息

  [Space]
  [SerializeField] private string fileName;
  [SerializeField] private string filePath;

  public void UpdateFileInfo(string name, string path)
  {
    fileName = name;
    filePath = path;
    return;
  }

  #endregion
  // ==================================================
  #region Step 01 // 获取Token

  public void UploadData2Net() => StartCoroutine(nameof(GetTokenFromCloud));
  private IEnumerator GetTokenFromCloud()
  {
    using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(cloudTokenURL))
    {
      yield return unityWebRequest.SendWebRequest();
      if (unityWebRequest.result != UnityWebRequest.Result.Success)
      {
        Debug.Log($"[U2OYM] {unityWebRequest.error}");
        StartCoroutine(nameof(RetryUpload));
      }
      else
      {
        tokenJson = JsonConvert.DeserializeObject<TokenCallbackJson>(unityWebRequest.downloadHandler.text);
        Debug.Log($"[U2OYM] Get token sucessed: {tokenJson.data.token}");

        StartCoroutine(nameof(PoseFile2Cloud)); // 下一步
      }
    }
    yield break;
  }

  #endregion
  // ==================================================
  #region Step 02 // 上传文件到七牛云

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
        Debug.Log($"[U2OYM] {unityWebRequest.error}");
        StartCoroutine(nameof(RetryUpload));
      }
      else
      {
        cloudCallbackJson = JsonConvert.DeserializeObject<CloudCallbackJson>(unityWebRequest.downloadHandler.text);
        Debug.Log($"[U2OYM] Upload sucessed: {cloudCallbackJson.data.file_url}");

        StartCoroutine(SaveFile2Server()); // 下一步
      }
    }
    yield break;
  }

  #endregion
  // ==================================================
  #region Step 03 // 七牛云返回数据传至服务器

  private IEnumerator SaveFile2Server()
  {
    serverJson.file_url = cloudCallbackJson.data.file_url;
    serverJson.app_id = appID;

    string jsonString = JsonConvert.SerializeObject(serverJson);
    byte[] bytes = Encoding.Default.GetBytes(jsonString);

    using (UnityWebRequest unityWebRequest = new UnityWebRequest(cloudURL, "POST"))
    {
      unityWebRequest.SetRequestHeader("Content-Type", "application/json");
      unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
      unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

      yield return unityWebRequest.SendWebRequest();
      if (unityWebRequest.result != UnityWebRequest.Result.Success)
      {
        Debug.Log($"[U2OYM] {unityWebRequest.error}");
        StartCoroutine(nameof(RetryUpload));
      }
      else
      {
        serverCallbackJson = JsonConvert.DeserializeObject<ServerCallbackJson>(unityWebRequest.downloadHandler.text);
        Debug.Log($"[U2OYM] {serverCallbackJson.data.view_url}");

        // 返回链接
        if (OnUploadFinishedBackString != null)
        {
          OnUploadFinishedBackString(serverCallbackJson.data.view_url);
        }

        // 组装
        sunCodeURL = $"https://h5.skyelook.com/api/wechat/getQrcodeApp/{serverCallbackJson.data.code}/wx039a4c76d8788bb0/?env=trial"; // ?env=trial // 额外添加?
        StartCoroutine(nameof(GetSunCode4Server)); // 下一步搞图
      }
    }
    yield break;
  }

  #endregion
  // ==================================================
  #region Step 04 // 从服务器上获取码

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
        if (OnUploadFinishedBackTexture != null)
        {
          OnUploadFinishedBackTexture(((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture);
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