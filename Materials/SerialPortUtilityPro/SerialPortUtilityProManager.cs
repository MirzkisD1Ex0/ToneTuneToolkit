using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using SerialPortUtility;

public class SerialPortUtilityProManager : MonoBehaviour
{
  public static SerialPortUtilityProManager Instance;

  private SerialPortUtilityPro serialPortUtilityPro;

  // ==============================

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    Init();
  }

  // ==============================

  private void Init()
  {
    serialPortUtilityPro = GetComponent<SerialPortUtilityPro>();
    return;
  }

  // ==============================

  /// <summary>
  /// 
  /// </summary>
  /// <param name="index">设备序号</param>
  public void LoadPortInfo(int portInfoIndex)
  {
    serialPortUtilityPro.VendorID = SerialPortUtilityProConfiger.Instance.GetDeviceVendorID(portInfoIndex);
    serialPortUtilityPro.ProductID = SerialPortUtilityProConfiger.Instance.GetDeviceProductID(portInfoIndex);
    serialPortUtilityPro.SerialNumber = SerialPortUtilityProConfiger.Instance.GetDeviceSerialNumber(portInfoIndex);
    return;
  }

  /// <summary>
  /// 串口开关
  /// </summary>
  /// <param name="value"></param>
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

  // ==============================
  // 发包

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
    Debug.Log("SerialPort Send: " + value);
    return;
  }

  // ==============================
  // 收包

  /// <summary>
  /// 读原流
  /// 配合SerialPortUtilityPro使用
  /// </summary>
  /// <param name="streaming"></param>
  public void ReadStreaming(object streaming)
  {
    Debug.Log("Arduino Recive: " + streaming);
    string stringRawData = streaming.ToString();
    InMessageProcessing(stringRawData);
    return;
  }

  /// <summary>
  /// 读二进制流
  /// 配合SerialPortUtilityPro使用
  /// </summary>
  /// <param name="byteData"></param>
  public void ReadBinaryStreaming(object byteData)
  {
    Debug.Log(byteData);
    string stringRawData = BitConverter.ToString((byte[])byteData); // 比特流翻译
    Debug.Log("Arduino Recive: " + stringRawData.Replace('-', ' '));
    InMessageProcessing(stringRawData);
    return;
  }

  private void InMessageProcessing(string value)
  {
    int resultValue;
    bool canTrans = int.TryParse(value, out resultValue);

    if (!canTrans) // 转换失败
    {
      return;
    }
    return;
  }
}