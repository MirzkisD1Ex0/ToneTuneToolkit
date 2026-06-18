/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using ToneTuneToolkit.Common;

/// <summary>
/// 通常来说设置产品的VID/PID就足以识别硬件了
/// 填入序列号将导致识别唯一
/// 仅读取配置
/// </summary>
public class SerialPortUtilityProConfiger : SingletonMaster<SerialPortUtilityProConfiger>
{
  private string spupConfigPath = @$"{Application.streamingAssetsPath}/Configs/serialportutilityproconfig.json";

  [SerializeField] private List<DeviceInfo> deviceInfos = new List<DeviceInfo>();

  // ==================================================

  protected override void Awake() => Init();

  // ==================================================

  private void Init()
  {
    base.Awake();
    LoadConfig();
    // foreach (var device in deviceInfos) { Debug.Log(@$"[SPUP C] VID: {device.VendorID}, PID: {device.ProductID}, SN: {device.SerialNumber}"); }
  }

  // ==================================================

  private void LoadConfig()
  {
    string ssupSettingJson = File.ReadAllText(spupConfigPath, Encoding.UTF8);
    RawDeviceInfo rawDeviceInfo = JsonUtility.FromJson<RawDeviceInfo>(ssupSettingJson);

    if (rawDeviceInfo == null || rawDeviceInfo.device_info == null) { return; }

    foreach (string rawItem in rawDeviceInfo.device_info)
    {
      string[] parts = rawItem.Split('_');

      if (parts.Length >= 3)
      {
        DeviceInfo data = new DeviceInfo
        {
          VendorID = parts[0],
          ProductID = parts[1],
          SerialNumber = parts[2]
        };
        deviceInfos.Add(data);
      }
    }
  }

  // ==================================================

  public string GetDeviceVendorID(int index) => deviceInfos[index].VendorID;
  public string GetDeviceProductID(int index) => deviceInfos[index].ProductID;
  public string GetDeviceSerialNumber(int index) => deviceInfos[index].SerialNumber;

  // ==================================================

  [Serializable]
  public class RawDeviceInfo
  {
    public List<string> device_info;
  }

  [Serializable]
  public class DeviceInfo
  {
    public string VendorID;
    public string ProductID;
    public string SerialNumber;
  }
}
