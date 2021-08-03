using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class Delegate01 : MonoBehaviour
  {
    private delegate void TheDelegate(string st); // 定义一个委托
    private TheDelegate handler; // 委托变量事件

    public void ButtonTriggerA()
    {
      handler = FunctionA; // 指向FunctionA
      // handler += FunctionA; // 如果是+= // 会添加一个事件 // 添加
      handler("AAAAAAAAAAA");
      return;
    }

    public void ButtonTriggerB()
    {
      handler = FunctionB;
      handler("BBBBBB");
      return;
    }

    private void FunctionA(string value)
    {
      Debug.Log("Function A " + value);
      return;
    }

    private void FunctionB(string value)
    {
      Debug.Log("Function B " + value);
      return;
    }
  }
}