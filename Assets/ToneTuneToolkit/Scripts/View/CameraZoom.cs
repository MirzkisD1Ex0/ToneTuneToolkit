/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.View
{
  /// <summary>
  /// 相机POV多层级缩放
  ///
  /// 推荐挂在相机上
  /// 但挂在其它地方也无所谓
  /// </summary>
  public class CameraZoom : MonoBehaviour
  {
    public int[] ZoomLevels = new int[] { 60, 40, 20 };
    public float ZoomSpeed = .05f;

    private int index = 0;
    private UnityEngine.Camera cameraCaC;

    private void Start()
    {
      if (!Camera.main)
      {
        TipTools.Error("[CameraZoom] " + "Cant find Camera.");
        this.enabled = false;
        return;
      }
      cameraCaC = Camera.main;
    }

    private void Update()
    {
      this.Zoom(cameraCaC, ZoomSpeed);
    }

    /// <summary>
    /// 相机缩放
    /// </summary>
    /// <param name="cameraObject">主相机</param>
    private void Zoom(Camera cameraObject, float zoomSpeed)
    {
      if (Input.GetMouseButtonDown(1))
      {
        index = index < ZoomLevels.Length - 1 ? index + 1 : 0;
      }
      cameraObject.fieldOfView = Mathf.Lerp(cameraObject.fieldOfView, this.ZoomLevels[index], zoomSpeed);
      if (Mathf.Abs(cameraObject.fieldOfView - this.ZoomLevels[index]) <= zoomSpeed)
      {
        cameraObject.fieldOfView = this.ZoomLevels[index];
      }
    }
  }
}