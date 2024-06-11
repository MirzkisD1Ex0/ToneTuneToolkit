using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LonginesYogaPhotoJoy
{
  public class WebCamManager : MonoBehaviour
  {
    public static WebCamManager Instance;

    public RawImage WebCamRawImage;

    private WebCamTexture webCamTexture;
    // private string deviceName = "Logitech BRIO";
    // private string deviceName = "Camera (NVIDIA Broadcast)";
    private string deviceName = "EOS Webcam Utility";
    // private string deviceName = "OBS Virtual Camera";

    private bool isReady = false;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      InitWebcam();
      // SwitchWebcam(true);
    }

    private void OnApplicationQuit()
    {
      SwitchWebcam(false);
    }

    // ==================================================

    // private void Init()
    // {
    //   InitWebcam();
    //   return;
    // }

    private void InitWebcam()
    {
      if (WebCamTexture.devices.Length <= 0)
      {
        Debug.Log("<color=red>[WM]</color> 无可用设备");
        return;
      }

      WebCamDevice[] devices = WebCamTexture.devices;
      for (int i = 0; i < devices.Length; i++)
      {
        Debug.Log($"[WM] 设备[{i}]:{devices[i].name}");
        if (devices[i].name == deviceName)
        {
          webCamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height, 30)
          {
            wrapMode = TextureWrapMode.Clamp
          };

          WebCamRawImage.texture = webCamTexture;
          isReady = true;
          break;
        }
      }
      return;
    }



    public void SwitchWebcam(bool value)
    {
      if (webCamTexture == null || isReady == false)
      {
        return;
      }

      if (value == true)
      {
        webCamTexture.Play();
      }
      else
      {
        if (webCamTexture.isPlaying)
        {
          webCamTexture.Stop();
        }
      }
      return;
    }

  }
}