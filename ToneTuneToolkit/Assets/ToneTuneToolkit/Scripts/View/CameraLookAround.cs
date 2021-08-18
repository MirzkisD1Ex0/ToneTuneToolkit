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
        this.enabled = false;
        return;
      }
      this.cameraTrC = Camera.main.transform;
    }

    private void Update()
    {
      this.RotateViewTrigger();
    }

    private void RotateViewTrigger()
    {
      if (Input.GetMouseButton(0)) // 按住鼠标左键
      {
        this.xValue = Input.GetAxis("Mouse X");
        this.yValue = Input.GetAxis("Mouse Y");
        if (this.xValue != 0 || this.yValue != 0) // 并且鼠标移动
        {
          // transform.Rotate(0, xValue * speed * Time.deltaTime, 0, Space.World);
          // transform.Rotate(yValue * -speed * Time.deltaTime, 0, 0, Space.Self); // 左右
          this.RotateView(this.cameraTrC, this.xValue, this.yValue, this.ViewSpeed);
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
      this.rotationValue.x += y * Time.deltaTime * speed;
      this.rotationValue.y += x * Time.deltaTime * -speed;
      if (this.rotationValue.x > 90) // 矫正
      {
        this.rotationValue.x = 90;
      }
      else if (this.rotationValue.x < -90)
      {
        this.rotationValue.x = -90;
      }
      objectTrC.rotation = Quaternion.Euler(this.rotationValue);
      return;
    }
  }
}