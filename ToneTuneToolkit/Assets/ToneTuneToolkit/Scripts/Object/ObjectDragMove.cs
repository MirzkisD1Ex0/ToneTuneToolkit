/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Collections;
using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 物体拖拽
  ///
  /// 挂在需要拖拽的物体上
  /// 需要相机为MainCameraTag
  /// 需要碰撞器
  /// </summary>
  public class ObjectDragMove : MonoBehaviour
  {
    private Vector3 screenPosition;
    private Vector3 offset;
    private Vector3 currentScreenPosition;
    private Camera cameraCaC;

    private void Start()
    {
      if (!Camera.main)
      {
        TipTools.Error("[ObjectDrag] " + "Cant find Camera.");
        enabled = false;
        return;
      }
      cameraCaC = Camera.main;
    }

    private IEnumerator OnMouseDown()
    {
      screenPosition = cameraCaC.WorldToScreenPoint(transform.position);
      offset = transform.position - cameraCaC.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
      while (Input.GetMouseButton(0)) // 鼠标左键拖拽
      {
        currentScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
        transform.position = cameraCaC.ScreenToWorldPoint(currentScreenPosition) + offset;
        yield return new WaitForFixedUpdate();
      }
    }
  }
}