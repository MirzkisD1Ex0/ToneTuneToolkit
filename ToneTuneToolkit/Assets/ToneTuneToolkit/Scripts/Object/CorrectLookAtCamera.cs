/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 使物体永远朝向且正对相机
  /// </summary>
  public class CorrectLookAtCamera : MonoBehaviour
  {
    private Vector3 targetPosition;
    private Vector3 targetQuaternion;

    // ==================================================

    private void Update()
    {
      CorrectLookAt();
    }

    // ==================================================

    private void CorrectLookAt()
    {
      // Debug.DrawLine(transform.position, Camera.main.transform.position, Color.green);
      targetPosition = transform.position + Camera.main.transform.rotation * Vector3.forward;
      targetQuaternion = Camera.main.transform.rotation * Vector3.up;
      transform.LookAt(targetPosition, targetQuaternion);
      return;
    }
  }
}
