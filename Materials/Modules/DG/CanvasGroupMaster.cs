using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

/// <summary>
/// CanvasGroup工具
/// </summary>
public class CanvasGroupMaster : SingletonMaster<CanvasGroupMaster>
{
  private const float ANIMTIME = 0.33f;

  // ==================================================

  public static void DoFade(CanvasGroup cg, bool isFadeIn)
  {
    if (isFadeIn)
    {
      cg.DOFade(1, ANIMTIME).OnComplete(() =>
      {
        cg.interactable = true;
        cg.blocksRaycasts = true;
      });
    }
    else
    {
      cg.interactable = false;
      cg.DOFade(0, ANIMTIME).OnComplete(() =>
      {
        cg.blocksRaycasts = false;
      });
    }
  }

  public static void DoFade(CanvasGroup cg, bool isFadeIn, float time)
  {
    if (isFadeIn)
    {
      cg.DOFade(1, time).OnComplete(() =>
      {
        cg.interactable = true;
        cg.blocksRaycasts = true;
      });
    }
    else
    {
      cg.interactable = false;
      cg.DOFade(0, time).OnComplete(() =>
      {
        cg.blocksRaycasts = false;
      });
    }
  }

  public async static void DoFade(CanvasGroup cg, bool isFadeIn, float time, float delayTime)
  {
    await Task.Delay((int)(1000 * delayTime));

    if (isFadeIn)
    {
      cg.DOFade(1, time).OnComplete(() =>
      {
        cg.interactable = true;
        cg.blocksRaycasts = true;
      });
    }
    else
    {
      cg.interactable = false;
      cg.DOFade(0, time).OnComplete(() =>
      {
        cg.blocksRaycasts = false;
      });
    }
  }
}