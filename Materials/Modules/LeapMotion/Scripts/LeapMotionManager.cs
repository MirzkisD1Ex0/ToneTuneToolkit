/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using UnityEngine;
using Leap;
using Leap.Unity;
using UnityEngine.Events;
using System.Collections;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.LeapMotion
{
  [RequireComponent(typeof(LeapServiceProvider))]
  public class LeapMotionManager : SingletonMaster<LeapMotionManager>
  {
    public static UnityAction OnHandDetected;
    public static UnityAction OnHandLost;
    public static UnityAction OnSwipeLeft;
    public static UnityAction OnSwipeRight;
    public static UnityAction OnSwipeBack;
    public static UnityAction OnSwipeFront;

    // ==================================================

    private void Update()
    {
      DetectHands();
      DetectSwipe();
    }

    // ==================================================
    #region 检测是否有手

    private void DetectHands()
    {
      Frame frame = Hands.Provider.CurrentFrame;

      if (frame.Hands.Count >= 1)
      {
        // Debug.Log(@$"[LMM]: 手部数量={frame.Hands.Count}");
        OnHandDetected?.Invoke();
      }
      else
      { OnHandLost?.Invoke(); }
    }

    #endregion
    // ==================================================
    #region 设置检测间隔

    private float detectSpace = 0.6f; // 检测间隙
    public void SetDetectSpace(float value) => detectSpace = value;

    #endregion
    // ==================================================
    #region 设置阈值

    private float detectThreshold = 0.9f; // 挥手速度的阈值 // 越小越容易检测到
    public void SetDetectThreshold(float value) => detectThreshold = value;

    #endregion
    // ==================================================
    #region 挥手检测

    private bool allowNotice = true;

    private void DetectSwipe()
    {
      if (!allowNotice) { return; } // 冷却中直接退出

      Frame frame = Hands.Provider.CurrentFrame;

      foreach (Hand hand in frame.Hands)
      {
        Vector3 v = hand.PalmVelocity; // 获取速度向量

        // 1. 计算三个轴向速度的绝对值
        float absX = Mathf.Abs(v.x);
        float absY = Mathf.Abs(v.y);
        float absZ = Mathf.Abs(v.z);

        // 2. 找到最大速度分量
        float maxVelocity = Mathf.Max(absX, absY, absZ);

        // 3. 如果最大速度都没达到阈值，说明挥得不够快，跳过
        if (maxVelocity < detectThreshold) { continue; }

        // 触发逻辑，开始上锁
        allowNotice = false;

        // 4. 判断哪个轴的位移最显著，并输出对应的 DebugLog
        if (absX > absY && absX > absZ)
        {
          // 左右挥手 (X轴)
          if (v.x > 0)
          {
            Debug.Log("[LMM] 检测到：向右挥手 (Right)");
            OnSwipeRight?.Invoke();
          }
          else
          {
            Debug.Log("[LMM] 检测到：向左挥手 (Left)");
            OnSwipeLeft?.Invoke();
          }
        }
        else if (absZ > absX && absZ > absY)
        {
          // 前后挥手 (Z轴)
          if (v.z > 0)
          {
            Debug.Log("[LMM] 检测到：向前挥手 (Front)");
            OnSwipeFront?.Invoke();
          }
          else
          {
            Debug.Log("[LMM] 检测到：向后挥手 (Back)");
            OnSwipeBack?.Invoke();
          }
        }
        else if (absY > absX && absY > absZ)
        {
          // 上下挥手 (Y轴) - 如果你需要的话
          Debug.Log($"[LMM] 检测到：垂直位移 (Y轴速度: {v.y})");
        }

        // 启动冷却协程
        StartCoroutine(BeginCooldown());

        // 只要触发了一次有效挥手，就跳出 foreach，防止双手数倍触发
        break;
      }
    }

    private IEnumerator BeginCooldown()
    {
      yield return new WaitForSeconds(detectSpace);
      allowNotice = true;
    }

    #endregion
  }
}
