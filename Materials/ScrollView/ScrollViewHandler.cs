using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

namespace VolkswagenIDUNYXMusicPlayer
{
  public class ScrollViewHandler : MonoBehaviour
  {
    public static ScrollViewHandler Instance;
    public UnityAction<int> OnContentIndexChange;

    private ScrollRect scrollRect;
    private Vector2 scrollviewLocation;
    private int currentContentIndex = 0;
    private const float animTime = .66f;

    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      Init();
    }

    // ==================================================

    private void Init()
    {
      scrollRect = GetComponent<ScrollRect>();
      return;
    }



    public void GetVector2Location(Vector2 value)
    {
      scrollviewLocation = value;
      return;
    }

    public void AdjustView()
    {
      int newContentIndex = (int)Math.Round(scrollviewLocation.x, 0);

      scrollRect.horizontal = false;
      scrollRect.DOHorizontalNormalizedPos(newContentIndex, animTime).OnComplete(() =>
        {
          scrollRect.horizontal = true;

          if (currentContentIndex == newContentIndex) // 无变化
          {
            return;
          }
          else
          {
            currentContentIndex = newContentIndex;
            if (OnContentIndexChange != null)
            {
              OnContentIndexChange(newContentIndex);
            }
          }
        });
      return;
    }
  }
}