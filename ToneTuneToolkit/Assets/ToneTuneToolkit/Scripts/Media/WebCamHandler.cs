/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

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

  private void Start() => Init();
  // private void OnDestory() => UnInit();

  // ==================================================

  private void Init()
  {
    // InitWebcamByName();
    InitWebcamByIndex();
    StartWebcam();
  }

  private void UnInit()
  {
    // StopWebcam();
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

  private int _webCamIndex = 0;

  #endregion
  // ==================================================

  public void InitWebcamByName()
  {
    WebCamDevice[] devices = WebCamTexture.devices;
    if (devices.Length <= 0) { Debug.LogError("[WCH] 无可用设备"); return; }
    foreach (WebCamDevice device in devices) { Debug.Log(@$"[WCH] Find device: {device.name}"); }

    for (int i = 0; i < devices.Length; i++)
    {
      if (devices[i].name == _webCamName)
      {
        webCamDevice = devices[i];
        webCamTexture = new WebCamTexture(webCamDevice.name, _webCamWidth, _webCamHeight, _webCamFPS)
        { wrapMode = TextureWrapMode.Clamp };
        isWebCamReady = true;

        if (riPreviews.Count > 0) { foreach (RawImage ri in riPreviews) { ri.texture = webCamTexture; } }
        break;
      }
    }
  }

  public void InitWebcamByIndex()
  {
    WebCamDevice[] devices = WebCamTexture.devices;
    if (devices.Length <= 0) { Debug.LogError("[WCH] 无可用设备"); return; }
    foreach (WebCamDevice device in devices) { Debug.Log(@$"[WCH] Find device: {device.name}"); }

#if UNITY_EDITOR
    _webCamIndex = 2; // 测试是2
#else
    _webCamIndex = 0; // ipad摄像头是devices[0].name
#endif

    webCamTexture = new WebCamTexture(devices[_webCamIndex].name, Screen.width, Screen.height, 30)
    { wrapMode = TextureWrapMode.Clamp }; isWebCamReady = true;

    if (riPreviews.Count > 0)
    {
      foreach (RawImage ri in riPreviews) { ri.texture = webCamTexture; }
    }
  }

  // ==================================================

  public WebCamTexture GetWebcamTexture()
  {
    if (isWebCamReady) { return webCamTexture; }
    else { return null; }
  }

  // ==================================================
  #region Statue Control

  public void StartWebcam() { if (isWebCamReady) { webCamTexture.Play(); } }
  public void PauseWebcam() { if (isWebCamReady) { webCamTexture.Pause(); } }
  public void StopWebcam() { if (isWebCamReady) { webCamTexture.Stop(); } }

  #endregion
}
