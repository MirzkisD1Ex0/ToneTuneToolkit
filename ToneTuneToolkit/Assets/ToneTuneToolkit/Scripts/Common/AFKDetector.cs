/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace ToneTuneToolkit.Common
{
  public class AFKDetector : SingletonMaster<AFKDetector>
  {
    public static event Action OnAFK;

    private float timeoutDuration = 180f;

    [SerializeField] private float timer = 0f;
    private bool isAFK = false; // 是否已经进入挂机状态

    // ============================================================

    private void Update() => DetectAFK();

    // ============================================================

    private void DetectAFK()
    {
      // 1. 检测是否有任何输入（键盘、鼠标按键、屏幕触摸）
      bool hasInput = Input.anyKey || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0;

      // 2. 特殊检测：如果是在移动端，检测多指触摸
      if (Input.touchCount > 0)
      {
        hasInput = true;
      }

      // 3. 如果有输入，或者当前指针正悬停/点击在 UI 上
      if (hasInput || IsPointerOverUI())
      {
        ResetTimer();
      }
      else
      {
        // 4. 无输入时，累计时间
        timer += Time.deltaTime;

        if (timer >= timeoutDuration && !isAFK)
        {
          isAFK = true;
          Debug.Log($"检测到用户已超过 {timeoutDuration} 秒没有任何操作！");

          OnAFK?.Invoke();
        }
      }
    }

    // ============================================================

    private void ResetTimer()
    {
      if (isAFK)
      {
        isAFK = false;
      }
      timer = 0f;
    }

    private bool IsPointerOverUI()
    {
      if (EventSystem.current == null) return false;

      // 适配 PC 端鼠标 和 移动端/VR 端触摸
      if (Input.touchCount > 0)
      {
        return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
      }

      return EventSystem.current.IsPointerOverGameObject();
    }
  }
}