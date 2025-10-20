using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Networking
{
  /// <summary>
  /// 对乔宝具
  /// </summary>
  public class Upload2OYManager : SingletonMaster<Upload2OYManager>
  {
    public static UnityAction<string> OnUploadFinishedBackString;
    public static UnityAction<Texture2D> OnUploadFinishedBackTexture;

    private const int AppID = 100;
    private const float RetryWaitTime = 30f; // 重新上传尝试间隔

    [Header("Qiniu Access Token Callback")]
    [SerializeField] private QiniuAccessTokenCallback tokenCallback = new QiniuAccessTokenCallback();
    [Header("Qiniu Callback")]
    [SerializeField] private QiniuCallback qiniuCallback = new QiniuCallback();
    [Header("Server Callback")]
    [SerializeField] private ServerPackage serverPackage = new ServerPackage();
    [SerializeField] private ServerCallback serverCallback = new ServerCallback();

    private const string QiniuAccessTokenURL = @"https://h5.skyelook.com/api/qiniu/getAccessToken";
    private const string QiniuURL = @"https://upload.qiniup.com";
    private const string cloudURL = @"https://h5.skyelook.com/api/attachments";

    // ==================================================
    #region Step 00 // 完善文件信息

    private string fileName;
    private byte[] fileStream;

    public void UpdateFilePackage(string name, byte[] stream)
    {
      fileName = name;
      fileStream = stream;
    }

    #endregion
    // ==================================================
    #region Step 01 // 获取Token

    public void UploadFile() => StartCoroutine(nameof(GetQiniuAccessToken));

    private IEnumerator GetQiniuAccessToken()
    {
      using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(QiniuAccessTokenURL))
      {
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.LogWarning($"[U2OYM] {unityWebRequest.error}");
          StartCoroutine(nameof(RetryUpload));
        }
        else
        {
          tokenCallback = JsonConvert.DeserializeObject<QiniuAccessTokenCallback>(unityWebRequest.downloadHandler.text);
          Debug.Log($"[U2OYM] Get qiniu access token sucessed: {tokenCallback.data.token}");

          StartCoroutine(nameof(PostFile2Qiniu)); // 下一步
        }
      }
    }

    #endregion
    // ==================================================
    #region Step 02 // 上传文件到七牛云

    private IEnumerator PostFile2Qiniu()
    {
      WWWForm wwwForm = new WWWForm();
      wwwForm.AddField("token", tokenCallback.data.token);
      wwwForm.AddBinaryData("file", fileStream, fileName);

      using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(QiniuURL, wwwForm))
      {
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.LogWarning($"[U2OYM] {unityWebRequest.error}");
          StartCoroutine(nameof(RetryUpload));
        }
        else
        {
          qiniuCallback = JsonConvert.DeserializeObject<QiniuCallback>(unityWebRequest.downloadHandler.text);
          Debug.Log($"[U2OYM] Post file 2 qiniu sucessed: {qiniuCallback.data.file_url}");

          StartCoroutine(nameof(PostFile2Server)); // 下一步
        }
      }
    }

    #endregion
    // ==================================================
    #region Step 03 // 七牛云返回数据传至服务器

    private IEnumerator PostFile2Server()
    {
      serverPackage.file_url = qiniuCallback.data.file_url;
      serverPackage.app_id = AppID;

      string jsonString = JsonConvert.SerializeObject(serverPackage);
      byte[] bytes = Encoding.Default.GetBytes(jsonString);

      using (UnityWebRequest unityWebRequest = new UnityWebRequest(cloudURL, "POST"))
      {
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.LogWarning($"[U2OYM] {unityWebRequest.error}");
          StartCoroutine(nameof(RetryUpload));
        }
        else
        {
          serverCallback = JsonConvert.DeserializeObject<ServerCallback>(unityWebRequest.downloadHandler.text);
          Debug.Log($"[U2OYM] Post file info 2 server sucessed: {serverCallback.data.view_url}");

          // 返回链接
          OnUploadFinishedBackString?.Invoke(serverCallback.data.view_url);

          // 组装
          qrCodeURL = $"https://h5.skyelook.com/api/wechat/getQrcodeApp/{serverCallback.data.code}/wx039a4c76d8788bb0/?env=trial"; // ?env=trial // 额外添加?

          StartCoroutine(nameof(GetQRCode4Server)); // 下一步搞图
        }
      }
    }

    #endregion
    // ==================================================
    #region Step 04 // 从服务器上获取码

    [SerializeField] private string qrCodeURL;
    [SerializeField] private Texture2D t2dQRCode;

    private IEnumerator GetQRCode4Server()
    {
      using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(qrCodeURL)) // new UnityWebRequest(sunCodeURL, "GET"))
      {
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.LogWarning($"[U2OYM] {unityWebRequest.error}");
          StartCoroutine(nameof(RetryUpload));
        }
        else
        {
          t2dQRCode = ((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture;
          Debug.Log($"[U2OYM] Get qr texture sucessed");

          OnUploadFinishedBackTexture?.Invoke(t2dQRCode); // 返回图
        }
      }
    }

    #endregion
    // ==================================================
    #region 传不上去硬传

    private IEnumerator RetryUpload()
    {
      yield return new WaitForSeconds(RetryWaitTime);
      PostFile2Qiniu();
    }

    #endregion
    // ==================================================
    #region Json解析类

    // 七牛云Token回执
    [Serializable]
    public class QiniuAccessTokenCallback
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
    public class QiniuCallback
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
    public class ServerPackage
    {
      public string file_url;
      public int app_id;
      // public string options;
    }
    [Serializable]
    public class ServerCallback
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
  #endregion
}