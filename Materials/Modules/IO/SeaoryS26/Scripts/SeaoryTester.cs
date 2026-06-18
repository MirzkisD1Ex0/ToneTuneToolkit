/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.IO.Seaory;

public class SeaoryTester : MonoBehaviour
{
  [SerializeField] private bool isDebug = true;

  // ==================================================

  private void Update() => DEBUG_ShortcutKey();

  // private void OnGUI() => DEBUG_OnGUI();

  // ==================================================

  private void DEBUG_ShortcutKey()
  {
    if (!isDebug) { enabled = false; return; }

    if (Input.GetKeyDown(KeyCode.Keypad0))
    {
      SeaoryManager.ResetPrinter(SeaoryManager.ResetLevel.HARD);
    }

    if (Input.GetKeyDown(KeyCode.Keypad1)) { SeaoryManager.MoveCard2(SeaoryManager.CardPosition.HOPPER); }

    if (Input.GetKeyDown(KeyCode.Keypad2))
    {
      SeaoryManager.Instance.PrintImage(@$"{Application.streamingAssetsPath}/l.png");
    }
    if (Input.GetKeyDown(KeyCode.Keypad3))
    {
      SeaoryManager.Instance.PrintImage(@$"{Application.streamingAssetsPath}/p.png");
    }
  }



  private void DEBUG_OnGUI()
  {
    if (GUILayout.Button("移动卡片到出卡盒"))
    {
      SeaoryManager.MoveCard2(SeaoryManager.CardPosition.HOPPER);
    }
    if (GUILayout.Button("移动卡片到准备区"))
    {
      SeaoryManager.MoveCard2(SeaoryManager.CardPosition.PREPARE);
    }

    if (GUILayout.Button("重置打印机"))
    {
      uint code = SeaoryManager.ResetPrinter(SeaoryManager.ResetLevel.HARD);
      Debug.Log(code);
    }

    if (GUILayout.Button("色带余量"))
    {
      string log = SeaoryManager.GetRibbonCount();
      Debug.Log(log);
    }

    // if(GUILayout.Button(""))

    // GUILayout.Space(30);
    // if (GUILayout.Button("尝试打印"))
    // {
    //   StartPrinting();
    // }
  }
}
