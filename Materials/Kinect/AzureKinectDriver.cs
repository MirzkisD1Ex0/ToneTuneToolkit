using System.Collections;
using System.Collections.Generic;
using com.rfilkov.components;
using com.rfilkov.kinect;
using UnityEngine;
using Windows.Kinect;

namespace LancomeKinect
{
  public class AzureKinectDriver : MonoBehaviour, GestureListenerInterface
  {
    public static AzureKinectDriver Instance;

    public KinectGestureManager gestureManager;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    
    {
      Init();
    }

    // ==================================================

    private void Init()
    {
      StartCoroutine("KinectAwakeLoop");
      return;
    }

    private IEnumerator KinectAwakeLoop()
    {
      yield return new WaitForSeconds(60f);
      // KinectManager.Instance.StartDepthSensors();
      yield break;
    }

    // ==================================================

    public void UserDetected(ulong userID, int userIndex)
    {
      gestureManager.DetectGesture(userID, GestureType.Wave);
      gestureManager.DetectGesture(userID, GestureType.SwipeUp);
      gestureManager.DetectGesture(userID, GestureType.SwipeLeft);
      gestureManager.DetectGesture(userID, GestureType.SwipeRight);
      gestureManager.DetectGesture(userID, GestureType.RaiseRightHand);
      gestureManager.DetectGesture(userID, GestureType.RaiseLeftHand);
      Debug.Log($"[Kinect] User {userID} Detected...[OK]");
      return;
    }

    public void UserLost(ulong userID, int userIndex)
    {
      Debug.Log($"[Kinect] User {userID} lost...[OK]");
      return;
    }

    public void GestureInProgress(ulong userId, int userIndex, GestureType gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
      return;
    }

    public bool GestureCompleted(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint, Vector3 screenPos)
    {
      // if (userId == 0) // 检测第一个用户
      switch (gesture)
      {
        default: break;
        case GestureType.None:
          break;
        case GestureType.Wave:
          GameManager.Instance.PreStartStep01();
          break;
        case GestureType.SwipeUp:
          GameManager.Instance.PreStartStep01();
          break;
        case GestureType.SwipeLeft:
          GameManager.Instance.PreStartStep01();
          break;
        case GestureType.SwipeRight:
          GameManager.Instance.PreStartStep01();
          break;
        case GestureType.RaiseLeftHand:
          GameManager.Instance.PreStartStep01();
          break;
        case GestureType.RaiseRightHand:
          GameManager.Instance.PreStartStep01();
          break;
      }
      return true;
    }

    public bool GestureCancelled(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint)
    {
      return true;
    }
  }
}