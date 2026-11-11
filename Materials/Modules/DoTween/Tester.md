using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 蜻蜓队长模板
/// </summary>
public class Tester : MonoBehaviour
{
  private void Update() => DEBUG_Shortcut();

  // ============================================================

  private void DEBUG_Shortcut()
  {
    // if (Input.GetKeyDown(KeyCode.A)) { EnterLoading(); }
    // if (Input.GetKeyDown(KeyCode.S)) { EnterSticker(); }
    // if (Input.GetKeyDown(KeyCode.D)) { CellSetter.Instance.Init(); }
    // if (Input.GetKeyDown(KeyCode.F)) { LoadingManager.Instance.TestUseScreenshot(); }
    // if (Input.GetKeyDown(KeyCode.Z)) { ShootingManager.Instance.DEBUG_WhenCamFiShotFinished(); }
  }
}