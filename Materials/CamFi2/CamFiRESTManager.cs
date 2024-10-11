using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using BestHTTP;
using UnityEngine.Events;

namespace MartellGroupPhoto
{
  /// <summary>
  /// CamFiREST控制模块
  /// 记得退订事件
  /// </summary>
  public class CamFiRESTManager : MonoBehaviour
  {
    public static CamFiRESTManager Instance;

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
      StartCoroutine("KeepCameraAlive");
      return;
    }

    // ==================================================
    // 拍照
    public event UnityAction OnTakePicture;
    public void CamFiTakePicture()
    {
      HTTPRequest request = new HTTPRequest(
        new Uri(CamFiStorage.TakePic),
        HTTPMethods.Get,
        TakePictureCallback);
      request.Send();
      return;
    }
    private void TakePictureCallback(HTTPRequest httpRequest, HTTPResponse httpResponse)
    {
      if (httpResponse == null)
      {
        Debug.Log("[CamFiManager]请求无响应...[Error]");
        return;
      }
      if (httpResponse.StatusCode == 200)
      {
        Debug.Log("[CamFiManager]拍照成功...[Done]");
        if (OnTakePicture != null)
        {
          OnTakePicture();
        }
      }
      else
      {
        Debug.Log("[CamFiManager]拍照失败...[Done]");
      }
      return;
    }

    // ==================================================
    // 查询照片列表
    // 通常会获取最新照片index-1
    // 请勿使用
    public event UnityAction<string> OnGetFileList;
    public void CamFiGetFileList()
    {
      HTTPRequest request = new HTTPRequest(
        new Uri(CamFiStorage.GetFileList),
        HTTPMethods.Get,
        GetFileListCallback);
      request.Send();
      return;
    }
    private void GetFileListCallback(HTTPRequest httpRequest, HTTPResponse httpResponse)
    {
      if (httpResponse == null)
      {
        Debug.Log("[CamFiManager]请求无响应...[Error]");
        return;
      }
      if (httpResponse.StatusCode == 200)
      {
        string pictureListText = httpResponse.DataAsText.Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty);
        string[] pictureList = pictureListText.Split(",");

        foreach (string pictureName in pictureList)
        {
          Debug.Log($"[CamFiManager]{pictureName}...[Debug]");
        }

        Debug.Log("[CamFiManager]获取照片列表成功...[Done]");
        if (OnGetFileList != null) // 从订阅传出最新照片
        {
          OnGetFileList(pictureList[pictureList.Length - 1]);
        }
      }
      else
      {
        Debug.Log("[CamFiManager]获取照片列表失败...[Done]");
      }
      return;
    }

    // ==================================================
    // 获取照片原图
    public event UnityAction<Texture2D> OnGetRawFile;
    public void CamFiGetRawFile(string fileName)
    {
      fileName = fileName.Replace("/", "%2F");
      HTTPRequest request = new HTTPRequest(
        new Uri(CamFiStorage.GetRawFile + fileName),
        HTTPMethods.Get,
        GetRawFileCallback);
      request.Send();
      return;
    }
    private void GetRawFileCallback(HTTPRequest httpRequest, HTTPResponse httpResponse)
    {
      if (httpResponse == null)
      {
        Debug.Log("[CamFiManager]请求无响应...[Error]");
        return;
      }
      if (httpResponse.StatusCode == 200)
      {
        Debug.Log("[CamFiManager]获取照片成功...[Done]");
        if (OnGetRawFile != null) // 从订阅传出最新照片
        {
          OnGetRawFile(httpResponse.DataAsTexture2D);
        }
      }
      else
      {
        Debug.Log("[CamFiManager]获取照片失败...[Done]");
      }
      return;
    }

    // ==================================================
    // 实时取景
    public event UnityAction<bool> OnLiveView;
    public void CamFiLiveView(bool value)
    {
      string liveViewState;
      if (value)
      {
        liveViewState = CamFiStorage.StartLiveView;
      }
      else
      {
        liveViewState = CamFiStorage.StopLiveView;
      }
      HTTPRequest request = new HTTPRequest(
        new Uri(liveViewState),
        HTTPMethods.Get,
        LiveViewCallback);
      request.Send();
      return;
    }
    private void LiveViewCallback(HTTPRequest httpRequest, HTTPResponse httpResponse)
    {
      if (httpResponse == null)
      {
        Debug.Log("[CamFiManager]请求无响应...[Error]");
        return;
      }
      if (httpResponse.StatusCode == 200)
      {
        Debug.Log("[CamFiManager]取景视频流开启/关闭成功...[Done]");
        CamFiLiveViewManager.Instance.StartLiveView();
        if (OnLiveView != null)
        {
          OnLiveView(true);
        }
      }
      else
      {
        Debug.Log("[CamFiManager]取景视频流开启/关闭失败...[Done]");
      }
      return;
    }

    // ==================================================
    // oi，别tm睡着了
    private IEnumerator KeepCameraAlive()
    {
      HTTPRequest httpRequest = new HTTPRequest(
        new Uri(CamFiStorage.Live),
        HTTPMethods.Get);
      httpRequest.Send();
      yield return new WaitForSeconds(60f);
      StartCoroutine("KeepCameraAlive");
      yield break;
    }

    // ==================================================

    // // 参考
    // private IEnumerator KeepCameraAlive()
    // {
    //   HTTPRequest httpRequest = new HTTPRequest(
    //     new Uri($"{camFiAddress}{CamFiStorage.Live}"),
    //     HTTPMethods.Get,
    //     (HTTPRequest httpRequest, HTTPResponse httpResponse) =>
    //     {
    //       // Debug.Log(httpResponse.StatusCode);
    //     });
    //   yield return httpRequest.Send();
    //   Debug.Log(httpRequest.Response.StatusCode);
    //   yield return new WaitForSeconds(60f);
    //   StartCoroutine("KeepCameraAlive");
    //   yield break;
    // }

    // http://192.168.50.101/raw/%2Fstore_00020001%2FDCIM%2F100EOS5D%2F6A0A7754.JPG

  }
}