/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>



using UnityEngine;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 物体上下漂浮
  /// 需要挂在对象上
  /// </summary>
  public class ObjectFloating : MonoBehaviour
  {
    public float PerRadian = 2f; // 每次变化的弧度 // 速度
    public float Radius = 0.2f; // 半径 // 幅度

    private float radian = 0; // 弧度
    private Vector3 oldPos; // 开始时候的坐标

    // ==================================================

    private void Start()
    {
      oldPos = transform.position; // 将最初的位置保存到oldPos
    }

    private void Update()
    {
      Floating();
    }

    // ==================================================

    private void Floating()
    {
      radian += PerRadian / 100f; // 弧度每次加
      float temporaryValue = Mathf.Cos(radian) * Radius; // dy定义的是针对y轴的变量，也可以使用sin，找到一个适合的值就可以
      transform.position = oldPos + new Vector3(0, temporaryValue, 0);
      return;
    }
  }
}
