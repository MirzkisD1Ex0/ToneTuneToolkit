/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 物体上下漂浮
  ///
  /// 需要挂在对象上
  /// </summary>
  public class ObjectFloating : MonoBehaviour
  {
    public float PerRadian = 2f; // 每次变化的弧度 // 速度
    public float Radius = 0.2f; // 半径 // 幅度

    private float radian = 0; // 弧度
    private Vector3 oldPos; // 开始时候的坐标

    private void Start()
    {
      this.oldPos = transform.position; // 将最初的位置保存到oldPos
    }

    private void Update()
    {
      this.Float();
    }

    private void Float()
    {
      this.radian += this.PerRadian / 100f; // 弧度每次加
      float temporaryValue = Mathf.Cos(this.radian) * this.Radius; // dy定义的是针对y轴的变量，也可以使用sin，找到一个适合的值就可以
      transform.position = this.oldPos + new Vector3(0, temporaryValue, 0);
    }
  }
}