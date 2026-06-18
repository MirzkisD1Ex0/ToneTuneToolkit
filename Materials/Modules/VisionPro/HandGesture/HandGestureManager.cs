/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using ToneTuneToolkit.Common;
using UnityEngine;

public class HandGestureManager : SingletonMaster<HandGestureManager>
{
  public float RequiredDuration = 1f;

  // 手势状态
  private bool isLeftThumbUp;
  private bool isRightThumbUp;

  // 计时相关
  private float bothThumbsUpStartTime;
  private bool isCounting;

  // ==================================================

  private void Update() => HandleBothThumbsUpDuration();

  private void OnDisable()
  {
    ResetCounting();
  }

  // ==================================================

  public void OnActiveLeftHandThumbUP() => isLeftThumbUp = true;
  public void OnDeactiveLeftHandThumbUP() => isLeftThumbUp = false;

  public void OnActiveRightHandThumbUP() => isRightThumbUp = true;
  public void OnDeactiveRightHandThumbUP() => isRightThumbUp = false;

  // ==================================================

  private void HandleBothThumbsUpDuration()
  {
    if (isLeftThumbUp && isRightThumbUp)
    {
      if (!isCounting)
      {
        StartCounting();
        return;
      }

      if (Time.time - bothThumbsUpStartTime >= RequiredDuration)
      {
        TriggerBothThumbsUp();
      }
    }
    else if (isCounting)
    {
      ResetCounting();
    }
  }

  private void StartCounting()
  {
    bothThumbsUpStartTime = Time.time;
    isCounting = true;
  }

  private void TriggerBothThumbsUp()
  {
    Debug.Log("[HGM] Keep your thumbs up for 2 seconds!");
    ResetCounting();
    ARAnchorSetter.Instance.ResetPolySpatialAnchors();
  }

  private void ResetCounting()
  {
    isLeftThumbUp = false;
    isRightThumbUp = false;
    isCounting = false;
    Debug.Log("[HGM] Reset hand thumbs up status!");
  }
}
