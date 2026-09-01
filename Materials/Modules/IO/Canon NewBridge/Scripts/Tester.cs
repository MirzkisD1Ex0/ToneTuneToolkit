/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit.IO.Canon
{
  public class Tester : MonoBehaviour
  {
    public void OnGUI()
    {
      if (GUILayout.Button(Configer.Get)) { UniOSCManager.SendOSCMessage(Configer.Get, "status-001"); }
      if (GUILayout.Button(Configer.LiveViewOn)) { UniOSCManager.SendOSCMessage(Configer.LiveViewOn, "connect-001"); }
      if (GUILayout.Button(Configer.LiveViewOff)) { UniOSCManager.SendOSCMessage(Configer.LiveViewOff, "liveview-stop-001"); }
      if (GUILayout.Button(Configer.TakePhoto)) { UniOSCManager.SendOSCMessage(Configer.TakePhoto, "capture-001"); }
    }
  }
}
