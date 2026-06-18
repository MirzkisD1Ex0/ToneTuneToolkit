/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;
using System.Runtime.InteropServices;

public class JSCommunicator : SingletonMaster<JSCommunicator>
{
  [DllImport("__Internal")] public static extern void PuzzleReady();
  [DllImport("__Internal")] public static extern void PuzzleDebug(string value);
  [DllImport("__Internal")] public static extern void PuzzleFinished();

  public void StartGame(string message)
  {
    Debug.Log(@$"[JSC] Game type: {message}");
    GameManager.Instance.StartGame(message);
  }

  public void ResetGame()
  {
    GameManager.Instance.Reset();
  }
}
