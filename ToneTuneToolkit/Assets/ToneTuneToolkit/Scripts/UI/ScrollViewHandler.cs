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

    [SerializeField] private CanvasGroup cgBlocker;

    public int currentIndex = 0;
    private Vector2 scrollviewLocation;

    private float stopThreshold = 100f; // 速度阈值，小于该值认为停止 // 惯性功能所迫
    private float checkInterval = 0.1f; // 检测间隔（秒）
    private bool isScrolling;
    private float lastCheckTime;

    private float[] anchorPositions; // 锚点位置
    private float cellDistance; // 单元距离

    private const float ANIMTIME = .33f;

    // ==================================================

    private void Start() => Init();
    private void Update() => ScrollRollDetect();

    // ==================================================

    private void Init()
    {
      sv = GetComponent<ScrollRect>();
      cg = GetComponent<CanvasGroup>();

      cellDistance = 1 / ((float)sv.content.childCount - 1);
      anchorPositions = new float[sv.content.childCount];
      for (int i = 0; i < anchorPositions.Length; i++)
      {
        anchorPositions[i] = cellDistance * i;
      }

      return;
    }

    private void Reset()
    {
      currentIndex = 0;
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
      // Debug.Log(scrollviewLocation.x);
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
      int newIndex = 0;
      float min = Mathf.Abs(scrollviewLocation.x - anchorPositions[0]);
      for (int i = 1; i < anchorPositions.Length; i++)
      {
        float d = Mathf.Abs(scrollviewLocation.x - anchorPositions[i]);
        if (d < min)
        {
          min = d;
          newIndex = i;
        }
      }

      sv.horizontal = false;
      sv.DOHorizontalNormalizedPos(newIndex * cellDistance, ANIMTIME).OnComplete(() =>
        {
          sv.horizontal = true;
          if (cgBlocker)
          {
            cgBlocker.blocksRaycasts = false;
          }

          if (currentIndex != newIndex) // 有变化
          {
            currentIndex = newIndex;
            if (OnScrollViewStopped != null)
            {
              OnScrollViewStopped(newIndex);
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
      if (cgBlocker)
      {
        cgBlocker.blocksRaycasts = true;
      }

      sv.DOHorizontalNormalizedPos(normalizedPosition, ANIMTIME).OnComplete(() =>
      {
        // AdjustView(); // 视图矫正
        if (cgBlocker)
        {
          cgBlocker.blocksRaycasts = false;
        }
      });
      return;
    }
  }
}