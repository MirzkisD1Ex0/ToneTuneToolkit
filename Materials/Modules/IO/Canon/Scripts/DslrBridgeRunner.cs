/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.IO.Canon
{
  /// <summary>
  /// 单反相机桥启动器
  /// </summary>
  public class DslrBridgeRunner : SingletonMaster<DslrBridgeRunner>
  {
    private string appPath = @$"{Application.streamingAssetsPath}/DSLRBridge/bin/pixo_dslr_bridge.exe";
    private Process process;

    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    private void Init()
    {
      RunDSLRBridge();
      process.OutputDataReceived += OnOuputDataReceived;
    }

    private void UnInit()
    {
      process.OutputDataReceived -= OnOuputDataReceived;

      try
      {
        process.Kill();
      }
      catch { }
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
      process = new Process();
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.FileName = appPath;
      process.StartInfo.CreateNoWindow = true;
      process.Start();

    }

    private void OnOuputDataReceived(object sender, DataReceivedEventArgs args)
    {
      UnityEngine.Debug.Log(args.Data);
    }
  }
}
