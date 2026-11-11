using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.DoTween;
using ToneTuneToolkit.Common;

/// <summary>
/// 进程管理器模板
/// </summary>
public class GameManager : SingletonMaster<GameManager>
{
  [SerializeField] private bool isDebug = false;
  public static bool IsDebug = false;

  // ==================================================

  private void Start() => Init();
  private void OnDestroy() => UnInit();

  // ==================================================

  private void Init()
  {
    IsDebug = isDebug;
    // AFKDetector.OnAFK += WhenAFK;
  }

  private void UnInit()
  {
    // AFKDetector.OnAFK -= WhenAFK;
  }

  public void Reset()
  {
    EnterStandby();
    // ShootingManager.Instance.Reset();
    // StickerManager.Instance.Reset();
    // LoadingManager.Instance.Reset();

    StopAllCoroutines();
  }

  public void WhenAFK(bool value)
  {
    if (!value) { return; }
    if (isStandby) { return; }
    Reset();
  }

  // ==================================================

  private bool isStandby = true;
  public void EnterStandby()
  {
    StageManager.Instance.SwitchStage2(0);
    isStandby = true;
  }

  public void EnterIntro()
  {
    StageManager.Instance.SwitchStage2(1);
    isStandby = false;
  }

  public void EnterShooting()
  {
    // ShootingManager.Instance.Reset();
    StageManager.Instance.SwitchStage2(2);
  }

  public void EnterSticker()
  {
    // StickerManager.Instance.Preset();
    StageManager.Instance.SwitchStage2(3);
  }

  public void EnterLoading()
  {
    // LoadingManager.Instance.Preset();
    StageManager.Instance.SwitchStage2(4);
  }

  public void EnterQR()
  {
    StageManager.Instance.SwitchStage2(5);
  }

  // ============================================================

  private Coroutine resetCancelCoroutine;

  private int resetIndex = 0;
  public void ForceReset()
  {
    resetIndex++;
    if (resetIndex >= 3)
    {
      Reset();
      resetIndex = 0;
      return;
    }

    if (resetCancelCoroutine != null) { StopCoroutine(resetCancelCoroutine); }
    resetCancelCoroutine = StartCoroutine(ResetCancelCoroutine());
  }

  private IEnumerator ResetCancelCoroutine()
  {
    yield return new WaitForSeconds(3f);
    resetIndex = 0;
    resetCancelCoroutine = null;
  }
}