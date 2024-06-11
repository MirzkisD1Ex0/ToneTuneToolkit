using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Best.HTTP;
using Best.HTTP.Request.Upload.Forms;
using UnityEngine.Events;
using ToneTuneToolkit.Data;
using UnityEngine.Networking;
using System.Text;

namespace LonginesYogaPhotoJoy
{
  public class RemoveBGManager : MonoBehaviour
  {
    public static RemoveBGManager Instance;

    private UnityAction<Texture2D> onRemoveBGCompelete;

    private const string removebgAPI = "https://api.remove.bg/v1.0/removebg";
    // private const string removebgAPI = "http://192.168.50.130:3500";
    private string key;
    // 测试key U1j4pJeg9zT63Kfa8zDmiRkG
    // 正式key 76YHaSA8WZYmbZXfqfBeYbqy // 20240606 剩余100
    // live X859F9v3g4YpoPBRQe2n7h8T

    [Header("DEBUG - Peek")]
    [SerializeField] private Texture2D preuploadTexture;
    [SerializeField] private Texture2D downloadTexture;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      Init();
    }

    // ==================================================

    public void AddEventListener(UnityAction<Texture2D> unityAction)
    {
      onRemoveBGCompelete += unityAction;
      return;
    }

    public void RemoveEventListener(UnityAction<Texture2D> unityAction)
    {
      onRemoveBGCompelete -= unityAction;
      return;
    }

    // ==================================================

    private void Init()
    {
      key = JsonManager.GetJson(Application.streamingAssetsPath + "/removebgkey.json", "Key");
      return;
    }

    // ==================================================


    // public void UploadPhoto2RemoveBG(Texture2D value)
    // {
    //   StartCoroutine(UploadPhoto2RemoveBGAction(value));
    //   return;
    // }
    // private IEnumerator UploadPhoto2RemoveBGAction(Texture2D value)
    // {
    //   yield return new WaitForEndOfFrame();
    //   byte[] bytes = value.EncodeToPNG(); // 图转比特流

    //   // string base64 = "data:image/jpg;base64," + Convert.ToBase64String(bytes);
    //   WWWForm form = new WWWForm();
    //   form.AddBinaryData("image_file", bytes);
    //   form.AddField("size", "auto");
    //   form.AddField("type", "person");

    //   using (UnityWebRequest www = UnityWebRequest.Post(removebgAPI, form))
    //   {
    //     www.SetRequestHeader("Content-Type", "multipart/form-data");
    //     // www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    //     www.SetRequestHeader("X-Api-Key", key);
    //     DownloadHandler downloadHandler = new DownloadHandlerBuffer();
    //     www.downloadHandler = downloadHandler;
    //     yield return www.SendWebRequest();
    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log(www.error + "...<color=red>[ER]</color>");
    //       yield break;
    //     }
    //     else
    //     {
    //       result = www.downloadHandler.text.ToString();
    //       Debug.Log("RemoveBG...<color=green>[OK]</color>");
    //       // Debug.Log(result + "...<color=green>[OK]</color>");
    //     }
    //   }
    // }





    // public void UploadPhoto2RemoveBG(Texture2D value)
    // {
    //   StartCoroutine(UploadPhoto2RemoveBGAction(value));
    //   return;
    // }

    public void UploadPhoto2RemoveBG(Texture2D value)
    {
      preuploadTexture = value;

      byte[] bytes = value.EncodeToJPG(); // 图转比特流
      MultipartFormDataStream form = new MultipartFormDataStream();
      form.AddStreamField("image_file", new MemoryStream(bytes), "filename", "image/jpg");
      form.AddField("size", "auto");
      form.AddField("type", "person");

      HTTPRequest request = HTTPRequest.CreatePost(new Uri(removebgAPI), RequestFinishedCallback);
      request.SetHeader("X-Api-Key", key);
      request.UploadSettings.UploadStream = form;
      request.Send();
      return;
    }

    private void RequestFinishedCallback(HTTPRequest req, HTTPResponse resp)
    {
      switch (req.State)
      {
        case HTTPRequestStates.Finished:
          if (resp.IsSuccess)
          {
            if (onRemoveBGCompelete != null)
            {
              onRemoveBGCompelete(resp.DataAsTexture2D);
            }
            Debug.Log("Upload finished succesfully!");
          }
          else
          {
            // 6. Error handling
            Debug.LogError($"Server sent an error: {resp.StatusCode}-{resp.Message}");
          }
          break;

        // 6. Error handling
        default:
          Debug.LogError($"Request finished with error! Request state: {req.State}");
          break;
      }
    }

    #region 可用
    // /// <summary>
    // /// 上传照片至RemoveBG
    // /// </summary>
    // /// <param name="value"></param>
    // public void UploadPhoto2RemoveBG(Texture2D value)
    // {
    //   preuploadTexture = value;
    //   byte[] bytes = value.EncodeToPNG();

    //   MultipartFormDataStream form = new MultipartFormDataStream();
    //   form.AddField("image_file", bytes);
    //   // form.AddField("size", "full");
    //   form.AddField("size", "auto");
    //   form.AddField("type", "person");

    //   HTTPRequest request = new HTTPRequest(
    //     new Uri(removebgAPI),
    //     HTTPMethods.Post,
    //     UploadPictureCallback);

    //   request.SetHeader("X-Api-Key", key);
    //   // request.SetForm(form);
    //   request.UploadSettings.UploadStream = form;
    //   request.Send();
    //   return;
    // }

    // private void UploadPictureCallback(HTTPRequest httpRequest, HTTPResponse httpResponse)
    // {

    //   Debug.Log(httpResponse.StatusCode);
    //   if (httpResponse == null)
    //   {
    //     Debug.Log("RemoveBG请求无响应...<color=red>[ER]</color>");
    //     return;
    //   }
    //   if (httpResponse.StatusCode == 200)
    //   {
    //     Debug.Log(httpResponse.DataAsTexture2D.width + " / " + httpResponse.DataAsTexture2D.height);

    //     string fullPath = $"{Application.streamingAssetsPath}/RemoveBG/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png";
    //     byte[] bytes = httpResponse.DataAsTexture2D.EncodeToPNG();
    //     File.WriteAllBytes(fullPath, bytes);

    //     downloadTexture = httpResponse.DataAsTexture2D; // 无必要

    //     if (onRemoveBGCompelete != null)
    //     {
    //       onRemoveBGCompelete(httpResponse.DataAsTexture2D);
    //     }
    //     Debug.Log("RemoveBG返回成功...[OK]");
    //   }
    //   else
    //   {
    //     Debug.Log($"RemoveBG返回失败,{httpResponse.StatusCode}...<color=red>[ER]</color>");
    //     Debug.Log(httpResponse.DataAsText);
    //     // UploadPhoto2RemoveBG(preuoloadTexture);
    //   }
    //   return;
    // }
    #endregion
  }
}