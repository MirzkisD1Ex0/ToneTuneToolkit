using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Camera
{
  /// <summary>
  /// OK
  /// 鼠标拖动控制相机旋转
  /// 推荐挂在相机上
  /// 需要设置MainCameraTag
  /// 如果是为了实现全景效果，建议减少球模型的面数，此外还需要在建模软件内将模型的法线翻转至球的内侧
  /// 或者使用Sphere + Panoramic材质 // 更省事
  /// </summary>
  public class CameraLookAround : MonoBehaviour
  {
    public float speed = 1000; // 旋转速度

    private float xValue;
    private float yValue;
    private Vector3 rotationValue = Vector3.zero;

    private void Start()
    {
      if (!UnityEngine.Camera.main)
      {
        TipTools.Notice(this.name + "相机缺失");
        this.enabled = false;
        return;
      }
    }

    private void Update()
    {
      xValue = Input.GetAxis("Mouse X");
      yValue = Input.GetAxis("Mouse Y");

      if (Input.GetMouseButton(0)) // 按住鼠标左键
      {
        if (xValue != 0 || yValue != 0) // 并且鼠标移动
        {
          RotateView(xValue, yValue);
        }
      }
    }

    /// <summary>
    /// 视角滚动
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void RotateView(float x, float y)
    {
      rotationValue.x += y * Time.deltaTime * speed;
      rotationValue.y += -x * Time.deltaTime * speed;
      if (rotationValue.x > 90) // 矫正
      {
        rotationValue.x = 90;
      }
      else if (rotationValue.x < -90)
      {
        rotationValue.x = -90;
      }
      transform.rotation = Quaternion.Euler(rotationValue);
    }
  }
}