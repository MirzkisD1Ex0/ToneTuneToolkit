/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using System.Diagnostics;

namespace ToneTuneToolkit.Other
{
  /// <summary>
  /// CMD命令行
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