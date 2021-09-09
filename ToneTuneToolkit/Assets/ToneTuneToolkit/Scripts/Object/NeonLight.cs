/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 霓虹灯效果
  ///
  /// 挂在需要拖拽的物体上
  /// 需要灯光组件
  /// </summary>
  public class NeonLight : MonoBehaviour
  {
    [Header("[Float]闪烁速度:数值越大，闪烁速度越慢")]
    public float Speed = 2f;
    [Header("[Float]间隔时间:数值越小，闪烁间隔越短")]
    public float intervalTime = 1f;

    private int r = 0, g = 0, b = 0, a = 0;
    private Light lightC;

    private void Start()
    {
      if (!GetComponent<Light>())
      {
        TipTools.Error("[NeonLight] " + "Cant find Light.");
        gameObject.AddComponent<Light>();
        lightC.type = LightType.Point;
      }
      lightC = GetComponent<Light>();
      r = Random.Range(0, 255);
      g = Random.Range(0, 255);
      b = Random.Range(0, 255);
      a = Random.Range(155, 255);
    }

    private void Update()
    {
      LightPingPong();
    }

    private void LightPingPong()
    {
      if (lightC.color.a <= 0.01f)
      {
        r = Random.Range(0, 255);
        g = Random.Range(0, 255);
        b = Random.Range(0, 255);
        a = Random.Range(155, 255);
      }
      lightC.color = Color.Lerp(
          new Color(r / 255f, g / 255f, b / 255f, a / 255f),
          new Color(0, 0, 0, 0),
          Mathf.PingPong(Time.time / Speed, intervalTime));
      return;
    }
  }
}