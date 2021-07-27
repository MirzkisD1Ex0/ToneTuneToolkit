using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Camera
{
  /// <summary>
  /// OK
  /// 鼠标控制物体环绕注视对象
  /// 推荐挂在相机上
  /// 需要相机为MainCameraTag
  /// </summary>
  public class CameraFocusObject : MonoBehaviour
  {
    public GameObject FocusObject;

    private Transform foTrCmpt;

    private void Start()
    {
      if (!FocusObject)
      {
        TipTools.Notice(this.name + "组件缺失");
        this.enabled = false;
        return;
      }
      if (!UnityEngine.Camera.main)
      {
        TipTools.Notice(this.name + "相机缺失");
        this.enabled = false;
        return;
      }

      foTrCmpt = FocusObject.GetComponent<Transform>();
    }

    private void LateUpdate()
    {
      CameraRotate();
      CameraZoom();
    }

    /// <summary>
    /// 相机的旋转
    /// </summary>
    private void CameraRotate()
    {
      float mouseAxisX = Input.GetAxis("Mouse X");
      float mouseAxisY = -Input.GetAxis("Mouse Y");
      if (Input.GetKey(KeyCode.Mouse0)) // 左键
      {
        UnityEngine.Camera.main.transform.RotateAround(foTrCmpt.position, Vector3.up, mouseAxisX * 5);
        UnityEngine.Camera.main.transform.RotateAround(foTrCmpt.position, UnityEngine.Camera.main.transform.right, mouseAxisY * 5);
      }
      return;
    }

    /// <summary>
    /// 相机的缩放
    /// </summary>
    private void CameraZoom()
    {
      if (Input.GetAxis("Mouse ScrollWheel") > 0)
      {
        UnityEngine.Camera.main.transform.Translate(Vector3.forward * 0.1f);
      }
      if (Input.GetAxis("Mouse ScrollWheel") < 0)
      {
        UnityEngine.Camera.main.transform.Translate(Vector3.forward * -0.1f);
      }
      return;
    }
  }
}