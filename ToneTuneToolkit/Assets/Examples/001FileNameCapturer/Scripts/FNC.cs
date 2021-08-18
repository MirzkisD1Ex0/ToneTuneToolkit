using UnityEngine;
using ToneTuneToolkit.Common;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class FNC : MonoBehaviour
  {
    private void Start()
    {
      string[] fileNames = FileNameCapturer.GetFileName(ToolkitManager.ConfigsPath, ".json");

      foreach (string item in fileNames)
      {
        TipTools.Notice(item);
      }
    }
  }
}