using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class TEST : MonoBehaviour
{
  private EventListener<bool> listenerTest;

  private void Start()
  {
    listenerTest = new EventListener<bool>();

    listenerTest.OnVariableChange += Test; // 发报纸给Test
  }

  private void OnDestroy()
  {
    listenerTest.OnVariableChange -= Test; // 取消订阅
  }

  private void Update()
  {
    listenerTest.Value = Input.GetKey(KeyCode.W);
  }

  private void Test(bool value)
  {
    Debug.Log(value);
  }
}

public class EventListener<T>
{
  public delegate void OnValueChangeDelegate(T newVal);
  public event OnValueChangeDelegate OnVariableChange;
  private T m_value;
  public T Value
  {
    get
    {
      return m_value;
    }
    set
    {
      if (m_value.Equals(value))
      {
        return;
      }
      OnVariableChange?.Invoke(value);
      m_value = value;
    }
  }
}