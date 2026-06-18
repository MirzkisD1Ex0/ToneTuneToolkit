/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ToneTuneToolkit.IO.CamFi
{
  public class Tester : MonoBehaviour
  {
    // ==================================================

    private void Start() => Init();

    private void OnDestroy() => UnInit();

    private void OnGUI()
    {
      if (GUILayout.Button("CamFiTakePicture", GUILayout.Width(200), GUILayout.Height(40))) { RESTSender.Instance.CamFiTakePicture(); }
      if (GUILayout.Button("CamFiTakePicture", GUILayout.Width(200), GUILayout.Height(40))) { RESTSender.Instance.CamFiGetFileList(); }
    }

    // ==================================================

    private void Init()
    {
      SocketListener.OnCamFiConnected += StartLiveView;
      SocketListener.OnCamFiFileAdded += GetNewestPicName;
      RESTSender.OnGetRawFile += GetNewestPicTexture2D;
    }

    private void UnInit()
    {
      SocketListener.OnCamFiConnected -= StartLiveView;
      SocketListener.OnCamFiFileAdded -= GetNewestPicName;
      RESTSender.OnGetRawFile -= GetNewestPicTexture2D;
    }

    // ==================================================

    private void StartLiveView()
    {
      RESTSender.Instance.CamFiLiveView(true);
    }

    private void GetNewestPicName(string lastFile)
    {
      RESTSender.Instance.CamFiGetRawFile(lastFile);
    }

    public Image PreviewImage;
    private void GetNewestPicTexture2D(Texture2D texture2D)
    {
      PreviewImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one);
    }
  }
}
