using System.Collections;
using System.Collections.Generic;
using com.rfilkov.kinect;
using UnityEngine;

public class AzureKinectDriver : MonoBehaviour, GestureListenerInterface
{
  public static AzureKinectDriver Instance;

  public int playerIndex = 0;
  public List<GestureType> detectGestures = new List<GestureType>();

  // ==================================================

  private void Awake() => Instance = this;
  private void Start() => Init();

  // ==================================================

  private void Init()
  {
    // StartCoroutine(nameof(KinectAwakeLoop));
    return;
  }

  private IEnumerator KinectAwakeLoop()
  {
    while (true)
    {
      yield return new WaitForSeconds(60f);
      KinectManager.Instance.StartDepthSensors();
    }
  }

  // ==================================================

  /// <summary>
  /// 检测到用户
  /// </summary>
  /// <param name="userID"></param>
  /// <param name="userIndex"></param>
  public void UserDetected(ulong userID, int userIndex)
  {
    if (userIndex == playerIndex)
    {
      Debug.Log($"[AKD] Target user {playerIndex} Detected.");
      KinectGestureManager gestureManager = KinectManager.Instance.gestureManager;
      foreach (GestureType gesture in detectGestures)
      {
        gestureManager.DetectGesture(userID, gesture); // 添加监听的动作
      }
    }
    else
    {
      Debug.Log($"[AKD] Non-target user {userID} Detected.");
    }
    // gestureManager.DetectGesture(userID, GestureType.SwipeLeft);
    // gestureManager.DetectGesture(userID, GestureType.SwipeRight);
    // gestureManager.DetectGesture(userID, GestureType.RaiseRightHand);
    // gestureManager.DetectGesture(userID, GestureType.RaiseLeftHand);
    return;
  }

  /// <summary>
  /// 用户丢失
  /// </summary>
  /// <param name="userID"></param>
  /// <param name="userIndex"></param>
  public void UserLost(ulong userID, int userIndex)
  {
    if (userIndex != playerIndex)
    {
      return;
    }
    Debug.Log($"[AKD] User {userID} lost.");
    return;
  }

  public void GestureInProgress(ulong userId, int userIndex, GestureType gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
  {
    return;
  }

  public bool GestureCompleted(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint, Vector3 screenPos)
  {
    if (userIndex != playerIndex) // 检测到非指定用户
    {
      return false;
    }

    Debug.Log($"[AKD] Gesture <color=white>{gesture}</color> detected.");
    switch (gesture)
    {
      default: break;
      case GestureType.None:
        break;
      // case GestureType.SwipeUp:
      //   break;
      case GestureType.SwipeLeft:
        break;
      case GestureType.SwipeRight:
        break;
    }
    return true;
  }

  public bool GestureCancelled(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint)
  {
    return true;
  }
}