/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit.IO.Printer
{
  /// <summary>
  /// 打印机控制器
  /// </summary>
  public class PrinterHandler : MonoBehaviour
  {
    private void Start() => Init();

    private void Init()
    {
      L805();
      // DNP();
    }

    private void DNP()
    {
      PrinterManager.PrinterSetting setting = new PrinterManager.PrinterSetting();
      setting.PrinterName = "DP-DS620";
      setting.PaperSizeName = "(4x6)";
      setting.DPI = 100;
      setting.WidthInch = 6;
      setting.HeightInch = 4;
      setting.Landscape = false;
      setting.Margin = Vector4.zero;
      setting.rotateFlip = System.Drawing.RotateFlipType.Rotate270FlipNone;
      PrinterManager.Instance.SetPrinter(setting);
    }

    private void L805()
    {
      PrinterManager.PrinterSetting setting = new PrinterManager.PrinterSetting();
      setting.PrinterName = "EPSON L805 Series";
      setting.PaperSizeName = "(4x6)";
      setting.DPI = 100;
      setting.WidthInch = 4;
      setting.HeightInch = 6;
      setting.Landscape = false;
      setting.Margin = Vector4.zero;
      setting.rotateFlip = System.Drawing.RotateFlipType.RotateNoneFlipNone;
      PrinterManager.Instance.SetPrinter(setting);
    }
  }
}
