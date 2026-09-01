/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.IO.Canon
{
  /// <summary>
  /// 单反相机桥启动器
  /// </summary>
  public class DslrBridgeRunner : SingletonMaster<DslrBridgeRunner>
  {
    private string appPath = @$"{Application.streamingAssetsPath}/DSLRBridge/start.bat";
    private Process process;

    // 用于向 DSLRBridge.exe 发送 CTRL_BREAK，等价于在它终端里按下 Ctrl+C
    private const uint CTRL_BREAK_EVENT = 1;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(IntPtr handler, bool add);

    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    private void Init()
    {
      RunDSLRBridge();
    }

    private void UnInit()
    {
      KillProcessTree();
    }

    public void Reset() => StartCoroutine(ResetDelay());
    private IEnumerator ResetDelay()
    {
      UnInit();
      yield return new WaitForSeconds(3f);
      Init();
    }

    // ==================================================

    private void RunDSLRBridge()
    {
      string dir = Path.GetDirectoryName(appPath);
      string exePath = Path.Combine(dir, "DSLRBridge.exe");
      string photosPath = Path.Combine(dir, "output", "photos");

      if (!File.Exists(exePath))
      {
        UnityEngine.Debug.LogError($"DSLRBridge 可执行文件不存在：{exePath}");
        process = null;
        return;
      }

      // 直接启动 DSLRBridge.exe，等价于执行 start.bat 里的那行命令。
      // 之所以不通过 cmd.exe /c 启动：关闭时 cmd 会自己拦截 Ctrl+C，
      // 弹 "Terminate batch job (Y/N)?" 卡住，导致无法优雅退出。
      // 注意：Arguments 需与 StreamingAssets/DSLRBridge/start.bat 保持一致。
      process = new Process();
      process.StartInfo.FileName = exePath;
      process.StartInfo.WorkingDirectory = dir;
      process.StartInfo.Arguments = $"--backend edsdk --http-port 8080 --photos \"{photosPath}\"";
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      process.OutputDataReceived += OnOuputDataReceived;
      process.ErrorDataReceived += OnOuputDataReceived;
      process.Start();
      UnityEngine.Debug.Log($"DSLRBridge 已启动（PID {process.Id}）：{exePath}");
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
    }

    private void KillProcessTree()
    {
      if (process == null)
      {
        return;
      }

      try
      {
        if (!process.HasExited)
        {
          // 先发 CTRL_BREAK（等价于在它终端里按 Ctrl+C），让 DSLRBridge.exe
          // 自己跑清理逻辑、正常释放相机。
          SendCtrlBreak(process.Id);

          // 最多等 3 秒；超时说明它没响应，走强杀兜底。
          if (!process.WaitForExit(3000))
          {
            KillByTaskkill();
          }
        }
      }
      catch { }

      try
      {
        process.Dispose();
      }
      catch { }

      process = null;
    }

    private void SendCtrlBreak(int pid)
    {
      try
      {
        FreeConsole();
        if (!AttachConsole((uint)pid))
        {
          return;
        }

        SetConsoleCtrlHandler(IntPtr.Zero, true);
        GenerateConsoleCtrlEvent(CTRL_BREAK_EVENT, (uint)pid);
        FreeConsole();
      }
      catch { }
    }

    private void KillByTaskkill()
    {
      try
      {
        ProcessStartInfo psi = new ProcessStartInfo("taskkill", $"/F /T /PID {process.Id}")
        {
          UseShellExecute = false,
          CreateNoWindow = true
        };
        Process.Start(psi).WaitForExit(2000);
      }
      catch { }
    }

    private void OnOuputDataReceived(object sender, DataReceivedEventArgs args)
    {
      if (string.IsNullOrEmpty(args.Data))
      {
        return;
      }

      UnityEngine.Debug.Log(args.Data);
    }
  }
}
