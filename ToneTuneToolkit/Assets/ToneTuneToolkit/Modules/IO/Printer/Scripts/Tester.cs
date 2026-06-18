/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.IO.Printer
{
  public class Tester : MonoBehaviour
  {
    [SerializeField] private Texture2D t2dTest;

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.A)) { PrinterManager.Instance.Print(t2dTest); }
    }
  }
}
