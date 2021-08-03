using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class DelegateScript : MonoBehaviour
  {
    private delegate void DelegateA(int num);
    private delegate void DelegateB(int numa, int numb);
    DelegateA da;
    DelegateB db;

    private void Start()
    {
      // 委托类型DelegateA实例da引用方法PrintNum
      da = PrintNum;
      da(50);

      // 传两个参自动重载
      db = PrintNum;
      db(50, 50);
    }

    private void PrintNum(int num)
    {
      Debug.Log("Print Num: " + num);
    }

    private void PrintNum(int numa, int numb)
    {
      Debug.Log("Double Num: " + numa * numb);
    }
  }
}