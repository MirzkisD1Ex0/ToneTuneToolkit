using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LearnStorage
{
  /// <summary>
  /// 
  /// </summary>
  public class Counter : MonoBehaviour
  {
    public Text dt;
    public int time = 120;
    public float index = 1;

    private void Update()
    {
      if (Time.time >= index)
      {
        time--;
        dt.text = string.Format("{0:d2}:{1:d2}", time / 60, time % 60);
        index = Time.time + 1;
      }
    }
  }
}