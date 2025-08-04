using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Q))
    {
      FaceDetectorManager.Instance.SwitchFaceDetector(FaceDetectorManager.FaceDetectorState.Play);
    }
    if (Input.GetKeyDown(KeyCode.W))
    {
      FaceDetectorManager.Instance.SwitchFaceDetector(FaceDetectorManager.FaceDetectorState.Stop);
    }
  }
}
