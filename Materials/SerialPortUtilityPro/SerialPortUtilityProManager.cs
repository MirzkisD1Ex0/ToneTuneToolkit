using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SerialPortUtility;
using UnityEngine.Events;

namespace FordBroncoToproofAssemble
{
  public class SerialPortUtilityProManager : MonoBehaviour
  {
    public static SerialPortUtilityProManager Instance;

    private SerialPortUtilityPro serialPortUtilityPro;
    private event UnityAction<object> OnReciveMessage;

    // ==============================

    private void Awake()
    {
      Instance = this;
      serialPortUtilityPro = FindAnyObjectByType<SerialPortUtilityPro>();
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Q)) // 秒表正计时
      {
        SendMessage2Device(TimerCommandStorage.Start);
      }
      if (Input.GetKeyDown(KeyCode.W)) // 暂停
      {
        SendMessage2Device(TimerCommandStorage.Pause);
      }
      if (Input.GetKeyDown(KeyCode.E)) // 重置
      {
        SendMessage2Device(TimerCommandStorage.Reset);
      }
      if (Input.GetKeyDown(KeyCode.A)) // 返回值
      {
        SendMessage2Device(TimerCommandStorage.GetTime);
      }
    }

    // ==============================

    public void AddEventListener(UnityAction<object> unityAction)
    {
      OnReciveMessage += unityAction;
      return;
    }

    public void RemoveEventListener(UnityAction<object> unityAction)
    {
      OnReciveMessage -= unityAction;
      return;
    }

    // ==============================
    // 发包

    /// <summary>
    /// 发送信号给设备
    /// </summary>
    /// <param name="value">是否带0x都可以</param>
    public void SendMessage2Device(string value)
    {
      byte[] data = OutMessageProcessing(value);
      serialPortUtilityPro.Write(data);
      return;
    }

    /// <summary>
    /// 发出数据包处理
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private byte[] OutMessageProcessing(string value)
    {
      string[] valueSlices = value.Replace("0x", "").Split(' '); // 去0x // 分割
      byte[] bytes = new byte[valueSlices.Length];
      for (int i = 0; i < bytes.Length; i++)
      {
        bytes[i] = Convert.ToByte(Convert.ToInt32(valueSlices[i], 16));
      }
      return bytes;
    }

    // ==============================
    // 收包

    /// <summary>
    /// 读二进制流
    /// 配合SerialPortUtilityPro使用
    /// </summary>
    /// <param name="byteData"></param>
    public void ReadBinaryStreaming(object byteData)
    {
      string stringRawData = BitConverter.ToString((byte[])byteData); // 比特流翻译
      InMessageProcessing(stringRawData);
      return;
    }

    private void InMessageProcessing(string value)
    {
      string[] dataSlices = value.Split('-'); // 数据切片

      // 在此处理/过滤数据
      if (dataSlices.Length < 14) // 以长度判断
      {
        return;
      }

      string result = string.Empty;
      for (int i = 6; i < 12; i++)
      {
        result += Convert.ToChar(Convert.ToInt32(dataSlices[i], 16)) - '0'; // Ascii16转10转char转int
      }

      // 广播订阅
      if (OnReciveMessage != null)
      {
        OnReciveMessage(result);
        return;
      }
      return;
    }
  }
}