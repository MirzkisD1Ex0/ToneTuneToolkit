/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
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
      ProcessStartInfo psi = new ProcessStartInfo
      {
        FileName = command,
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true
      };

      p.StartInfo = psi;
      p.Start();
      return;
    }
  }
}
