using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceFrameManager : MonoBehaviour
{
  public static SequenceFrameManager Instance;

  // ==================================================

  private void Awake() => Instance = this;

  // ==================================================

  public void ResetAll()
  {
    SwitchSequenceFrameAnimation(-1,false);
    return;
  }

  // ==================================================

  public List<GameObject> sequenceFrames;

  public void SwitchSequenceFrameAnimation(int index, bool isPlay)
  {
    if (index == -1) // -1全部播放 // 或全部关闭
    {
      foreach (var item in sequenceFrames)
      {
        item.GetComponent<SequenceFrameHandler>().SwitchAnimation(isPlay);
      }
      return;
    }
    sequenceFrames[index].GetComponent<SequenceFrameHandler>().SwitchAnimation(isPlay);
    return;
  }
}