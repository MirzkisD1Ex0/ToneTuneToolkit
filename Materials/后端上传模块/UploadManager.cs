using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace OwnTheFloor
{
  public class UploadManager : MonoBehaviour
  {
    public static UploadManager Instance;

    private event UnityAction<string, string> OnFinalCallbackUpdate; // sting形参

    private int appID = 76;
    private float retryWaitTime = 30f; // 重新上传尝试间隔

    private Texture2D currentTexture2D;
    private string currentFileName;

    private TokenJson tokenJson = new TokenJson();
    private CloudCallbackJson cloudCallbackJson = new CloudCallbackJson();
    private ServerJson serverJson = new ServerJson();
    private ServerCallbackJson serverCallbackJson = new ServerCallbackJson();

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    // ==================================================

    public void AddEventListener(UnityAction<string, string> unityAction)
    {
      OnFinalCallbackUpdate += unityAction;
      return;
    }

    public void RemoveEventListener(UnityAction<string, string> unityAction)
    {
      OnFinalCallbackUpdate -= unityAction;
      return;
    }

    private void EventNoticeAll()
    {
      if (OnFinalCallbackUpdate == null) // 如果没人订阅
      {
        return;
      }
      // OnFinalCallbackUpdate(serverCallbackJson.data.view_url); // 把viewurl丢出去
      OnFinalCallbackUpdate(serverCallbackJson.data.view_url, serverCallbackJson.data.file_url); // 把fileurl丢出去
      return;
    }

    // ==================================================

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileTexture"></param>
    /// <param name="fileName"></param>
    public void UploadData2Net(Texture2D fileTexture, string fileName)
    {
      currentTexture2D = fileTexture;
      currentFileName = fileName;
      StartCoroutine(GetToken4Cloud());
      return;
    }



    /// <summary>
    /// 获取Token
    /// 第一步
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetToken4Cloud()
    {
      string url = @"https://h5.skyelook.com/api/qiniu/getAccessToken";

      using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
      {
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.Log(unityWebRequest.error + "...<color=red>[ER]</color>");
          StartCoroutine(RetryUpload());
        }
        else
        {
          tokenJson = JsonConvert.DeserializeObject<TokenJson>(unityWebRequest.downloadHandler.text);
          Debug.Log($"Get token sucessed: {tokenJson.data.token}...<color=green>[OK]</color>");

          StartCoroutine(UploadData2Cloud());
        }
      }
      yield break;
    }

    /// <summary>
    /// 上传文件到七牛云
    /// 第二步
    /// </summary>
    private IEnumerator UploadData2Cloud()
    {
      string url = "https://upload.qiniup.com";
      byte[] bytes = currentTexture2D.EncodeToPNG();

      WWWForm wwwForm = new WWWForm();
      wwwForm.AddField("token", tokenJson.data.token);
      wwwForm.AddBinaryData("file", bytes, currentFileName);

      using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, wwwForm))
      {
        // unityWebRequest.SetRequestHeader("Content-Type", "multipart/form-data;charset=utf-8");
        // unityWebRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        // unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.Log(unityWebRequest.error + "...<color=red>[ER]</color>");
          StartCoroutine(RetryUpload());
        }
        else
        {
          cloudCallbackJson = JsonConvert.DeserializeObject<CloudCallbackJson>(unityWebRequest.downloadHandler.text);
          Debug.Log($"Upload sucessed: {cloudCallbackJson.data.file_url}...<color=green>[OK]</color>");

          StartCoroutine(SaveFile2Server());
        }
      }
      yield break;
    }

    /// <summary>
    /// 七牛云返回数据传至服务器
    /// 第三步
    /// </summary>
    /// <returns></returns>
    private IEnumerator SaveFile2Server()
    {
      string url = "https://h5.skyelook.com/api/attachments";

      serverJson.file_url = cloudCallbackJson.data.file_url;
      serverJson.app_id = appID;
      // serverJson.options = "google-gds-print";

      string jsonString = JsonConvert.SerializeObject(serverJson);
      byte[] bytes = Encoding.Default.GetBytes(jsonString);

      Debug.Log(jsonString);

      using (UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST"))
      {
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.Log(unityWebRequest.error + "...<color=red>[ER]</color>");
          StartCoroutine(RetryUpload());
        }
        else
        {
          serverCallbackJson = JsonConvert.DeserializeObject<ServerCallbackJson>(unityWebRequest.downloadHandler.text);
          Debug.Log($"{unityWebRequest.downloadHandler.text}");
          Debug.Log($"{serverCallbackJson.data.view_url}...<color=green>[OK]</color>");

          EventNoticeAll(); // 钩子在此
        }
      }
      yield break;
    }

    /// <summary>
    /// 传不上去硬传
    /// </summary>
    /// <returns></returns>
    private IEnumerator RetryUpload()
    {
      yield return new WaitForSeconds(retryWaitTime);
      UploadData2Cloud();
      yield break;
    }

    // ==================================================
    // Json解析类
    // 七牛云Token回执
    public class TokenJson
    {
      public int status;
      public int code;
      public TokenDataJson data;
      public string message;
    }
    public class TokenDataJson
    {
      public string token;
    }

    // 七牛云文件上传回执
    public class CloudCallbackJson
    {
      public int code;
      public CloudCallbackDataJson data;
      public int status;
    }
    public class CloudCallbackDataJson
    {
      public string file_name;
      public string file_url;
    }

    // 向服务器发送的json
    public class ServerJson
    {
      public string file_url;
      public int app_id;
      // public string options;
    }

    // 服务器回执
    public class ServerCallbackJson
    {
      public int status;
      public int code;
      public ServerCallbackDataJson data;
    }
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
}