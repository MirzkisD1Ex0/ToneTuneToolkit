using UnityEngine;
using ToneTuneToolkit.Other;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class CMDL : MonoBehaviour
  {
    private void Start()
    {
      CMDLauncher.LaunchProcess("notepad.exe");
    }
  }
}