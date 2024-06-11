using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace LonginesYogaPhotoJoy
{
  /// <summary>
  /// 后端对接专用
  /// </summary>
  public class BackendNetManager : MonoBehaviour
  {
    public static BackendNetManager Instance;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      Init();
    }

    // private void Update()
    // {
    //   if (Input.GetKeyUp(KeyCode.U))
    //   {
    //     string testPath = @"D:\2024-06-03-21-28-17.png";
    //     UploadPhoto2Backend(ToneTuneToolkit.Media.TextureProcessor.ReadTexture(testPath));
    //   }
    // }

    // ==================================================

    private void Init()
    {
      GetStartupQR();
      return;
    }

    // ==================================================
    #region 获取启动QR码
    public event UnityAction<string> OnGetStartupInfoComplete;
    public StartupQRResponse QRData;

    private const string qrURL = "https://open.skyelook.com/api/longine_gz/startQr";
    private const string deviceCode = "Test_001";
    public void GetStartupQR()
    {
      StartCoroutine("GetStartupQRAction");
      return;
    }
    private IEnumerator GetStartupQRAction()
    {
      WWWForm wwwForm = new WWWForm();
      wwwForm.AddField("device_code", deviceCode);

      using (UnityWebRequest www = UnityWebRequest.Post(qrURL, wwwForm)) // 获取二维码链接
      {
        // www.SetRequestHeader("Content-Type", "multipart/form-data"); // 永远永远不要用这个
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        DownloadHandler downloadHandler = new DownloadHandlerBuffer();
        www.downloadHandler = downloadHandler;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
          Debug.Log($"{www.error}...<color=red>[ER]</color>");
          yield break;
        }
        else
        {
          Debug.Log($"QRCode Json :\n{www.downloadHandler.text}...<color=green>[OK]</color>");
          QRData = JsonConvert.DeserializeObject<StartupQRResponse>(www.downloadHandler.text.ToString());

          if (OnGetStartupInfoComplete != null)
          {
            OnGetStartupInfoComplete(QRData.data.start_code);
          }
          Debug.Log("QRCode Json...<color=green>[OK]</color>");
        }
      }

      // using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(QRData.data.qr_url)) // 获取二维码
      // {
      //   yield return www.SendWebRequest();
      //   if (www.result != UnityWebRequest.Result.Success)
      //   {
      //     Debug.Log($"{www.error}...<color=red>[ER]</color>");
      //     yield break;
      //   }
      //   else
      //   {
      //     if (OnGetStartupQRComplete != null)
      //     {
      //       OnGetStartupQRComplete(DownloadHandlerTexture.GetContent(www));
      //     }
      //     Debug.Log("QRCode Image...<color=green>[OK]</color>");
      //     StartCoroutine("QueryUserStatus"); // 启动轮询
      //   }
      // }
      yield break;
    }

    [Serializable]
    public class StartupQRResponse
    {
      public int code;
      public string message;
      public StartupQRData data;
    }
    [Serializable]
    public class StartupQRData
    {
      public string start_code;
      public string qr_url;
    }
    #endregion

    // ==================================================
    #region 轮询是否有玩家在玩
    // public event UnityAction OnUserActive;
    // public StatusResponse StatusData;

    // private const string statusURL = "https://open.skyelook.com/api/longine_gz/startStatus";
    // private IEnumerator QueryUserStatus()
    // {
    //   WWWForm wwwForm = new WWWForm();
    //   wwwForm.AddField("device_code", deviceCode);
    //   wwwForm.AddField("start_code", QRData.data.start_code);

    //   using (UnityWebRequest www = UnityWebRequest.Post(statusURL, wwwForm))
    //   {
    //     www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    //     www.downloadHandler = new DownloadHandlerBuffer();
    //     yield return www.SendWebRequest();
    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log($"{www.error}...<color=red>[ER]</color>");
    //       yield break;
    //     }
    //     else
    //     {
    //       StatusData = JsonConvert.DeserializeObject<StatusResponse>(www.downloadHandler.text);
    //       // Debug.Log($"{www.downloadHandler.text}...<color=green>[OK]</color>");
    //       Debug.Log("Query...<color=green>[OK]</color>");
    //     }
    //   }

    //   // 轮询 // 启动游戏
    //   switch (StatusData.data.status)
    //   {
    //     default: break;
    //     case "0":
    //       yield return new WaitForSeconds(2f);
    //       StartCoroutine("QueryUserStatus");
    //       break;
    //     case "1":
    //       if (OnUserActive != null)
    //       {
    //         OnUserActive();
    //       }
    //       StopCoroutine("QueryUserStatus");
    //       break;
    //     case "2": break;
    //     case "3": break;
    //   }
    //   yield break;
    // }

    // [Serializable]
    // public class StatusResponse
    // {
    //   public int code;
    //   public string message;
    //   public StatusResponseData data;
    // }
    // [Serializable]
    // public class StatusResponseData
    // {
    //   public string status;
    //   public string status_text;
    // }
    #endregion

    // ==================================================
    #region 上传图片
    public UploadResponse UploadData;

    public event UnityAction<Texture2D> OnUpload;

    private const string uploadURL = "https://open.skyelook.com/api/longine_gz/uploadThumb";

    public void UploadPhoto2Backend(Texture2D texture2D)
    {
      StartCoroutine(UploadPhoto2BackendAction(texture2D));
      return;
    }

    private IEnumerator UploadPhoto2BackendAction(Texture2D texture2D)
    {
      byte[] bytes = texture2D.EncodeToPNG(); // 图转比特流
      string base64 = "data:image/png;base64," + Convert.ToBase64String(bytes);

      WWWForm wwwForm = new WWWForm();
      wwwForm.AddField("device_code", deviceCode);
      wwwForm.AddField("start_code", QRData.data.start_code);
      wwwForm.AddField("file", base64);
      // wwwForm.AddBinaryData("file", bytes);

      using (UnityWebRequest www = UnityWebRequest.Post(uploadURL, wwwForm))
      {
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
          Debug.Log($"{www.error}...<color=red>[ER]</color>");
          yield break;
        }
        else
        {
          Debug.Log("Upload...<color=green>[OK]</color>");
          Debug.Log(www.downloadHandler.text.ToString());
          UploadData = JsonConvert.DeserializeObject<UploadResponse>(www.downloadHandler.text.ToString());
        }
      }

      using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(UploadData.data.qr_url)) // 搞最终QR图 // DEBUG
      {
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
          Debug.Log($"{www.error}...<color=red>[ER]</color>");
          yield break;
        }
        else
        {
          if (OnUpload != null)
          {
            OnUpload(DownloadHandlerTexture.GetContent(www));
          }
          Debug.Log("Final QRCode...<color=green>[OK]</color>");
        }
      }
      yield break;
    }

    [Serializable]
    public class UploadResponse
    {
      public int code;
      public string message;
      public UploadResponseData data;
    }
    [Serializable]
    public class UploadResponseData
    {
      public string file_url;
      public string qr_url;
    }
    #endregion
  }
}