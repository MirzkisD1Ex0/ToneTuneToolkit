/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;

namespace ToneTuneToolkit.DoTween
{
  /// <summary>
  /// 阶段管理工具
  /// </summary>
  public class UIStageManager : SingletonMaster<UIStageManager>
  {
    [SerializeField] private List<CanvasGroup> cgStages = new List<CanvasGroup>();

    // ==================================================

    public void SwitchStage2(int stageIndex)
    {
      for (int i = 0; i < cgStages.Count; i++)
      {
        CanvasGroupMaster.DoFade(cgStages[i], i == stageIndex);
      }
    }

    public void SingleSwitchStage(int stageIndex, bool inOrOut)
    {
      CanvasGroupMaster.DoFade(cgStages[stageIndex], inOrOut);
    }
  }
}
