using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MartellController
{
  /// <summary>
  /// 相机管理器
  /// 并在相机初始化后返回贴图
  /// </summary>
  public class CameraManager : MonoBehaviour
  {
    public static CameraManager Instance;

    private WebCamDevice iosCamDevice;
    private static WebCamTexture iosCamTexture;

    private string cameraName = "Logitech BRIO";
    private int cameraWidth;
    private int cameraHeight;
    private int cameraFPS = 60;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      Init();
    }

    private void OnApplicationQuit()
    {
      UnInit();
    }

    // ==================================================

    private void Init()
    {
      StartCoroutine("RequestCameraAuthorization");
      return;
    }

    private void UnInit()
    {
      iosCamTexture.Stop();
      return;
    }



    /// <summary>
    /// 获取相机使用权限
    /// </summary>
    /// <returns></returns>
    private IEnumerator RequestCameraAuthorization()
    {
      yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

      if (Application.HasUserAuthorization(UserAuthorization.WebCam))
      {
        LogManager.Log("已获取相机权限");
        CreateCamera();
      }
      else
      {
        LogManager.ErrorLog("无法获取相机权限");
        StartCoroutine("RequestCameraAuthorization");
      }
      yield break;
    }


    private static bool isCameraCreated = false;
    private void CreateCamera()
    {
      if (WebCamTexture.devices.Length <= 0)
      {
        LogManager.ErrorLog("设备无可用相机");
        return;
      }



#if UNITY_EDITOR // 编辑器使用罗技
      foreach (WebCamDevice device in WebCamTexture.devices)
      {
        if (device.name == cameraName)
        {
          iosCamDevice = device;
        }
      }
      iosCamTexture = new WebCamTexture(iosCamDevice.name, cameraWidth, cameraHeight, cameraFPS);
#else // IOS使用0号相机
    iosCamDevice = WebCamTexture.devices[0];
    iosCamTexture = new WebCamTexture(iosCamDevice.name);
#endif
      iosCamTexture.Play();
      isCameraCreated = true;
      LogManager.Log($"Name :{iosCamTexture.deviceName} / Width:{iosCamTexture.width} / Height:{iosCamTexture.height} / FPS:{iosCamTexture.requestedFPS}");
      LogManager.Log("相机初始化完成");
      return;
    }

    /// <summary>
    /// 返回相机贴图
    /// </summary>
    /// <returns></returns>
    public static Texture GetCamTexture()
    {
      if (isCameraCreated)
      {
        return iosCamTexture;
      }
      else
      {
        return null;
      }
    }
  }
}