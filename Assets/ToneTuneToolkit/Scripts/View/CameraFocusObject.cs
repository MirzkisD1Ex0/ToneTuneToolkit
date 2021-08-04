/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.View
{
  /// <summary>
  /// 鼠标拖动控制相机环绕注视对象
  ///
  /// 推荐挂在相机上
  /// 需要相机为MainCameraTag
  /// </summary>
  public class CameraFocusObject : MonoBehaviour
  {
    public GameObject FocusObjectGO;
    public float AroundSpeed = 5f;
    public float ZoomSpeed = .2f;

    private Transform foTrC;
    private Transform cameraTrC;

    private void Start()
    {
      if (!this.FocusObjectGO)
      {
        TipTools.Error("[CameraFocusObject] " + "Cant find nessary component.");
        this.enabled = false;
        return;
      }
      if (!Camera.main)
      {
        TipTools.Error("[CameraFocusObject] " + "Camera lost.");
        this.enabled = false;
        return;
      }

      this.foTrC = this.FocusObjectGO.transform;
      this.cameraTrC = Camera.main.transform;
    }

    private void LateUpdate()
    {
      this.CameraRotate(this.cameraTrC, this.foTrC, this.AroundSpeed);
      this.CameraZoom(this.cameraTrC, this.ZoomSpeed);
      this.cameraTrC.LookAt(this.foTrC);
    }

    /// <summary>
    /// 相机围绕旋转
    /// </summary>
    private void CameraRotate(Transform cameraTrC, Transform objectTrC, float aroundSpeed)
    {
      if (Input.GetMouseButton(0)) // 左键
      {
        cameraTrC.RotateAround(objectTrC.position, Vector3.up, Input.GetAxis("Mouse X") * aroundSpeed);
        cameraTrC.RotateAround(objectTrC.position, cameraTrC.right, Input.GetAxis("Mouse Y") * -aroundSpeed);
      }
      return;
    }

    /// <summary>
    /// 相机远近缩放
    /// </summary>
    private void CameraZoom(Transform cameraTrC, float zoomSpeed)
    {
      if (Input.GetAxis("Mouse ScrollWheel") > 0)
      {
        cameraTrC.Translate(Vector3.forward * zoomSpeed);
      }
      if (Input.GetAxis("Mouse ScrollWheel") < 0)
      {
        cameraTrC.Translate(Vector3.forward * -zoomSpeed);
      }
      return;
    }
  }
}