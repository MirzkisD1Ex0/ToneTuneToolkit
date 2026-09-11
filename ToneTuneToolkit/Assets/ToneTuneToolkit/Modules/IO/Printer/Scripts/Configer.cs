/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.1
/// </summary>

using UnityEngine;
using System.Drawing;

namespace ToneTuneToolkit.IO.Printer
{
  /// <summary>
  /// 打印机配置器
  /// Framework 4.0
  /// </summary>
  public class Configer : MonoBehaviour
  {

    // ==================================================

    private void Start() => Init();

    // ==================================================

    private void Init()
    {
      // L805_6Inch();
      DNP_Portrait();
    }

    // ==================================================

    private void DNP()
    {
      PrinterManager.PrinterSetting setting = new PrinterManager.PrinterSetting();
      setting.PrinterName = "DP-DS620";
      setting.PaperSizeName = "(4x6)";
      setting.DPI = 100;
      setting.WidthInch = 4;
      setting.HeightInch = 6;
      setting.Landscape = false;
      setting.Margin = Vector4.zero; // 英寸 // 左右边距 // 上下边距
      setting.rotateFlip = RotateFlipType.RotateNoneFlipNone;
      PrinterManager.Instance.SetPrinter(setting);
    }

    // 竖版合成 // 竖版打印 // 首选项纵向 // PR(4x6)
    private void DNP_Portrait()
    {
      PrinterManager.PrinterSetting setting = new PrinterManager.PrinterSetting
      {
        PrinterName = "DP-DS620",
        PaperSizeName = "PR(4x6)",
        DPI = 100,
        WidthInch = 4,
        HeightInch = 6,
        Landscape = false,
        Margin = Vector4.zero,
        rotateFlip = RotateFlipType.RotateNoneFlipNone
      };
      PrinterManager.Instance.SetPrinter(setting);
    }

    // LYNKCO
    // 纵向合成 // 纵向打印 // 打印机首选项选择横向(6x4)
    private void DNP_LYNKCO()
    {
      PrinterManager.PrinterSetting setting = new PrinterManager.PrinterSetting();
      setting.PrinterName = "DP-DS620";
      setting.PaperSizeName = "PR(4x6)";
      setting.DPI = 100;
      setting.WidthInch = 4;
      setting.HeightInch = 6;
      setting.Landscape = false;
      setting.Margin = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);
      setting.rotateFlip = RotateFlipType.Rotate180FlipNone;
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
