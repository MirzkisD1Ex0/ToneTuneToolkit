using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MartellGroupPhoto;

public class CamFiTestManager : MonoBehaviour
{


  // ==================================================

  private void Start()
  {
    Init();
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Q))
    {
      CamFiRESTManager.Instance.CamFiTakePicture(); // 拍照
    }
    // if (Input.GetKeyDown(KeyCode.W))
    // {
    //   CamFiRESTManager.Instance.CamFiGetFileList();
    // }
  }

  private void OnApplicationQuit()
  {
    UnInit();
  }

  // ==================================================

  private void Init()
  {
    CamFiSocketManager.Instance.OnConnected += StartLiveView;
    CamFiSocketManager.Instance.OnFileAdded += GetNewestPicName;
    CamFiRESTManager.Instance.OnGetRawFile += GetNewestPicTexture2D;
    return;
  }

  private void UnInit()
  {
    CamFiSocketManager.Instance.OnConnected -= StartLiveView;
    CamFiSocketManager.Instance.OnFileAdded -= GetNewestPicName;
    CamFiRESTManager.Instance.OnGetRawFile -= GetNewestPicTexture2D;
    return;
  }

  // ==================================================

  private void StartLiveView()
  {
    CamFiRESTManager.Instance.CamFiLiveView(true);
    return;
  }

  private void GetNewestPicName(string lastFile)
  {
    CamFiRESTManager.Instance.CamFiGetRawFile(lastFile);
    return;
  }

  public Image PreviewImage;
  private void GetNewestPicTexture2D(Texture2D texture2D)
  {
    PreviewImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one);
    return;
  }
}