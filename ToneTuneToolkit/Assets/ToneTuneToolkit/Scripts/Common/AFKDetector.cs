/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System;
using UnityEngine;

namespace ToneTuneToolkit.Common
{
  public class AFKDetector : SingletonMaster<AFKDetector>
  {
    public static event Action<bool> OnAFK;

    private float timeoutDuration = 3f;

    [SerializeField] private float timer = 0f;
    private bool isAFK = false; // 是否已经进入挂机状态

    // ============================================================

    private void Update() => DetectAFK();

    // ============================================================

    private void DetectAFK()
    {
      // 1. 核心条件判定：点击键盘(anyKey) 或者是 鼠标物理点击(左/右/中键)
      bool hasActiveInput =
        Input.anyKey ||
        Input.GetMouseButtonDown(0) ||
        Input.GetMouseButtonDown(1) ||
        Input.GetMouseButtonDown(2);

      // 2. 特殊检测：如果是在移动端/VR端，检测是否有手指【刚刚触摸按下】
      if (Input.touchCount > 0)
      {
        foreach (Touch touch in Input.touches)
        {
          if (touch.phase == TouchPhase.Began)
          {
            hasActiveInput = true;
            break;
          }
        }
      }

      // 3. 只有发生主动交互（按键/点击）时才会重置计时器，鼠标单纯悬停移动不再影响计时
      if (hasActiveInput)
      {
        timer = 0f;
        if (isAFK)
        {
          isAFK = false;
          OnAFK?.Invoke(isAFK);
        }
      }
      else
      {
        // 4. 无输入时，累计时间
        timer += Time.deltaTime;

        if (timer >= timeoutDuration && !isAFK)
        {
          isAFK = true;
          Debug.Log($"检测到用户已超过 {timeoutDuration} 秒没有任何操作！");
          OnAFK?.Invoke(isAFK);
        }
      }
    }
  }
}