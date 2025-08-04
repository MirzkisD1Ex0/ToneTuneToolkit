/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit.View
{
  /// <summary>
  /// WSDA空格Shift鼠标右键控制漫游
  /// 推荐挂在相机上
  /// </summary>
  public class CameraSimpleMove : MonoBehaviour
  {
    private float sensitivity = 2f;
    private float moveSpeed = 3f;
    private Transform cameraTransformCOM;

    // ==================================================

    private void Start()
    {
      cameraTransformCOM = Camera.main.transform;
    }

    private void Update()
    {
      SimpleMove();
    }

    // ==================================================

    private void SimpleMove()
    {
      if (Input.GetKey(KeyCode.W))
      {
        cameraTransformCOM.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.Self);
      }
      else if (Input.GetKey(KeyCode.S))
      {
        cameraTransformCOM.Translate(Vector3.back * Time.deltaTime * moveSpeed, Space.Self);
      }

      if (Input.GetKey(KeyCode.D))
      {
        cameraTransformCOM.Translate(Vector3.right * Time.deltaTime * moveSpeed, Space.Self);
      }
      else if (Input.GetKey(KeyCode.A))
      {
        cameraTransformCOM.Translate(Vector3.left * Time.deltaTime * moveSpeed, Space.Self);
      }

      if (Input.GetKey(KeyCode.Space))
      {
        cameraTransformCOM.Translate(Vector3.up * Time.deltaTime * moveSpeed, Space.World);
      }
      else if (Input.GetKey(KeyCode.LeftShift))
      {
        cameraTransformCOM.Translate(Vector3.down * Time.deltaTime * moveSpeed, Space.World);
      }

      float mouseX = Input.GetAxis("Mouse X");
      float mouseY = Input.GetAxis("Mouse Y");
      if (Input.GetMouseButton(1))
      {
        cameraTransformCOM.Rotate(new Vector3(0, mouseX, 0) * sensitivity, Space.World);
        cameraTransformCOM.Rotate(new Vector3(-mouseY, 0, 0) * sensitivity, Space.Self);
      }
      return;
    }
  }
}
