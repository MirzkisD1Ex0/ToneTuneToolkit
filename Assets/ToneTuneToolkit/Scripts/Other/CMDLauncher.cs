using UnityEngine;
using System.Diagnostics;

namespace ToneTuneToolkit.Other
{
  /// <summary>
  /// OK
  /// CMD命令行启动
  /// </summary>
  public class CMDLauncher : MonoBehaviour
  {
    /// <summary>
    /// 启动CMD命令
    /// </summary>
    /// <param name="command">notepad.exe</param>
    public static void LaunchProcess(string command)
    {
      Process p = new Process();
      ProcessStartInfo psi = new ProcessStartInfo();
      psi.FileName = command;
      psi.UseShellExecute = false;
      psi.RedirectStandardError = true;
      psi.RedirectStandardInput = true;
      psi.RedirectStandardOutput = true;
      psi.CreateNoWindow = true;

      p.StartInfo = psi;
      p.Start();
      return;
    }
  }
}