/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using DG.Tweening;
using UnityEngine;

namespace ToneTuneToolkit.UI
{
  /// <summary>
  /// 控件闪烁
  /// 需要挂在对象上
  /// </summary>
  [RequireComponent(typeof(CanvasGroup))]
  public class UGUIFlick : MonoBehaviour
  {
    [Range(0f, 1)] public float MinAlpha = 0.2f; // 最小透明度
    [Range(0f, 1)] public float MaxAlpha = 1f; // 最大透明度
    private float anim_duration = 1f; // 速度
    private CanvasGroup cg;
    private Tween tween;

    // ==================================================

    private void Start() => Init();

    // ==================================================

    private void Init()
    {
      cg = GetComponent<CanvasGroup>();
      cg.alpha = MinAlpha;
      SetAnimDuration(anim_duration);
    }

    // ==================================================

    public void SetAnimDuration(float value)
    {
      anim_duration = value;
      tween.Kill();
      cg.alpha = MinAlpha;
      tween = cg.DOFade(MaxAlpha, anim_duration).SetLoops(-1, LoopType.Yoyo);
    }

    // ==================================================

    public void End2(float value)
    {
      tween.Kill();
      tween = cg.DOFade(value, anim_duration);
    }
  }
}
