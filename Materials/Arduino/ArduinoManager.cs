/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;

namespace ToneTuneToolkit.Arduino
{
  public class ArduinoManager : SingletonMaster<ArduinoManager>
  {
    [SerializeField] private SerialController sc;

    // ==================================================

    private void Start() => Init();

    // ==================================================

    private void Init()
    {
      // 读取配置文件
    }

    // ==================================================

    public void SetPortName(string value) => sc.portName = value;
    public void SetBaudRate(int value) => sc.baudRate = value;

    public void ResetArduino() => StartCoroutine(nameof(ResetArduinoAction));
    private IEnumerator ResetArduinoAction()
    {
      sc.enabled = false;
      yield return new WaitForSeconds(1f);
      sc.enabled = true;
    }
  }
}