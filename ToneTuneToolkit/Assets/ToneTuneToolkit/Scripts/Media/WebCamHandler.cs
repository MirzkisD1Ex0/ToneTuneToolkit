/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;
using UnityEngine.UI;

public class WebCamHandler : SingletonMaster<WebCamHandler>
{
  [SerializeField] private List<RawImage> riPreviews;

  private WebCamDevice webCamDevice;
  private WebCamTexture webCamTexture;
  private bool isWebCamReady = false;

  // ==================================================

  private void Start()
  {
    InitWebcam();
    StartWebcam();
  }

  // ==================================================
  #region 相机配置

  private string _webCamName = "OBSBOT Virtual Camera";
  private int _webCamWidth = 1920;
  private int _webCamHeight = 1080;
  private int _webCamFPS = 60;

  public void SetWebcam(string name, int width, int height, int fps)
  {
    _webCamName = name;
    _webCamWidth = width;
    _webCamHeight = height;
    _webCamFPS = fps;
    return;
  }

  #endregion
  // ==================================================

  public void InitWebcam()
  {
    foreach (WebCamDevice device in WebCamTexture.devices)
    {
      Debug.Log(device.name);
      if (device.name == _webCamName)
      {
        webCamDevice = device;
        webCamTexture = new WebCamTexture(webCamDevice.name, _webCamWidth, _webCamHeight, _webCamFPS);
        // _webCamTexture.Play();
        isWebCamReady = true;

        if (riPreviews.Count > 0) // Preview
        {
          foreach (RawImage ri in riPreviews)
          {
            ri.texture = webCamTexture;
          }
        }
        break;
      }
    }
    return;
  }

  public WebCamTexture GetWebcamTexture()
  {
    if (isWebCamReady)
    {
      return webCamTexture;
    }
    else
    {
      return null;
    }
  }

  public void StartWebcam()
  {
    if (isWebCamReady)
    {
      webCamTexture.Play();
    }
    return;
  }

  public void PauseWebcam()
  {
    if (isWebCamReady)
    {
      webCamTexture.Pause();
    }
    return;
  }

  public void StopWebcam()
  {
    if (isWebCamReady)
    {
      webCamTexture.Stop();
    }
    return;
  }
}
