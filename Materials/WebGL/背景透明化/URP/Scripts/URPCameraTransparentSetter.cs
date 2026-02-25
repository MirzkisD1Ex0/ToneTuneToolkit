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