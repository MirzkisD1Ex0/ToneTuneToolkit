/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 物体拖拽旋转
  /// 挂在需要旋转的物体上
  /// 需要碰撞器
  /// </summary>
  public class ObjectDragRotate : MonoBehaviour
  {
    private float rotateSpeedFactor = 2f;

    // ==================================================

    private void OnMouseDrag()
    {
      DragObject();
    }

    // ==================================================

    private void DragObject()
    {
      transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotateSpeedFactor);
      transform.Rotate(Vector3.right * Input.GetAxis("Mouse Y") * rotateSpeedFactor);
      return;
    }
  }
}