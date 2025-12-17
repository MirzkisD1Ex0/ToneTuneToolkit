/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;

namespace ToneTuneToolkit.Arduino
{
  public class ArduinoResponder : SingletonMaster<ArduinoResponder>
  {

    // ==================================================

    private void OnConnectionEvent(bool success)
    {
      if (success) { Debug.Log("[AR] Connection established"); }
      else { Debug.Log("[AR] Connection attempt failed or disconnection detected"); }
    }

    private void OnMessageArrived(string msg) => AnalyzeMessage(msg);

    // ==================================================

    public void AnalyzeMessage(string value)
    {
      if (value == null) { return; }
      Debug.Log("[AR] Arduino message arrived: " + value);

      AudioManager.Instance.StartPlay();
      // switch (value)
      // {
      //   default:  break;
      // case "Test": Debug.Log("Testing."); break;
      // }
    }
  }
}