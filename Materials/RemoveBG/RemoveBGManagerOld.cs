using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using BestHTTP;
using BestHTTP.Forms;
using UnityEngine.Events;
using ToneTuneToolkit.Data;

namespace LonginesYogaPhotoJoy
{
  public class RemoveBGManagerOld : MonoBehaviour
  {
    public static RemoveBGManagerOld Instance;

    private UnityAction<Texture2D> onRemoveBGCompelete;

    private const string removebgAPI = "https://api.remove.bg/v1.0/removebg";
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

    /// <summary>
    /// 上传照片至RemoveBG
    /// </summary>
    /// <param name="value"></param>
    public void UploadPhoto2RemoveBG(Texture2D value)
    {
      preuploadTexture = value;
      byte[] bytes = value.EncodeToPNG();

      HTTPMultiPartForm form = new HTTPMultiPartForm();
      form.AddBinaryData("image_file", bytes);
      // form.AddField("size", "full");
      form.AddField("size", "auto");
      form.AddField("type", "person");

      HTTPRequest request = new HTTPRequest(
        new Uri(removebgAPI),
        HTTPMethods.Post,
        UploadPictureCallback);

      request.SetHeader("X-Api-Key", key);
      request.SetForm(form);
      request.Send();
      return;
    }

    private void UploadPictureCallback(HTTPRequest httpRequest, HTTPResponse httpResponse)
    {
      if (httpResponse == null)
      {
        Debug.Log("RemoveBG请求无响应...<color=red>[ER]</color>");
        return;
      }
      if (httpResponse.StatusCode == 200)
      {
        Debug.Log(httpResponse.DataAsTexture2D.width + " / " + httpResponse.DataAsTexture2D.height);

        string fullPath = $"{Application.streamingAssetsPath}/RemoveBG/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png";
        byte[] bytes = httpResponse.DataAsTexture2D.EncodeToPNG();
        File.WriteAllBytes(fullPath, bytes);

        downloadTexture = httpResponse.DataAsTexture2D; // 无必要

        if (onRemoveBGCompelete != null)
        {
          onRemoveBGCompelete(httpResponse.DataAsTexture2D);
        }
        Debug.Log("RemoveBG返回成功...[OK]");
      }
      else
      {
        Debug.Log($"RemoveBG返回失败,{httpResponse.StatusCode}...<color=red>[ER]</color>");
        Debug.Log(httpResponse.DataAsText);
        // UploadPhoto2RemoveBG(preuoloadTexture);
      }
      return;
    }
  }
}