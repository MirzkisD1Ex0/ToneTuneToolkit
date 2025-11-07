/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

namespace ToneTuneToolkit.IO.Printer
{
  /// <summary>
  /// 打印机配置器
  /// </summary>
  public class PrinterConfiger : MonoBehaviour
  {
    private void Start() => Init();

    // ==================================================

    private void Init()
    {
      // L805();
      // DNP();
      // DNP_Theory();
      L805_A4();
    }

    // ==================================================

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
      setting.rotateFlip = RotateFlipType.Rotate270FlipNone;
      PrinterManager.Instance.SetPrinter(setting);
    }

    private void DNP_Theory()
    {
      // 纸张规格:PR(4x6)
      // 边框:禁用
      PrinterManager.PrinterSetting setting = new PrinterManager.PrinterSetting();
      setting.PrinterName = "DP-DS620";
      setting.PaperSizeName = "PR(4x6)";
      setting.DPI = 100;
      setting.WidthInch = 4;
      setting.HeightInch = 6;
      setting.Landscape = false;
      setting.Margin = Vector4.zero;
      setting.rotateFlip = RotateFlipType.RotateNoneFlipNone;
      PrinterManager.Instance.SetPrinter(setting);
    }

    private void L805_6Inch()
    {
      PrinterManager.PrinterSetting setting = new PrinterManager.PrinterSetting();
      setting.PrinterName = "EPSON L805 Series";
      setting.PaperSizeName = "(4x6)";
      setting.DPI = 100;
      setting.WidthInch = 4;
      setting.HeightInch = 6;
      setting.Landscape = false;
      setting.Margin = Vector4.zero;
      setting.rotateFlip = RotateFlipType.RotateNoneFlipNone;
      PrinterManager.Instance.SetPrinter(setting);
    }

    private void L805_A4()
    {
      PrinterManager.PrinterSetting setting = new PrinterManager.PrinterSetting();
      setting.PrinterName = "EPSON L805 Series";
      setting.PaperSizeName = "A4";
      setting.DPI = 100;
      setting.WidthInch = 12.4f;
      setting.HeightInch = 17.54f;
      setting.Landscape = false;
      setting.Margin = Vector4.zero;
      setting.rotateFlip = RotateFlipType.RotateNoneFlipNone;
      PrinterManager.Instance.SetPrinter(setting);
    }
  }
}
