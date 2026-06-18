/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class URPCameraTransparentSetter : MonoBehaviour
{
  private void Start()
  {
    GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;

    var universalAdditionalCameraData = GetComponent<UniversalAdditionalCameraData>();
    universalAdditionalCameraData.renderPostProcessing = false;
  }
}
