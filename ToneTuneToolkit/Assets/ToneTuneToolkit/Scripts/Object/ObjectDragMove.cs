/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections;
using UnityEngine;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 物体拖拽
  /// 挂在需要拖拽的物体上
  /// 需要相机为MainCameraTag
  /// 需要碰撞器
  /// </summary>
  public class ObjectDragMove : MonoBehaviour
  {
    private Vector3 screenPosition;
    private Vector3 offset;
    private Vector3 currentScreenPosition;
    private Camera cameraCOM;

    // ==================================================

    private void Start()
    {
      if (!Camera.main)
      {
        Debug.Log("[ObjectDrag] <color=red>Cant find camera</color>...[Er]");
        enabled = false;
        return;
      }
      cameraCOM = Camera.main;
    }

    private void OnMouseDown()
    {
      DragObject();
    }

    // ==================================================

    private IEnumerator DragObject()
    {
      screenPosition = cameraCOM.WorldToScreenPoint(transform.position);
      offset = transform.position - cameraCOM.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
      while (Input.GetMouseButton(0)) // 鼠标左键拖拽
      {
        currentScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
        transform.position = cameraCOM.ScreenToWorldPoint(currentScreenPosition) + offset;
        yield return new WaitForFixedUpdate();
      }
      yield break;
    }
  }
}
