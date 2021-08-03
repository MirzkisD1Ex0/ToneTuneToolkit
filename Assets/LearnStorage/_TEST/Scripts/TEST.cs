using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ToneTuneToolkit.Common;

/// <summary>
/// 
/// </summary>
public class TEST : MonoBehaviour
{
  private EventListener<int> listenerTest = new EventListener<int>();

  private void Start()
  {
    listenerTest.OnValueChange += Test; // 发报纸给Test
  }

  private void OnDestroy()
  {
    listenerTest.OnValueChange -= Test; // 取消订阅
  }

  private void Test(int value)
  {
    Debug.Log(value);
  }

  public void GG()
  {
    listenerTest.Value += 1;
  }
}