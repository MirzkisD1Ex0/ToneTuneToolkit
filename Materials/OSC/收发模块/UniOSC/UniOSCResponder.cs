/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniOSC;

namespace ToneTuneToolkit.OSC
{
  public class UniOSCResponder : UniOSCEventTarget
  {
    public override void OnOSCMessageReceived(UniOSCEventArgs args)
    {
      AnalyseMessage(args);
      return;
    }

    private void AnalyseMessage(UniOSCEventArgs args)
    {
      Debug.Log($"[UniOSCReceiver] {args.Address}...<color=green>[OK]</color>");
      switch (args.Address)
      {
        default: break;

        case "/callback/resetscene": // 重加载场景
          SceneManager.LoadScene("Scene");
          break;
      }
    }
  }
}
