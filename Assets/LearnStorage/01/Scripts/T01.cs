using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LearnStorage
{
  /// <summary>
  /// 
  /// </summary>
  public class T01 : MonoBehaviour
  {
    private Text debugTCmpt;

    private void Start()
    {
      debugTCmpt = GameObject.Find("Text").GetComponent<Text>();

      Draw(10);
    }

    private void Draw(int level)
    {
      for (int i = 0; i < level; i++)
      {
        for (int j = 0; j < i; j++)
        {
          debugTCmpt.text += "0";
        }
        for (int j = 0; j < level - i; j++)
        {
          debugTCmpt.text += "#";
        }
        debugTCmpt.text += "\n";
      }
      return;
    }
  }
}