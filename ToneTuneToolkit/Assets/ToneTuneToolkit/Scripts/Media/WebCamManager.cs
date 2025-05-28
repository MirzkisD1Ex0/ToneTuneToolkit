/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 相机管理器
/// 并在相机初始化后返回贴图
/// </summary>
public class WebCamManager : SingletonMaster<WebCamManager>
{
  public static UnityAction OnWebCamInitComplete;

  [SerializeField] private RawImage DEBUG_PreviewRawImage;

  private string cameraName = "OBSBOT Virtual Camera";
  private int cameraWidth = 1280;
  private int cameraHeight = 720;
  private int cameraFPS = 60;

  private static WebCamTexture webCamTexture;

  // ==================================================

  private void Start() => Init();
  private void OnDestroy() => UnInit();

  // ==================================================

  private void Init()
  {
    PrintAllDevices();
    RequestCameraAuthorization();
    return;
  }

  private void UnInit()
  {
    webCamTexture.Stop();
    return;
  }

  // ==================================================
  #region 获取摄像头使用权限

  private void RequestCameraAuthorization() => StartCoroutine(nameof(RequestCameraAuthorizationAction));
  private IEnumerator RequestCameraAuthorizationAction()
  {
    yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

    if (Application.HasUserAuthorization(UserAuthorization.WebCam))
    {
      Debug.Log("[WCM] 已获取摄像头权限");
      InitWebCamera();
    }
    else
    {
      Debug.Log("[WCM] 无法获取摄像头权限");
      RequestCameraAuthorization();
    }
    yield break;
  }

  #endregion
  // ==================================================
  #region 创建摄像头

  private static bool isWebCameraCreated = false;
  public void InitWebCamera()
  {
    if (WebCamTexture.devices.Length <= 0)
    {
      Debug.Log("[WCM] 设备无可用摄像头");
      return;
    }


    WebCamDevice[] devices = WebCamTexture.devices;
    WebCamDevice device = devices[0];

#if UNITY_EDITOR // 编辑器使用罗技 // 或笔记本前置
    foreach (WebCamDevice item in devices)
    {
      if (item.name == cameraName)
      {
        device = item;
        break;
      }
    }
    webCamTexture = new WebCamTexture(device.name, cameraWidth, cameraHeight, cameraFPS)
    {
      wrapMode = TextureWrapMode.Clamp
    };

#else // IOS 0=后置相机 1=前置相机
    device = devices[1];
    webCamTexture = new WebCamTexture(device.name)
    {
      wrapMode = TextureWrapMode.Clamp
    };
    // webCamTexture = new WebCamTexture(device.name, cameraWidth, cameraHeight, cameraFPS);
#endif
    webCamTexture.Play();
    isWebCameraCreated = true;
    Debug.Log($"[WCM] 摄像头初始化完成完成 Name :{device.name} / Width:{webCamTexture.width} / Height:{webCamTexture.height} / FPS:{webCamTexture.requestedFPS}");

    if (DEBUG_PreviewRawImage) // Preview
    {
      DEBUG_PreviewRawImage.texture = webCamTexture;
    }
    return;
  }

  #endregion
  // ==================================================

  /// <summary>
  /// 返回相机贴图
  /// </summary>
  /// <returns></returns>
  public static Texture GetCamTexture()
  {
    if (isWebCameraCreated)
    {
      return webCamTexture;
    }
    else
    {
      return null;
    }
  }

  // ==================================================
  #region 状态控制

  public void StartWebcam()
  {
    if (isWebCameraCreated)
    {
      webCamTexture.Play();
    }
    return;
  }

  public void PauseWebcam()
  {
    if (isWebCameraCreated)
    {
      webCamTexture.Pause();
    }
    return;
  }

  public void StopWebcam()
  {
    if (isWebCameraCreated)
    {
      webCamTexture.Stop();
    }
    return;
  }

  #endregion
  // ==================================================

  private void PrintAllDevices()
  {
    foreach (WebCamDevice item in WebCamTexture.devices)
    {
      Debug.Log($"[WCM] 找到摄像头: {item.name}");
    }
    return;
  }
}