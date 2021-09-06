/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.Mobile
{
  /// <summary>
  /// 物体在移动平台上的旋转与缩放
  ///
  /// 挂在需要拖拽的物体上
  /// 需要碰撞器
  /// </summary>
  public class ObjectRotateAndScale : MonoBehaviour
  {
    private Vector2 oldPosA;
    private Vector2 oldPosB;

    private void Update()
    {
      if (Input.GetMouseButton(0))
      {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
          if (hit.collider.tag == "Player") // 如果射线不在Tag为Player的物体上则跳过后续的步骤
          {
            if (Input.touchCount == 1)
            {
              ObjectRotate();
            }
            if (Input.touchCount == 2)
            {
              ObjectScale();
            }
          }
        }
      }
    }

    /// <summary>
    /// 单指操作
    /// 旋转
    /// </summary>
    private void ObjectRotate()
    {
      if (Input.GetTouch(0).phase == TouchPhase.Moved)
      {
        float offsetX = Input.GetAxis("Mouse X");
        float offsetY = Input.GetAxis("Mouse Y");
        transform.Rotate(new Vector3(offsetY, -offsetX, 0) * 10f, Space.World);
      }
      return;
    }

    /// <summary>
    /// 双指操作
    /// 缩放
    /// </summary>
    private void ObjectScale()
    {
      if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
      {
        Vector2 newPosA = Input.GetTouch(0).position;
        Vector2 newPosB = Input.GetTouch(1).position;
        float oldScale;
        float newScale;
        oldScale = transform.localScale.x;
        if (IsEnlarge(oldPosA, oldPosB, newPosA, newPosB))
        {
          newScale = oldScale * 1.02f;
        }
        else
        {
          newScale = oldScale / 1.02f;
        }
        transform.localScale = new Vector3(newScale, newScale, newScale);
        oldPosA = newPosA;
        oldPosB = newPosB;
      }
    }

    /// <summary>
    /// 双指触点判断
    /// 远了就是放大
    /// </summary>
    /// <param name="oldPositionA"></param>
    /// <param name="oldPositionB"></param>
    /// <param name="newPositionA"></param>
    /// <param name="newPositionB"></param>
    /// <returns></returns>
    private bool IsEnlarge(Vector2 oldPositionA, Vector2 oldPositionB, Vector2 newPositionA, Vector2 newPositionB)
    {
      float oldDistance = Vector2.Distance(oldPositionA, oldPositionB);
      float newDistance = Vector2.Distance(newPositionA, newPositionB);
      if (oldDistance < newDistance)
      {
        return true;
      }
      return false;
    }
  }
}