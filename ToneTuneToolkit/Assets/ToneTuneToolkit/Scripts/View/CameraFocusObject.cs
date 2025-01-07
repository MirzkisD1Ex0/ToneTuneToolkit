/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
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

    private Transform objectTransformCOM;
    private Transform cameraTransformCOM;

    // ==================================================

    private void Start()
    {
      if (!FocusObjectGO)
      {
        Debug.Log("[CameraFocusObject] Cant find necessary component...[Er]");
        enabled = false;
        return;
      }
      if (!Camera.main)
      {
        Debug.Log("[CameraFocusObject] Camera lost...[Er]");
        enabled = false;
        return;
      }

      Init();
    }

    private void LateUpdate()
    {
      CameraRotate(cameraTransformCOM, objectTransformCOM, AroundSpeed);
      CameraZoom(cameraTransformCOM, ZoomSpeed);
      cameraTransformCOM.LookAt(objectTransformCOM);
    }

    // ==================================================

    private void Init()
    {
      objectTransformCOM = FocusObjectGO.transform;
      cameraTransformCOM = Camera.main.transform;
      return;
    }

    /// <summary>
    /// 相机围绕旋转
    /// </summary>
    private void CameraRotate(Transform cameraTransformCOM, Transform objectTransformCOM, float aroundSpeed)
    {
      if (Input.GetMouseButton(0)) // 左键
      {
        cameraTransformCOM.RotateAround(objectTransformCOM.position, Vector3.up, Input.GetAxis("Mouse X") * aroundSpeed);
        cameraTransformCOM.RotateAround(objectTransformCOM.position, cameraTransformCOM.right, Input.GetAxis("Mouse Y") * -aroundSpeed);
      }
      return;
    }

    /// <summary>
    /// 相机远近缩放
    /// </summary>
    private void CameraZoom(Transform transformCOM, float zoomSpeed)
    {
      if (Input.GetAxis("Mouse ScrollWheel") > 0)
      {
        transformCOM.Translate(Vector3.forward * zoomSpeed);
      }
      if (Input.GetAxis("Mouse ScrollWheel") < 0)
      {
        transformCOM.Translate(Vector3.forward * -zoomSpeed);
      }
      return;
    }
  }
}
