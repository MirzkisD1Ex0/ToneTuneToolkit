/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.LED
{
  /// <summary>
  /// LED助手
  /// ledconfig.json并无不同 // 只是端口为固定的而已
  /// 需要挂载在对象上
  /// 寻找必要组件
  /// </summary>
  [RequireComponent(typeof(LEDCommandCenter))]
  public class LEDHandler : MonoBehaviour
  {
    public static LEDHandler Instance;

    #region GOs
    public static GameObject NodeColorGO;
    public static GameObject NodeBrightnessGO;
    public static GameObject NodeSettingGO;
    public static GameObject SliderBrightnessGO;
    public static GameObject SliderPortGO;
    public static GameObject SliderBeginGO;
    public static GameObject SliderEndGO;
    public static GameObject SliderSpeedGO;
    public static GameObject NodeEffectGO;
    #endregion

    private void Awake()
    {
      NodeColorGO = GameObject.Find("Node - Color");
      NodeBrightnessGO = GameObject.Find("Node - Brightness");
      NodeSettingGO = GameObject.Find("Node - Setting");
      SliderBrightnessGO = GameObject.Find("Slider - Brightness");
      SliderPortGO = GameObject.Find("Slider - Port");
      SliderBeginGO = GameObject.Find("Slider - Begin");
      SliderEndGO = GameObject.Find("Slider - End");
      SliderSpeedGO = GameObject.Find("Slider - Speed");
      NodeEffectGO = GameObject.Find("Node - Effect");
    }

    /// <summary>
    /// 色温计算
    /// 从黄色到蓝色
    /// </summary>
    /// <param name="temperature">浮动范围需要在-1,1之间</param>
    /// <returns></returns>
    public static Color CalculateColorTemperature(float temperature)
    {
      float r = 1f, g = 1f, b = 1f;
      if (-1f <= temperature && temperature < 0f)
      {
        r = 1f;
        g = 1f;
        b = 1f + temperature;
      }
      else if (temperature == 0)
      {
        r = 1f;
        g = 1f;
        b = 1f;
      }
      else if (0f < temperature && temperature <= 1f)
      {
        r = 1f - temperature;
        g = 1f - temperature;
        b = 1f;
      }
      return new Color(r, g, b, 1);
    }
  }
}