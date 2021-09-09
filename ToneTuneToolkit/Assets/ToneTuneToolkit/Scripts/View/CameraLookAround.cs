/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.View
{
  /// <summary>
  /// 鼠标拖动控制相机旋转
  ///
  /// 推荐挂在相机上
  /// 需要设置MainCameraTag
  /// 如果是为了实现全景效果，建议减少球模型的面数，此外还需要在建模软件内将模型的法线翻转至球的内侧
  /// 或者使用Sphere + Panoramic材质 // 更省事
  /// </summary>
  public class CameraLookAround : MonoBehaviour
  {
    // [Range(0, 600)]
    public float ViewSpeed = 600; // 旋转速度

    private float xValue;
    private float yValue;
    private Vector3 rotationValue = Vector3.zero;
    private Transform cameraTrC;

    private void Start()
    {
      if (!Camera.main)
      {
        TipTools.Error("[CameraLookAround] " + "Cant find Camera.");
        enabled = false;
        return;
      }
      cameraTrC = Camera.main.transform;
    }

    private void Update()
    {
      RotateViewTrigger();
    }

    private void RotateViewTrigger()
    {
      if (Input.GetMouseButton(0)) // 按住鼠标左键
      {
        xValue = Input.GetAxis("Mouse X");
        yValue = Input.GetAxis("Mouse Y");
        if (xValue != 0 || yValue != 0) // 并且鼠标移动
        {
          // transform.Rotate(0, xValue * speed * Time.deltaTime, 0, Space.World);
          // transform.Rotate(yValue * -speed * Time.deltaTime, 0, 0, Space.Self); // 左右
          RotateView(cameraTrC, xValue, yValue, ViewSpeed);
        }
      }
      return;
    }

    /// <summary>
    /// 视角滚动
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void RotateView(Transform objectTrC, float x, float y, float speed)
    {
      rotationValue.x += y * Time.deltaTime * speed;
      rotationValue.y += x * Time.deltaTime * -speed;
      if (rotationValue.x > 90) // 矫正
      {
        rotationValue.x = 90;
      }
      else if (rotationValue.x < -90)
      {
        rotationValue.x = -90;
      }
      objectTrC.rotation = Quaternion.Euler(rotationValue);
      return;
    }
  }
}