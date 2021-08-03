using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LearnStorage
{
  /// <summary>
  /// 
  /// </summary>
  public class T02 : MonoBehaviour
  {
    [Range(0, 10)]
    public int Level = 0;

    private PlayerStatus ps = new PlayerStatus();

    private void Start()
    {
      Debug.Log(ps.Health);
      ps.Health -= 10;
      Debug.Log(ps.Health);
    }

    // 无MonoBehaviour
    // C#数据类
    // 字段
    // 属性
    // 构造函数
    // 方法

    // 脚本
    // 字段
    // 方法

    // 避免在脚本中写构造函数
  }
}