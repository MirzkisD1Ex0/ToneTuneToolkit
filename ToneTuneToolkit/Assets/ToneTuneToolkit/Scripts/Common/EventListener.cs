/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// 事件监听泛型参数类
  ///
  /// Value
  /// private EventListener<int> listenerTest = new EventListener<int>();
  /// listenerTest.OnValueChange += Test; // Test一个方法
  /// listenerTest.Value += 1; // 改变时Test执行
  /// </summary>
  public class EventListener<T> // 泛型用T // 约定俗成?
  {
    public delegate void OnValueChangeDelegate(T newValue); // 委托 类型也可以改成int等等
    public event OnValueChangeDelegate OnValueChange; // 事件 // 如同按钮的onClick
    private T valueStorage;
    public T Value
    {
      get
      {
        return valueStorage; // 返回储存的数值
      }
      set
      {
        if (valueStorage.Equals(value)) // 如果新进值和储存值相等
        {
          return;
        }
        OnValueChange?.Invoke(value); // C#6新语法 // 空值传播运算符
        valueStorage = value;
      }
    }
  }
}
