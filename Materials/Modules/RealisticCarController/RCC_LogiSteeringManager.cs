/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ToneTuneToolkit.RealisticCarController
{
  // [RequireComponent(typeof(LogitechSteeringWheel))] // 不需要
  public class RCC_LogiSteeringManager : MonoBehaviour
  {
    [SerializeField] private RCC_Inputs newInputs = new RCC_Inputs();

    public static UnityAction OnSteeringKey01Pressed;
    public static UnityAction OnSteeringKey09Pressed;
    public static UnityAction OnSteeringKey23Pressed;
    public static UnityAction OnThrottlePressed; private bool isThrottlePressed = false;

    // ==================================================

    private void Update() => LogiSteeringControl();

    // ==================================================

    private void LogiSteeringControl()
    {
      if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0))
      {
        return;
      } // 检测罗技方向盘是否开启

      if (AllowOverrideInputs && RCC_SceneManager.Instance.activePlayerVehicle != null) { RCC_SceneManager.Instance.activePlayerVehicle.OverrideInputs(newInputs); }

      G29_Input();
      // Keyboard_Input();
    }

    // ==================================================

    private static bool AllowOverrideInputs = true;
    public static void SwitchAllowOverrideInputs(bool value) => AllowOverrideInputs = value;

    // ==================================================

    private void Keyboard_Input()
    {
      if (Input.GetKey(KeyCode.W)) { newInputs.throttleInput = 1; }
      if (Input.GetKey(KeyCode.S))
      {
        newInputs.throttleInput = 0;
        newInputs.brakeInput = 0;
      }

      if (Input.GetKey(KeyCode.A)) { newInputs.steerInput = -1f; }
      else if (Input.GetKey(KeyCode.D)) { newInputs.steerInput = 1f; }

      if (Input.GetKey(KeyCode.X)) { newInputs.brakeInput = 1; }
    }



    private void G29_Input()
    {
      LogitechGSDK.DIJOYSTATE2ENGINES logiData = LogitechGSDK.LogiGetStateUnity(0); // 获取罗技方向盘数据对象

      // for (int i = 0; i < 30; i++) // 按键编号扫射
      // { if (LogitechGSDK.LogiButtonTriggered(0, i)) { Debug.Log("当前按下的按键编号是: " + i); } }

      LogitechGSDK.LogiSetOperatingRange(0, 540); // 设置转动角度
      LogitechGSDK.LogiPlaySpringForce(0, 0, 40, 80); // 设置中心弹力

      if (LogitechGSDK.LogiButtonTriggered(0, 1)) { OnSteeringKey01Pressed?.Invoke(); } // 方块键
      if (LogitechGSDK.LogiButtonTriggered(0, 9)) { OnSteeringKey09Pressed?.Invoke(); } // 菜单键
      if (LogitechGSDK.LogiButtonTriggered(0, 23)) { OnSteeringKey23Pressed?.Invoke(); } // 回车键

      if (logiData.lY <= -31000) // 油门
      {
        if (!isThrottlePressed) // 确保只触发一次，不会每帧都触发
        {
          isThrottlePressed = true;
          OnThrottlePressed?.Invoke(); // 触发事件
        }
      }
      else { isThrottlePressed = false; }

      newInputs.steerInput = logiData.lX / 32768f; // 方向盘控制转向
      newInputs.throttleInput = Mathf.InverseLerp(-32768, 32767, -logiData.lY);
      newInputs.brakeInput = Mathf.InverseLerp(-32768, 32767, -logiData.lRz);
    }
  }
}
