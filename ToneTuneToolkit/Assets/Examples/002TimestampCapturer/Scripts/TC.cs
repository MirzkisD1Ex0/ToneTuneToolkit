using UnityEngine;
using System;
using ToneTuneToolkit.Common;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class TC : MonoBehaviour
  {
    private void Start()
    {
      // TimestampCapturer.Instance.GetNetTimestamp();

      long localTimestamp = TimestampCapturer.GetLocalTimestamp();
      DateTime dt = DataConverter.ConvertTimestamp2DateTime(localTimestamp);
      Debug.Log("Local Date: " + dt + " = " + localTimestamp);
    }
  }
}