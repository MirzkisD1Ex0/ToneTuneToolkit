using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using ToneTuneToolkit;

namespace ToneTuneToolkit.LED
{
  /// <summary>
  /// OK
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
  }
}