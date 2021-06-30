using System.Collections;
using UnityEngine;

namespace ToneTuneToolkit
{
    /// <summary>
    /// 拖拽对象
    /// 挂在需要拖拽的对象上
    /// 需要相机为MainCameraTag
    /// 需要碰撞器
    /// </summary>
    public class ObjectDrag : MonoBehaviour
    {
        private Vector3 screenPosition;
        private Vector3 offset;
        private Vector3 currentScreenPosition;

        private void Start()
        {
            if (!Camera.main)
            {
                TTTDebug.Warning(this.name + "相机缺失");
                this.enabled = false;
                return;
            }
        }

        private IEnumerator OnMouseDown()
        {
            screenPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
            while (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                currentScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
                gameObject.transform.position = Camera.main.ScreenToWorldPoint(currentScreenPosition) + offset;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}