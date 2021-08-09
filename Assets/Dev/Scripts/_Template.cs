using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.Object;
using ToneTuneToolkit.Other;

namespace Dev
{
  /// <summary>
  /// 
  /// </summary>
  public class _Template : MonoBehaviour
  {
    private void Start()
    {
      QRCodeHelper.Instance.GetQRContent(Application.streamingAssetsPath + "/asd.jpg");
    }
  }
}