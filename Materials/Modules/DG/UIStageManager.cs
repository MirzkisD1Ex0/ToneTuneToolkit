using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;

/// <summary>
/// 阶段管理工具
/// </summary>
public class UIStageManager : SingletonMaster<UIStageManager>
{
  [SerializeField] private List<CanvasGroup> cgStages = new List<CanvasGroup>();

  // ==================================================

  public void Reset()
  {
    SwitchStage2(0);
    return;
  }

  // ==================================================

  public void SwitchStage2(int stageIndex)
  {
    for (int i = 0; i < cgStages.Count; i++)
    {
      if (i == stageIndex)
      {
        CanvasGroupMaster.DoFade(cgStages[i], true);
      }
      else
      {
        CanvasGroupMaster.DoFade(cgStages[i], false);
      }
    }
    return;
  }

  public void SingleSwitchStage(int stageIndex, bool inOrOut)
  {
    CanvasGroupMaster.DoFade(cgStages[stageIndex], inOrOut);
    return;
  }
}