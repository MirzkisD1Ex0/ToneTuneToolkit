/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
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
    private Transform cameraTrC;

    private void Start()
    {
      cameraTrC = Camera.main.transform;
    }

    private void Update()
    {
      SimpleMove();
    }

    private void SimpleMove()
    {
      if (Input.GetKey(KeyCode.W))
      {
        cameraTrC.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.Self);
      }
      else if (Input.GetKey(KeyCode.S))
      {
        cameraTrC.Translate(Vector3.back * Time.deltaTime * moveSpeed, Space.Self);
      }

      if (Input.GetKey(KeyCode.D))
      {
        cameraTrC.Translate(Vector3.right * Time.deltaTime * moveSpeed, Space.Self);
      }
      else if (Input.GetKey(KeyCode.A))
      {
        cameraTrC.Translate(Vector3.left * Time.deltaTime * moveSpeed, Space.Self);
      }

      if (Input.GetKey(KeyCode.Space))
      {
        cameraTrC.Translate(Vector3.up * Time.deltaTime * moveSpeed, Space.World);
      }
      else if (Input.GetKey(KeyCode.LeftShift))
      {
        cameraTrC.Translate(Vector3.down * Time.deltaTime * moveSpeed, Space.World);
      }

      float mouseX = Input.GetAxis("Mouse X");
      float mouseY = Input.GetAxis("Mouse Y");
      if (Input.GetMouseButton(1))
      {
        cameraTrC.Rotate(new Vector3(0, mouseX, 0) * sensitivity, Space.World);
        cameraTrC.Rotate(new Vector3(-mouseY, 0, 0) * sensitivity, Space.Self);
      }
      return;
    }
  }
}