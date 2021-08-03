using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LearnStorage
{
  /// <summary>
  /// 
  /// </summary>
  public class SingletonTEST : MonoBehaviour
  {

    private ST st = ST.GetST();

    private void Start()
    {

    }
  }
}