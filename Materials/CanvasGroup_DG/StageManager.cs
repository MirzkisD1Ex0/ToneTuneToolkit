using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;

/// <summary>
/// 阶段管理工具
/// </summary>
public class StageManager : SingletonMaster<StageManager>
{
  [Header("Stages")]
  [SerializeField] private List<CanvasGroup> cgStages = new List<CanvasGroup>();

  // ==================================================

  public void Reset()
  {
    SwitchStageTo(0);
    return;
  }

  // ==================================================

  public void SwitchStageTo(int stageIndex)
  {
    for (int i = 0; i < cgStages.Count; i++)
    {
      if (i == stageIndex)
      {
        CanvasGroupMaster.DoCanvasGroupFade(cgStages[i], true);
      }
      else
      {
        CanvasGroupMaster.DoCanvasGroupFade(cgStages[i], false);
      }
    }
    return;
  }

  public void SingleSwitchStage(int stageIndex, bool inOrOut)
  {
    CanvasGroupMaster.DoCanvasGroupFade(cgStages[stageIndex], inOrOut);
    return;
  }
}