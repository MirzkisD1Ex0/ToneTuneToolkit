/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamHandler : MonoBehaviour
{
  public static WebCamHandler Instance;

  [SerializeField] private RawImage previewRawImage;

  private WebCamDevice _webCamDevice;
  private WebCamTexture _webCamTexture;
  private bool _isWebCamReady = false;

  // ==================================================

  private void Awake() => Instance = this;
  // private void Start()
  // {
  //   InitWebcam();
  //   StartWebcam();
  // }

  // ==================================================
  #region 相机配置

  private string _webCamName = "MX Brio";
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
        _webCamDevice = device;
        _webCamTexture = new WebCamTexture(_webCamDevice.name, _webCamWidth, _webCamHeight, _webCamFPS);
        // _webCamTexture.Play();
        _isWebCamReady = true;

        if (previewRawImage) // Preview
        {
          previewRawImage.texture = _webCamTexture;
        }
        break;
      }
    }
    return;
  }

  public WebCamTexture GetWebcamTexture()
  {
    if (_isWebCamReady)
    {
      return _webCamTexture;
    }
    else
    {
      return null;
    }
  }

  public void StartWebcam()
  {
    if (_isWebCamReady)
    {
      _webCamTexture.Play();
    }
    return;
  }

  public void PauseWebcam()
  {
    if (_isWebCamReady)
    {
      _webCamTexture.Pause();
    }
    return;
  }

  public void StopWebcam()
  {
    if (_isWebCamReady)
    {
      _webCamTexture.Stop();
    }
    return;
  }
}