/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.1
/// </summary>
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ToneTuneToolkit.Common;
using UnityEngine;

public class WhiteScreenHelper : SingletonMaster<WhiteScreenHelper>
{
  private static CanvasGroup cg;

  private const float flashUpDuration = 0.08f; // 亮起时长（要"快"）
  private const float flashPeakAlpha = 0.66f; // 白闪峰值
  private const float flashDecayDuration = 0.32f; // 回落时长（要"慢"）

  // ==================================================

  private void Start() => Init();

  // ==================================================

  private void Init()
  {
    cg = GetComponent<CanvasGroup>();
    cg.interactable = false;
    cg.blocksRaycasts = false;
  }

  // ==================================================

  /// <summary>
  /// 拍照白闪：Alpha 0 → 0.5 快速亮起 → 缓慢回落到 0，模拟相机闪光
  /// 重复调用安全：上一次未完成的闪会被打断重来
  /// </summary>
  public static void QuickFlash()
  {
    cg.DOKill(); // 打断残留 tween，防止两次白闪抢同一属性
    cg.alpha = 0f;
    DOTween.Sequence()
      .Append(cg.DOFade(flashPeakAlpha, flashUpDuration).SetEase(Ease.OutQuad)) // 快速亮起
      .Append(cg.DOFade(0f, flashDecayDuration).SetEase(Ease.OutQuad)); // 慢速回落（OutQuad 快出慢收，接近闪光余晖的指数衰减）
  }
}
