using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.Common;

public class Test : MonoBehaviour
{
  private void Start()
  {
    string[] st = FileNameCapturer.GetFileName(ToolkitManager.ConfigsPath, ".json");

    foreach (string item in st)
    {
      Debug.Log(item);
    }
  }
}