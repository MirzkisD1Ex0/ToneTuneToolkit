/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
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
      // ObjectAngleYLimit();
      return;
    }

    private void ObjectAngleYLimit()
    {
      float angleY = CheckAngle(transform.eulerAngles.y);
      if (angleY <= -70f)
      {
        angleY = -70f;
      }
      else if (angleY >= 70f)
      {
        angleY = 70f;
      }
      transform.eulerAngles = new Vector3(transform.eulerAngles.x, angleY, transform.eulerAngles.z);
      return;
    }

    private float CheckAngle(float value)
    {
      float angle = value - 180f;
      if (angle > 0)
      {
        return angle - 180;
      }
      else
      {
        return angle + 180;
      }
    }
  }
}
