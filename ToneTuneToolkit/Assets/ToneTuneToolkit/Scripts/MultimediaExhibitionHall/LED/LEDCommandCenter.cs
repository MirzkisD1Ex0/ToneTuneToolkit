/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using ToneTuneToolkit.Common;
using ToneTuneToolkit.UDP;

namespace ToneTuneToolkit.LED
{
  /// <summary>
  /// 凌恩LED命令中心
  ///
  /// 语法配套
  /// </summary>
  public class LEDCommandCenter : MonoBehaviour
  {
    public static LEDCommandCenter Instance;

    #region SetBrightness
    private static string sbCommand = null;
    private static string brightnessValue = "100"; // 亮度参数
    #endregion


    #region SemanticLighting
    private static string slCommand = null;
    private static string actionValue = "DimColor";
    private static string effectTypeValue = "Blink";
    private static string portValue = "1";
    private static string beginValue = "1";
    private static string endValue = "144";
    private static string speedValue = "10";
    private static string colorValue = "#FFFFFF";
    #endregion

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      EventBind();
      SLDimColor("#FFFFFF");
      SBChangeBrightness(10);
      SLDimEffect("Delete");
    }

    private void OnApplicationQuit()
    {
      SLDimColor("#FFFFFF");
      SBChangeBrightness(0);
      SLDimEffect("Delete");
    }

    private void EventBind()
    {
      for (int i = 1; i < LEDHandler.NodeColorGO.transform.childCount; i++) // i+1/count-1 为了偏移title
      {
        GameObject tempGO = LEDHandler.NodeColorGO.transform.GetChild(i).gameObject;
        tempGO.GetComponent<Button>().onClick.AddListener(() => SLDimColor(tempGO.name));
      }

      LEDHandler.SliderBrightnessGO.GetComponent<Slider>().onValueChanged.AddListener(SBChangeBrightness);
      LEDHandler.SliderPortGO.GetComponent<Slider>().onValueChanged.AddListener(SLSetPort);
      LEDHandler.SliderBeginGO.GetComponent<Slider>().onValueChanged.AddListener(SLSetBegin);
      LEDHandler.SliderEndGO.GetComponent<Slider>().onValueChanged.AddListener(SLSetEnd);
      LEDHandler.SliderSpeedGO.GetComponent<Slider>().onValueChanged.AddListener(SLSetSpeed);

      for (int i = 1; i < LEDHandler.NodeEffectGO.transform.childCount; i++)
      {
        GameObject tempGO = LEDHandler.NodeEffectGO.transform.GetChild(i).gameObject;
        tempGO.GetComponent<Button>().onClick.AddListener(() => SLDimEffectPreaction(tempGO.name));
      }
      return;
    }

    #region SetBrightness
    private static void SBCommandRecompose()
    {
      sbCommand =
          LEDCommandHub.Basic.BaseCommand +
          LEDCommandHub.MethodType.SetBrightnessBrightness + brightnessValue;
      UDPCommunicator.Instance.SendMessageOut(sbCommand);
      return;
    }

    private void SBChangeBrightness(float brightnessValue)
    {
      LEDHandler.SliderBrightnessGO.transform.GetChild(0).GetComponent<Text>().text = brightnessValue.ToString();
      LEDCommandCenter.brightnessValue = ((int)brightnessValue).ToString();
      SBCommandRecompose();
      return;
    }
    #endregion

    #region SemanticLighting
    private static void SLCommandRecompose()
    {
      slCommand = null;
      slCommand +=
          LEDCommandHub.Basic.BaseCommand +
          LEDCommandHub.MethodType.SemanticLighting +
          LEDCommandHub.SemanticLighting.Action + actionValue +
          LEDCommandHub.Basic.Port + portValue +
          LEDCommandHub.Basic.Begin + beginValue +
          LEDCommandHub.Basic.End + endValue;

      switch (actionValue)
      {
        default: break;
        case LEDCommandHub.SemanticLighting.DimColor:
          slCommand +=
              LEDCommandHub.Basic.Color + colorValue;
          break;
        case LEDCommandHub.SemanticLighting.DimEffect:
          slCommand +=
              LEDCommandHub.SemanticLighting.DimEffectName + "TESTNAME" +
              LEDCommandHub.SemanticLighting.DimEffectType + effectTypeValue;
          if (effectTypeValue != "Delete")
          {
            slCommand +=
                LEDCommandHub.SemanticLighting.DimEffectSpeed + speedValue +
                LEDCommandHub.Basic.Color + colorValue;
          }
          break;
      }

      UDPCommunicator.Instance.SendMessageOut(slCommand);
      return;
    }

    /// <summary>
    /// 改变灯带的颜色
    /// </summary>
    /// <param name="hexColor">#FFFFFF</param>
    public void SLDimColor(string hexColor)
    {
      actionValue = LEDCommandHub.SemanticLighting.DimColor;
      colorValue = hexColor;
      SLCommandRecompose();
      return;
    }

    private void SLDimEffectPreaction(string effectType)
    {
      SLDimEffect("Delete");
      SLDimEffect(effectType);
      return;
    }

    private void SLDimEffect(string effectType)
    {
      actionValue = LEDCommandHub.SemanticLighting.DimEffect;
      effectTypeValue = effectType;
      SLCommandRecompose();
      return;
    }

    /// <summary>
    /// 设定端口
    /// </summary>
    /// <param name="port"></param>
    private void SLSetPort(float port)
    {
      LEDHandler.SliderPortGO.transform.GetChild(0).GetComponent<Text>().text = port.ToString();
      portValue = ((int)port).ToString();
      return;
    }

    /// <summary>
    /// 设定起始点
    /// </summary>
    /// <param name="begin"></param>
    private void SLSetBegin(float begin)
    {
      LEDHandler.SliderBeginGO.transform.GetChild(0).GetComponent<Text>().text = begin.ToString();
      beginValue = ((int)begin).ToString();
      return;
    }

    /// <summary>
    /// 设定结束点
    /// </summary>
    /// <param name="end"></param>
    private void SLSetEnd(float end)
    {
      LEDHandler.SliderEndGO.transform.GetChild(0).GetComponent<Text>().text = end.ToString();
      endValue = ((int)end).ToString();
      return;
    }

    private void SLSetSpeed(float speed)
    {
      LEDHandler.SliderSpeedGO.transform.GetChild(0).GetComponent<Text>().text = speed.ToString();
      speedValue = ((int)speed).ToString();
      return;
    }
    #endregion
  }
}