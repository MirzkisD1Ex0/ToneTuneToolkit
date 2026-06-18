/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Best.HTTP;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.IO.CamFi
{
  /// <summary>
  /// 指令发送方
  /// </summary>
  public class RESTSender : SingletonMaster<RESTSender>
  {
    private const float TIMEOUT_SECONDS = 10f;

    public static UnityAction<string> OnCamFiError;
    public static UnityAction OnTakePicture;
    public static UnityAction<string> OnGetFileList;
    public static UnityAction<Texture2D> OnGetRawFile;
    public static UnityAction<bool> OnLiveView;

    // ==================================================

    private void Start() => Init();

    // ==================================================

    private void Init()
    {
      StartCoroutine(KeepCameraAlive());
    }

    // ==================================================
    #region Public API

    public void CamFiTakePicture()
    {
      SendGET(Configer.TakePic, "拍照", res => OnTakePicture?.Invoke());
    }

    public void CamFiGetRawFile(string fileName)
    {
      SendGET(Configer.GetRawFile + fileName.Replace("/", "%2F"),
        "取图",
        res => OnGetRawFile?.Invoke(res.DataAsTexture2D));
    }

    public void CamFiGetFileList() // http://192.168.50.101/raw/%2Fstore_00020001%2FDCIM%2F100EOS5D%2F6A0A7754.JPG
    {
      SendGET(Configer.GetFileList, "取列", res =>
      {
        string text = res.DataAsText.Replace("[", "").Replace("]", "").Replace("\"", "");
        string[] list = text.Split(",");
        foreach (string name in list) Debug.Log($"[CamFi M] {name}...[Debug]");
        OnGetFileList?.Invoke(list[list.Length - 1]);
      });
    }

    public void CamFiLiveView(bool value)
    {
      string liveViewState = value ? Configer.StartLiveView : Configer.StopLiveView;
      SendGET(liveViewState, "取景", res =>
      {
        LiveViewer.Instance.SwitchLiveView(true);
        OnLiveView?.Invoke(true);
      });
    }

    #endregion
    // ==================================================
    #region Sender

    private void SendGET(string url, string actionName, Action<HTTPResponse> onSuccess)
    {
      HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Get,
        (HTTPRequest req, HTTPResponse res) => HandleResponse(req, res, actionName, onSuccess));
      request.TimeoutSettings.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
      request.Send();
    }

    private void HandleResponse(HTTPRequest req, HTTPResponse res, string actionName, Action<HTTPResponse> onSuccess)
    {
      if (res == null)
      {
        Debug.LogWarning($"[CamFi M] {actionName} 请求无响应: {req.Exception?.Message}");
        OnCamFiError?.Invoke(actionName);
        return;
      }
      if (res.StatusCode == 200)
      {
        Debug.Log($"<color=green>[CamFi M]</color> {actionName} 成功...[OK]");
        onSuccess?.Invoke(res);
      }
      else
      {
        Debug.LogWarning($"[CamFi M] {actionName} 失败: {res.StatusCode}");
        OnCamFiError?.Invoke(actionName);
      }
    }

    #endregion
    // ==================================================
    #region Heartbeat

    private const float KEEP_ALIVE_INTERVAL = 60f;
    private static readonly WaitForSeconds WaitForKeepAlive = new WaitForSeconds(KEEP_ALIVE_INTERVAL);

    private IEnumerator KeepCameraAlive()
    {
      while (true)
      {
        HTTPRequest httpRequest = new HTTPRequest(
          new Uri(Configer.Live),
          HTTPMethods.Get);
        httpRequest.Send();
        yield return WaitForKeepAlive;
      }
    }

    #endregion
  }
}
