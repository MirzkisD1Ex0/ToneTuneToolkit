/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;

namespace ToneTuneToolkit.SerialPort
{
  public class SerialPortUtilityProResponder : SingletonMaster<SerialPortUtilityProResponder>
  {
    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    private void Init()
    {
      SerialPortUtilityProManager.OnReceiveMessage += MessageProcessor;
    }

    private void UnInit()
    {
      SerialPortUtilityProManager.OnReceiveMessage -= MessageProcessor;
    }

    // ==================================================

    /// <summary>
    /// 消息翻译器
    /// </summary>
    /// <param name="value"></param>
    private void MessageProcessor(string value)
    {
      Debug.Log(value);
    }
  }
}
