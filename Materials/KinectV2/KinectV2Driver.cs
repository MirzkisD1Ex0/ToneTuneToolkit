using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit.KinectV2
{
  public class KinectV2Driver : MonoBehaviour, KinectGestures.GestureListenerInterface
  {
    public static KinectV2Driver Instance;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    // ==================================================

    public void UserDetected(long userID, int userIndex)
    {
      KinectManager kinectManager = KinectManager.Instance;
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.RaiseRightHand);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.RaiseLeftHand);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Psi);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Stop);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Wave);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.RaiseRightHand);
      kinectManager.DetectGesture(userID, KinectGestures.Gestures.SwipeUp);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.SwipeDown);
      kinectManager.DetectGesture(userID, KinectGestures.Gestures.SwipeLeft);
      kinectManager.DetectGesture(userID, KinectGestures.Gestures.SwipeRight);
      // gestures 枚举里 Click默认为 注释
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Click);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.ZoomOut);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.ZoomIn);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Wheel);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Jump);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Squat);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Push);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Pull);
      // kinectManager.DetectGesture(userID, KinectGestures.Gestures.Tpose);
      Debug.Log("[Kinect] User " + userID + " Detected...");
      return;
    }

    public void UserLost(long userID, int userIndex)
    {
      Debug.Log("[Kinect] User " + userID + " lost.");
      return;
    }

    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
      return;
    }

    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
    {
      // if (userId == 0) // 检测第一个用户
      switch (gesture)
      {
        default: break;
        case KinectGestures.Gestures.None:
          break;
        case KinectGestures.Gestures.SwipeUp:
          break;
        case KinectGestures.Gestures.SwipeLeft:
          break;
        case KinectGestures.Gestures.SwipeRight:
          break;
      }
      return true;
    }

    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
    {
      return true;
    }
  }
}