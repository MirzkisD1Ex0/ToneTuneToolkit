/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using SerialPortUtility;
using UnityEngine.Events;
using ToneTuneToolkit.Common;

/// <summary>
/// ReadProtocol使用Streaming
/// </summary>
public class SerialPortUtilityProManager : SingletonMaster<SerialPortUtilityProManager>
{
  private SerialPortUtilityPro serialPortUtilityPro;

  public static UnityAction<string> OnReceiveMessage;

  // ==================================================

  protected override void Awake() => Init();

  // ==================================================

  private void Init()
  {
    base.Awake();
    serialPortUtilityPro = GetComponent<SerialPortUtilityPro>();
  }

  public void Preset()
  {
    LoadPortInfo(0);
    SwitchSerialPortUtilityPro(true);
  }

  // ==================================================

  /// <summary>
  /// 加载端口信息
  /// </summary>
  /// <param name="index">设备序号</param>
  public void LoadPortInfo(int portInfoIndex)
  {
    serialPortUtilityPro.VendorID = SerialPortUtilityProConfiger.Instance.GetDeviceVendorID(portInfoIndex);
    serialPortUtilityPro.ProductID = SerialPortUtilityProConfiger.Instance.GetDeviceProductID(portInfoIndex);
    serialPortUtilityPro.SerialNumber = SerialPortUtilityProConfiger.Instance.GetDeviceSerialNumber(portInfoIndex);
  }

  // ==================================================
  #region 串口开关

  public void SwitchSerialPortUtilityPro(bool value)
  {
    if (value)
    { serialPortUtilityPro.Open(); }
    else
    { serialPortUtilityPro.Close(); }
  }

  #endregion
  // ==================================================
  #region 发包

  /// <summary>
  /// 发送信号给设备
  /// </summary>
  /// <param name="value"></param>
  /// <param name="modeIndex"></param>
  public void SendMessage2Device(string value)
  {
    serialPortUtilityPro.Write(Encoding.ASCII.GetBytes(value)); // 插件
    Debug.Log(@$"[SPUP M] Send: {value}");
  }

  public void SendByteMessage2Device(byte[] value)
  {
    serialPortUtilityPro.Write(value);
    Debug.Log(@$"[SPUP M] Send hex: {BitConverter.ToString(value)}");
  }

  #endregion
  // ==================================================
  #region 收包

  /// <summary>
  /// 读原流
  /// 配合SerialPortUtilityPro事件使用
  /// 需选择Read Data Structure
  /// </summary>
  /// <param name="streaming"></param>
  public void ReadStreaming(object streaming)
  {
    Debug.Log(@$"[SPUP M] Read: {streaming}");
    string stringRawData = streaming.ToString();
    // stringRawData = InMessageProcessing(stringRawData);
    OnReceiveMessage?.Invoke(stringRawData);
  }

  /// <summary>
  /// 读二进制流
  /// 配合SerialPortUtilityPro事件使用
  /// 需选择Read Data Structure
  /// </summary>
  /// <param name="byteData"></param>
  public void ReadBinaryStreaming(object byteData)
  {
    string stringRawData = BitConverter.ToString((byte[])byteData); // 比特流翻译
    stringRawData = InMessageProcessing(stringRawData);
    Debug.Log(@$"[SPUP M] Read: {stringRawData}");
    OnReceiveMessage?.Invoke(stringRawData);
  }

  /// <summary>
  /// 额外处理收到的消息
  /// </summary>
  /// <param name="value"></param>
  private string InMessageProcessing(string value)
  {
    value = value.Replace('-', ' ');
    return value;
  }

  #endregion
}
