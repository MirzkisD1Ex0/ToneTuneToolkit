using OpenCVForUnity;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiageoWhiskyBlending
{
  public class WebCamTextureToMat : MonoBehaviour
  {
    private WebCamDevice webCamDevice;
    private WebCamTexture webCamTexture;
    private Mat rgbaMat;

    private Color32[] colors;
    private bool isInitWaiting = false;
    private bool hasInitDone = false; // 如果Init完毕

    // ==================================================

    private void Start()
    {
      Init();
    }

    private void Update()
    {
      if (hasInitDone && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
      {
        Utils.webCamTextureToMat(webCamTexture, rgbaMat, colors);
        transform.GetComponent<FaceDetecter>().DetectFace(rgbaMat); // 传入mat 检测人脸 // 会导致原数据反转？

        // UpdatePreview();
      }
    }

    private void OnDestroy()
    {
      Dispose();
    }

    private void OnApplicationQuit()
    {
      Dispose();
    }

    // ==================================================

    private void Init()
    {
      if (isInitWaiting)
      {
        return;
      }
      StartCoroutine(InitAction());
      return;
    }
    private IEnumerator InitAction()
    {
      if (hasInitDone) // 防止重复安装
      {
        Dispose();
      }
      isInitWaiting = true;

      CreateCamera(); // 创建相机

      while (true)
      {
        if (webCamTexture.didUpdateThisFrame)
        {
          Debug.Log($"Name:{webCamTexture.deviceName} / Width:{webCamTexture.width} / Height:{webCamTexture.height} / FPS:{webCamTexture.requestedFPS}...<color=green>[OK]</color>");
          Debug.Log($"VideoRotationAngle:{webCamTexture.videoRotationAngle} / VideoVerticallyMirrored:{webCamTexture.videoVerticallyMirrored} / IsFrongFacing:{webCamDevice.isFrontFacing}...<color=green>[OK]</color>");

          isInitWaiting = false;
          hasInitDone = true;

          if (colors == null || colors.Length != webCamTexture.width * webCamTexture.height) // 确定color尺寸
          {
            colors = new Color32[webCamTexture.width * webCamTexture.height];
          }
          if (orginalPreviewTexture2D == null || orginalPreviewTexture2D.width != webCamTexture.width || orginalPreviewTexture2D.height != webCamTexture.height) // 确定texture2d尺寸
          {
            orginalPreviewTexture2D = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
          }

          rgbaMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255)); // 高、宽

          // UpdatePreview();
          break;
        }
        else
        {
          yield return null;
        }
      }
      yield break;
    }

    // ==================================================
    // 释放资源
    // DONE
    private void Dispose()
    {
      isInitWaiting = false;
      hasInitDone = false;

      if (webCamTexture != null)
      {
        webCamTexture.Stop();
        Destroy(webCamTexture);
        webCamTexture = null;
      }
      if (rgbaMat != null)
      {
        rgbaMat.Dispose();
        rgbaMat = null;
      }
      if (orginalPreviewTexture2D != null)
      {
        Destroy(orginalPreviewTexture2D);
        orginalPreviewTexture2D = null;
      }
      return;
    }

    // ==================================================
    // 按钮

    public void OnPlayButtonClick()
    {
      if (hasInitDone)
      {
        webCamTexture.Play();
      }
      return;
    }

    public void OnPauseButtonClick()
    {
      if (hasInitDone)
      {
        webCamTexture.Pause();
      }
      return;
    }

    public void OnStopButtonClick()
    {
      if (hasInitDone)
      {
        webCamTexture.Stop();
      }
      return;
    }

    // ==================================================
    // 相机配置
    // DONE
    private string requestedDeviceName = "Logitech BRIO";
    // private string requestedDeviceName = "GC21 Video";
    private int requestedWidth = 440;
    private int requestedHeight = 440;
    private int requestedFPS = 30;
    private void CreateCamera()
    {
      foreach (WebCamDevice device in WebCamTexture.devices)
      {
        if (device.name == requestedDeviceName)
        {
          webCamDevice = device;
          webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
          webCamTexture.Play();
        }
      }
      return;
    }

    // ==================================================
    // 预览画面

    public Image ImageOrginalPreview;
    private Texture2D orginalPreviewTexture2D;
    private void UpdatePreview()
    {
      if (!ImageOrginalPreview)
      {
        return;
      }

      Utils.matToTexture2D(rgbaMat, orginalPreviewTexture2D, colors); // Mat更新为texture2d

      // texture2D = RotateTexutre(texture2D, false);
      ImageOrginalPreview.sprite = Sprite.Create(orginalPreviewTexture2D, new UnityEngine.Rect(0, 0, orginalPreviewTexture2D.width, orginalPreviewTexture2D.height), Vector2.zero);
      return;
    }

    // ==================================================
    //  Texture画面旋转
    // private Texture2D RotateTexutre(Texture2D originalTexture, bool clockwise)
    // {
    //   Color32[] original = originalTexture.GetPixels32();
    //   Color32[] rotated = new Color32[original.Length];
    //   int w = originalTexture.width;
    //   int h = originalTexture.height;

    //   int iRotated, iOriginal;

    //   for (int j = 0; j < h; ++j)
    //   {
    //     for (int i = 0; i < w; ++i)
    //     {
    //       iRotated = (i + 1) * h - j - 1;
    //       iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
    //       rotated[iRotated] = original[iOriginal];
    //     }
    //   }

    //   Texture2D newTexture = new Texture2D(originalTexture.height, originalTexture.width, TextureFormat.RGBA32, false);
    //   newTexture.SetPixels32(rotated);
    //   newTexture.Apply();
    //   return newTexture;
    // }
  }
}