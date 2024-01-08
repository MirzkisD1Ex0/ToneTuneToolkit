using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using BestHTTP;
using BestHTTP.Forms;
using UnityEngine.Events;

namespace MartellController
{
  public class NetManager : MonoBehaviour
  {
    public static NetManager Instance;

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

    private void Init()
    {
      return;
    }

    // ==================================================

    public void UploadPicture2RemoveBG(Texture2D value)
    {
      byte[] bytes = value.EncodeToPNG();

      HTTPMultiPartForm form = new HTTPMultiPartForm();
      form.AddBinaryData("image_file", bytes);
      form.AddField("size", "auto");
      form.AddField("type", "person");

      HTTPRequest request = new HTTPRequest(
        new Uri("https://api.remove.bg/v1.0/removebg"),
        HTTPMethods.Post,
        UploadPictureCallback);
      // 测试key X859F9v3g4YpoPBRQe2n7h8T
      // 正式key 76YHaSA8WZYmbZXfqfBeYbqy
      request.SetHeader("X-Api-Key", "X859F9v3g4YpoPBRQe2n7h8T");
      request.SetForm(form);
      request.Send();
      return;
    }

    private void UploadPictureCallback(HTTPRequest httpRequest, HTTPResponse httpResponse)
    {
      if (httpResponse == null)
      {
        LogManager.Log("RemoveBG请求无响应...[Error]");
        return;
      }
      if (httpResponse.StatusCode == 200)
      {
        LogManager.Log("RemoveBG返回成功");
        LogManager.Log(httpResponse.DataAsTexture2D.width + " / " + httpResponse.DataAsTexture2D.height);

        string fullPath = $"{Application.streamingAssetsPath}/RemoveBGPictures/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png";
        byte[] bytes = httpResponse.DataAsTexture2D.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);

        TestManager.Instance.UpdateRemoveBGPreview(httpResponse.DataAsTexture2D);
      }
      else
      {
        LogManager.Log($"RemoveBG返回失败,{httpResponse.StatusCode}");
        LogManager.Log(httpResponse.DataAsText);
      }
      return;
    }
  }
}