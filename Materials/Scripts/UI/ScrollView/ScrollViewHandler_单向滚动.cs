/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 需要反哺
/// 单向滚动
/// 垂直向下 水平向右
/// </summary>
public class ScrollViewHandler : MonoBehaviour
{
  private ScrollRect sr;
  private const float ANIMTIME = .5f;



  [SerializeField] private bool isVertical = false;



  private int srContentCount;
  private float unitPosition;
  [SerializeField] private int currentIndex = 0;

  // ==================================================

  private void Start() => Init();

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Alpha0)) { Jump2Position(0); }
    if (Input.GetKeyDown(KeyCode.Alpha1)) { Jump2Position(1); }
    if (Input.GetKeyDown(KeyCode.Alpha2)) { Jump2Position(2); }
    if (Input.GetKeyDown(KeyCode.Alpha3)) { Jump2Position(3); }
  }

  // ==================================================

  private void Init()
  {
    sr = GetComponent<ScrollRect>();
    srContentCount = transform.GetChild(0).GetChild(0).childCount;
    unitPosition = 1f / (srContentCount - 1);

    if (startAutoLoop) { SwitchAutoLoop(true); }
  }

  // ==================================================
  #region 跳转控制

  public void Jump2Position(int index) => StartCoroutine(Jump2PositionCoroutine(index));

  private IEnumerator Jump2PositionCoroutine(int index)
  {
    // if (index <= 0)
    // {
    //   currentIndex = 0;
    //   if (isVertical) { sr.DOVerticalNormalizedPos(0, ANIMTIME); }
    //   else { sr.DOHorizontalNormalizedPos(0, ANIMTIME); }
    //   return;
    // }
    // // if (index >= (srContentCount - 1))
    // // {
    // //   currentIndex = 0;
    // //   if (isVertical) { sr.DOVerticalNormalizedPos(unitPosition * index, ANIMTIME); }
    // //   else { sr.DOHorizontalNormalizedPos(unitPosition * index, ANIMTIME); }
    // //   yield return new WaitForSeconds(ANIMTIME);
    // //   if (isVertical) { sr.verticalNormalizedPosition = 0; }
    // //   else { sr.horizontalNormalizedPosition = 0; }
    // //   yield break;
    // // }

    currentIndex = index;
    if (isVertical) { sr.DOVerticalNormalizedPos(1 - (unitPosition * index), ANIMTIME); }
    else { sr.DOHorizontalNormalizedPos(unitPosition * index, ANIMTIME); }
    // Debug.Log(@$"[SVH] index={currentIndex},position={unitPosition * index},count={srContentCount},unit={unitPosition}");

    // if (index <= 0)
    // {
    //   currentIndex = 0;
    //   if (isVertical) { sr.DOVerticalNormalizedPos(0, ANIMTIME); }
    //   else { sr.DOHorizontalNormalizedPos(0, ANIMTIME); }
    //   yield break;
    // }

    if (index >= (srContentCount - 1)) // 超出后归零
    {
      yield return new WaitForSeconds(ANIMTIME);
      if (isVertical) { sr.verticalNormalizedPosition = 1; }
      else { sr.horizontalNormalizedPosition = 0; }
      currentIndex = 0;
      yield break;
    }
  }

  #endregion
  // ==================================================
  #region Auto Loop

  [Space]
  [SerializeField] private bool startAutoLoop = true;
  [SerializeField] private float autoLoopSpaceTime = 3f;

  public void SwitchAutoLoop(bool value)
  {
    if (value) { StartCoroutine(AutoLoopCoroutine()); }
    else { StopAllCoroutines(); }
  }

  private int autoLoopIndex = 1; // 最好从1开始 // 因为到最大值后回重置到0
  private IEnumerator AutoLoopCoroutine()
  {
    while (true)
    {
      autoLoopIndex++;
      if (autoLoopIndex >= srContentCount) { autoLoopIndex = 1; }
      Jump2Position(autoLoopIndex);
      yield return new WaitForSeconds(autoLoopSpaceTime);
    }
  }

  #endregion
}