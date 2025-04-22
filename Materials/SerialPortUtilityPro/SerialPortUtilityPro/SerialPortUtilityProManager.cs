using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using SerialPortUtility;
using UnityEngine.Events;

public class SerialPortUtilityProManager : MonoBehaviour
{
  public static SerialPortUtilityProManager Instance;

  private SerialPortUtilityPro serialPortUtilityPro;

  public static UnityAction<string> OnReceiveMessage;

  // ==================================================

  private void Awake() => Instance = this;
  private void Start() => Init();

  // ==================================================

  private void Init()
  {
    serialPortUtilityPro = GetComponent<SerialPortUtilityPro>();
    return;
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
    return;
  }

  // ==================================================
  #region 串口开关

  public void SwitchSerialPortUtilityPro(bool value)
  {
    if (value)
    {
      serialPortUtilityPro.Open();
    }
    else
    {
      serialPortUtilityPro.Close();
    }
    return;
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
    // byte[] data = OutMessageProcessing(value);
    // serialPortUtilityPro.Write(data);

    serialPortUtilityPro.Write(Encoding.ASCII.GetBytes(value)); // 插件
    Debug.Log("[SPUP M] Send: " + value);
    return;
  }

  #endregion
  // ==============================
  #region 收包

  /// <summary>
  /// 读原流
  /// 配合SerialPortUtilityPro事件使用
  /// 需选择Read Data Structure
  /// </summary>
  /// <param name="streaming"></param>
  public void ReadStreaming(object streaming)
  {
    Debug.Log("[SPUP M] Read: " + streaming);
    string stringRawData = streaming.ToString();
    // stringRawData = InMessageProcessing(stringRawData);
    if (OnReceiveMessage != null)
    {
      OnReceiveMessage(stringRawData);
    }
    return;
  }

  /// <summary>
  /// 读二进制流
  /// 配合SerialPortUtilityPro事件使用
  /// 需选择Read Data Structure
  /// </summary>
  /// <param name="byteData"></param>
  public void ReadBinaryStreaming(object byteData)
  {
    Debug.Log(byteData);
    string stringRawData = BitConverter.ToString((byte[])byteData); // 比特流翻译
    stringRawData = InMessageProcessing(stringRawData);
    Debug.Log("[SPUP M] Read: " + stringRawData);
    if (OnReceiveMessage != null)
    {
      OnReceiveMessage(stringRawData);
    }
    return;
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