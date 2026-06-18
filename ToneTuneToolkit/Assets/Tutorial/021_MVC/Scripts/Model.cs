using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Model : MonoBehaviour
{
  public static Model Instance;

  private string theData;
  public string TheData
  {
    get { return theData; }
  }

  // 通知外部的事件
  private event UnityAction<string> OnDataUpdate;

  // ==================================================

  private void Awake()
  {
    Instance = this;
  }

  // ==================================================

  public void AddEventListener(UnityAction<string> unityAction)
  {
    OnDataUpdate += unityAction;
    return;
  }

  public void RemoveEventListener(UnityAction<string> unityAction)
  {
    OnDataUpdate -= unityAction;
    return;
  }

  private void NoticeAll()
  {
    if (OnDataUpdate == null) // 如果没人订阅
    {
      return;
    }
    OnDataUpdate(theData); // 把数据丢出去
    return;
  }

  // ==================================================

  public void UpdateTheFuckingData(string value)
  {
    theData = value;
    NoticeAll(); // 提醒订阅者
    return;
  }
}