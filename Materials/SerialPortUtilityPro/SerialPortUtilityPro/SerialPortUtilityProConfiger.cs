using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

/// <summary>
/// 通常来说设置产品的VID/PID就足以识别硬件了
/// 填入序列号将导致识别唯一
/// 仅读取配置
/// </summary>
public class SerialPortUtilityProConfiger : MonoBehaviour
{
  public static SerialPortUtilityProConfiger Instance;

  #region Path
  private string spupConfigPath = $"{Application.streamingAssetsPath}/configs/serialportutilityproconfig.json";
  #endregion

  #region Value
  [SerializeField] private List<DeviceInfoData> deviceInfoDatas;
  #endregion

  // ==================================================

  private void Awake() => Init();

  // ==================================================

  private void Init()
  {
    Instance = this;
    ReadConfig();

    // foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
    // {
    //   Debug.Log("可用串口: " + portName);
    // }
    return;
  }

  private void ReadConfig()
  {
    string ssupSettingJson = File.ReadAllText(spupConfigPath, Encoding.UTF8);
    Dictionary<string, List<string>> dic = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(ssupSettingJson);
    List<string> DeviceInfos = dic["device_info"];

    for (int i = 0; i < DeviceInfos.Count; i++)
    {
      DeviceInfoData tempDID = new DeviceInfoData();
      string[] infoSlice = DeviceInfos[i].Split('_');

      if (infoSlice.Length == 3)
      {
        tempDID.VendorID = infoSlice[0];
        tempDID.ProductID = infoSlice[1];
        tempDID.SerialNumber = infoSlice[2];
      }
      else
      {
        tempDID.VendorID = infoSlice[0];
        tempDID.ProductID = infoSlice[1];
        tempDID.SerialNumber = null;
      }

      deviceInfoDatas.Add(tempDID);
    }
    return;
  }

  // ==================================================

  public string GetDeviceVendorID(int index)
  {
    return deviceInfoDatas[index].VendorID;
  }

  public string GetDeviceProductID(int index)
  {
    return deviceInfoDatas[index].ProductID;
  }

  public string GetDeviceSerialNumber(int index)
  {
    return deviceInfoDatas[index].SerialNumber;
  }

  // ==================================================
  #region Data Class

  [Serializable]
  public class DeviceInfoData
  {
    public string VendorID;
    public string ProductID;
    public string SerialNumber;
  }
  #endregion
}