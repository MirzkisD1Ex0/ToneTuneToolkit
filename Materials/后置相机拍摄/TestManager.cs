using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

namespace MartellController
{
  public class TestManager : MonoBehaviour
  {

    public static TestManager Instance;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Update()
    {
      OrginalStreamRawImage.texture = CameraManager.GetCamTexture();
    }

    // ==================================================
    // Step 00
    // 视频流预览
    public RawImage OrginalStreamRawImage;
    public RectTransform OrginalStreamShotArea;//用来取景的ui，设置为透明的
    public GameObject GOAyayi;

    public void TakeShotOrgin()
    {
      GOAyayi.SetActive(false);
      StartCoroutine(TakeScreenshotAction(OrginalStreamShotArea));
      return;
    }

    private IEnumerator TakeScreenshotAction(RectTransform screenshotArea)
    {
      yield return new WaitForEndOfFrame();
      int width = (int)screenshotArea.rect.width;
      int height = (int)screenshotArea.rect.height;
      Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
      float leftBottomX = screenshotArea.transform.position.x + screenshotArea.rect.xMin;
      float leftBottomY = screenshotArea.transform.position.y + screenshotArea.rect.yMin;
      texture2D.ReadPixels(new Rect(leftBottomX, leftBottomY, width, height), 0, 0);
      texture2D.Apply();

      string fullPath = $"{Application.streamingAssetsPath}/OrginalPictures/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png";
      byte[] bytes = texture2D.EncodeToPNG();
      File.WriteAllBytes(fullPath, bytes);

      UpdateScreenshotPreview(texture2D);
      yield break;
    }



    // ==================================================
    // Step 01
    // 截图预览
    public RawImage ScreenshotPreviewRawImage;
    public Texture2D ScreenshotPreviewTexture2D;
    private void UpdateScreenshotPreview(Texture2D value)
    {
      // 贴图复制
      byte[] data = value.GetRawTextureData();
      ScreenshotPreviewTexture2D = new Texture2D(value.width, value.height, TextureFormat.RGBA32, false);
      ScreenshotPreviewTexture2D.LoadRawTextureData(data);
      ScreenshotPreviewTexture2D.Apply();

      ScreenshotPreviewRawImage.texture = ScreenshotPreviewTexture2D;
      GOAyayi.SetActive(true);

      NetManager.Instance.UploadPicture2RemoveBG(value); // 把图片发出去
      return;
    }

    // ==================================================
    // RemoveBG预览
    public RawImage RemoveBGPreviewRawImage;
    public Texture2D RemoveBGPreviewTexture2D;
    public RectTransform RemoveBGPreviewShotArea;//用来取景的ui，设置为透明的
    public void UpdateRemoveBGPreview(Texture2D value)
    {
      // 请勿复制
      // byte[] data = value.GetRawTextureData();
      // RemoveBGPreviewTexture2D = new Texture2D(value.width, value.height, TextureFormat.RGBA32, false);
      // RemoveBGPreviewTexture2D.LoadRawTextureData(data);
      // RemoveBGPreviewTexture2D.Apply();
      RemoveBGPreviewTexture2D = value;
      RemoveBGPreviewRawImage.texture = value;

      StartCoroutine(TakeMixshotAction(RemoveBGPreviewShotArea));
      return;
    }
    private IEnumerator TakeMixshotAction(RectTransform screenshotArea)
    {
      yield return new WaitForEndOfFrame();
      int width = (int)screenshotArea.rect.width;
      int height = (int)screenshotArea.rect.height;
      Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
      float leftBottomX = screenshotArea.transform.position.x + screenshotArea.rect.xMin;
      float leftBottomY = screenshotArea.transform.position.y + screenshotArea.rect.yMin;
      texture2D.ReadPixels(new Rect(leftBottomX, leftBottomY, width, height), 0, 0);
      texture2D.Apply();

      string fullPath = $"{Application.streamingAssetsPath}/MixPictures/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png";
      byte[] bytes = texture2D.EncodeToPNG();
      File.WriteAllBytes(fullPath, bytes);
      yield break;
    }


    // ==================================================

  }
}