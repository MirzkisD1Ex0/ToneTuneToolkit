using UnityEngine;
using System;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class FunctionFormalParameter : MonoBehaviour
  {
    private void Start()
    {
      Trigger(PrintString);
    }

    /// <summary>
    /// 带有函数形参的函数
    /// </summary>
    /// <param name="formalMethod"></param>
    private void Trigger(Action<string> formalMethod)
    {
      formalMethod("printsomething");
      return;
    }

    /// <summary>
    /// 被传入的形参函数
    /// </summary>
    private void PrintString(string message)
    {
      Debug.Log("FormalParameter:" + message);
      return;
    }
  }
}