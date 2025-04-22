using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

namespace ToneTuneToolkit.UI
{
  /// <summary>
  /// 滚动视图助手
  /// </summary>
  public class ScrollViewHandler : MonoBehaviour
  {
    public UnityAction<int> OnScrollViewStopped;

    private ScrollRect sv;
    private CanvasGroup cg;

    [SerializeField] private int currentContentIndex = 0;
    private Vector2 scrollviewLocation;

    private float stopThreshold = 50f; // 速度阈值，小于该值认为停止
    private float checkInterval = 0.1f; // 检测间隔（秒）
    private bool isScrolling;
    private float lastCheckTime;

    private const float ANIMTIME = .33f;

    // ==================================================

    private void Start() => Init();
    private void Update() => ScrollRollDetect();

    // ==================================================

    private void Init()
    {
      sv = GetComponent<ScrollRect>();
      cg = GetComponent<CanvasGroup>();
      return;
    }

    private void Reset()
    {
      currentContentIndex = 0;
      sv.horizontalNormalizedPosition = 0f;
      return;
    }

    // ==================================================

    public void SwitchInteractable(bool value)
    {
      if (sv == null)
      {
        return;
      }
      cg.interactable = value;
      cg.blocksRaycasts = value;
      return;
    }

    /// <summary>
    /// 添加到OnValueChanged中
    /// </summary>
    /// <param name="value"></param>
    public void GetVector2Location(Vector2 value)
    {
      scrollviewLocation = value;
      return;
    }



    /// <summary>
    /// 滚动状态检测
    /// </summary>
    private void ScrollRollDetect()
    {
      if (!sv)
      {
        return;
      }

      if (sv.velocity.sqrMagnitude > stopThreshold * stopThreshold)
      {
        isScrolling = true;
        lastCheckTime = Time.time;
      }
      else if (isScrolling && Time.time - lastCheckTime > checkInterval)
      {
        isScrolling = false;
        AdjustView(); // 视图矫正
      }
      return;
    }

    /// <summary>
    /// 矫正视图位置
    /// </summary>
    public void AdjustView()
    {
      int newContentIndex = (int)Math.Round(scrollviewLocation.x, 0);

      sv.horizontal = false;
      sv.DOHorizontalNormalizedPos(newContentIndex, ANIMTIME).OnComplete(() =>
        {
          sv.horizontal = true;

          if (currentContentIndex == newContentIndex) // 无变化
          {
            return;
          }
          else
          {
            currentContentIndex = newContentIndex;
            if (OnScrollViewStopped != null)
            {
              OnScrollViewStopped(newContentIndex);
            }
          }
        });
      return;
    }

    /// <summary>
    /// 滚动到横向指定位置
    /// </summary>
    public void Scroll2HorizontalPosition(float normalizedPosition)
    {
      sv.DOHorizontalNormalizedPos(normalizedPosition, ANIMTIME).OnComplete(() =>
      {
        AdjustView(); // 视图矫正
      });
      return;
    }
  }
}