using UnityEngine;
using ToneTuneToolkit.Common;
using System.Collections.Generic;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class FNC : MonoBehaviour
  {
    private void Start()
    {
      List<string> fileNames = FileCapturer.GetFileName2List(ToolkitManager.ConfigsPath, ".json");

      foreach (string item in fileNames)
      {
        TTTDebug.Log(item);
      }
    }
  }
}