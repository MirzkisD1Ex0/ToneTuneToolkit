/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System.Collections;
using System.Collections.Generic;
using com.rfilkov.kinect;
using UnityEngine;
using UnityEngine.Events;

namespace LancomeKinect
{
  public class AzureKinectDriver : MonoBehaviour, GestureListenerInterface
  {
    public static AzureKinectDriver Instance;

    public KinectGestureManager gestureManager;

    public static UnityAction OnGestureCompleted;

    // ==================================================

    private void Awake() => Instance = this;
    private void Start() => Init();

    // ==================================================

    private void Init()
    {
      // StartCoroutine(KinectAwakeLoop());
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

    public void UserDetected(ulong userID, int userIndex)
    {
      if (gestureManager)
      {
        gestureManager.DetectGesture(userID, GestureType.Wave);
        gestureManager.DetectGesture(userID, GestureType.SwipeUp);
        gestureManager.DetectGesture(userID, GestureType.SwipeLeft);
        gestureManager.DetectGesture(userID, GestureType.SwipeRight);
        gestureManager.DetectGesture(userID, GestureType.RaiseRightHand);
        gestureManager.DetectGesture(userID, GestureType.RaiseLeftHand);
      }
      Debug.Log($"[AKD] User {userID} Detected");
    }

    public void UserLost(ulong userID, int userIndex)
    {
      Debug.Log($"[AKD] User {userID} lost");
    }

    public void GestureInProgress(ulong userId, int userIndex, GestureType gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {

    }

    public bool GestureCompleted(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint, Vector3 screenPos)
    {
      // if (userId == 0) // 检测第一个用户
      switch (gesture)
      {
        default: break;
        // case GestureType.None:
        //   break;
        case GestureType.Wave:
          OnGestureCompleted?.Invoke();
          break;
          // case GestureType.SwipeUp:
          //   GameManager.Instance.PreStartStep01();
          //   break;
          // case GestureType.SwipeLeft:
          //   GameManager.Instance.PreStartStep01();
          //   break;
          // case GestureType.SwipeRight:
          //   GameManager.Instance.PreStartStep01();
          //   break;
          // case GestureType.RaiseLeftHand:
          //   GameManager.Instance.PreStartStep01();
          //   break;
          // case GestureType.RaiseRightHand:
          //   GameManager.Instance.PreStartStep01();
          //   break;
      }
      return true;
    }

    public bool GestureCancelled(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint)
    {
      return true;
    }
  }
}
