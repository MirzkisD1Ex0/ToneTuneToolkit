using UnityEngine;
using Leap;
using Leap.Unity;
using UnityEngine.Events;
using System.Collections;

public class LeapMotionManager : MonoBehaviour
{
  public static LeapMotionManager Instance;

  public static UnityAction OnHandDetected;
  public static UnityAction OnHandLost;
  public static UnityAction OnSwipeRight;
  public static UnityAction OnSwipeLeft;

  // ==================================================

  private void Awake() => Instance = this;
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
      Debug.Log(frame.Hands.Count);
      if (OnHandDetected != null)
      {
        OnHandDetected();
      }
    }
    else
    {
      if (OnHandLost != null)
      {
        OnHandLost();
      }
    }
    return;
  }

  #endregion
  // ==================================================
  #region 设置检测间隔

  private float detectSpace = 1f; // 检测间隙
  public void SetDetectSpace(float value)
  {
    detectSpace = value;
    return;
  }

  #endregion
  // ==================================================
  #region 设置阈值

  private float detectThreshold = 0.9f; // 挥手速度的阈值 // 越小越容易检测到
  public void SetDetectThreshold(float value)
  {
    detectThreshold = value;
    return;
  }

  #endregion
  // ==================================================
  #region 左右挥手检测

  private bool allowNotice = true;

  private void DetectSwipe()
  {
    Frame frame = Hands.Provider.CurrentFrame; // 获取当前帧
    foreach (Hand hand in frame.Hands) // 检测每只手
    {
      Vector3 palmVelocity = hand.PalmVelocity; // 获取手的手掌速度

      // 检测挥手动作
      if (palmVelocity.x > detectThreshold || palmVelocity.x < -detectThreshold)
      {
        if (!allowNotice) // 是否允许发消息
        {
          return;
        }
        allowNotice = false; // 上锁

        // 检测到挥手动作，执行相应的逻辑
        // Debug.Log("检测到挥手动作，方向：" + (palmVelocity.x > 0 ? "向右" : "向左"));

        if (palmVelocity.x > 0)
        {
          if (OnSwipeRight != null) // 右
          {
            OnSwipeRight();
          }
        }
        else if (palmVelocity.x < 0) // 左
        {
          if (OnSwipeLeft != null)
          {
            OnSwipeLeft();
          }
        }

        StartCoroutine(nameof(BeginCooldown)); // 解锁
      }
    }
    return;
  }

  private IEnumerator BeginCooldown()
  {
    yield return new WaitForSeconds(detectSpace);
    allowNotice = true;
    yield break;
  }

  #endregion
}