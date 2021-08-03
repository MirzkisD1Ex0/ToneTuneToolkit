using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LearnStorage
{
  /// <summary>
  /// 如果从开始至终都没有使用过这个实例，则会造成内存的浪费
  /// </summary>
  public class ST
  {
    private static ST instance;

    private ST() // 不加这句构造函数将为public // 可以从外部new
    {
      instance = new ST(); // 将实例化放在私有构造函数中 // 饿汉式 // 不管用不用立刻构造一个
    }

    public static ST GetST()
    {
      // if (instance == null) // 懒汉式 // 不get不生成 // 多线程同时访问可能造成实例化多个
      // {
      //   instance = new ST();
      // }
      return instance;
    }
  }
}

// private ST st = ST.GetST(); // 调用