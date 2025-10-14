/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.IO.Printer
{
  /// <summary>
  /// 
  /// 测试可打印1200x1800的图片，页面参数{X=0,Y=0,Width=400,Height=600}
  /// </summary>
  public class PrinterManager : SingletonMaster<PrinterManager>
  {
    private PrinterSetting ps = new PrinterSetting();

    // ==================================================

    private void Start() => Init();

    // ==================================================

    private void Init()
    {
      // DisplayInstalledPrinters();
      OperationalCheck();
    }

    // ==================================================
    #region 设置打印机

    public void SetPrinter(PrinterSetting setting)
    {
      ps = setting;

      foreach (string item in PrinterSettings.InstalledPrinters)
      {
        if (item.Contains(ps.PrinterName))
        {
          ps.PrinterName = item;
          return;
        }
      }
      ps.PrinterName = new PrinterSettings().PrinterName;
    }

    #endregion
    // ==================================================
    #region Tool 显示所有打印机

    private void DisplayInstalledPrinters()
    {
      foreach (string printer in PrinterSettings.InstalledPrinters) { Debug.Log($"[PM] 已安装打印机: {printer}"); }
    }

    #endregion
    // ==================================================

    /// <summary>
    /// 可运行性检查
    /// </summary>
    private void OperationalCheck()
    {
      if (!Application.isEditor && Application.platform != RuntimePlatform.WindowsPlayer)
      {
        Debug.LogWarning("[PM] 打印功能仅支持Windows平台");
      }
    }

    // ==================================================

    /// <summary>
    /// 打印照片
    /// </summary>
    /// <param name="texture"></param>
    public void Print(Texture2D texture)
    {
      try
      {
        // 打印前强制GC
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Debug.Log($"[PM] 原始Texture2D尺寸: {texture.width}x{texture.height}");

        using (Bitmap bitmap = T2D2BitmapSafe(texture))
        {
          Debug.Log($"[PM] 转换后Bitmap尺寸: {bitmap.Width}x{bitmap.Height}");

          PrintBitmap(bitmap);
        }
      }
      catch (Exception e)
      {
        Debug.LogError("[PM] 打印失败: " + e.Message);
      }
    }

    // ==================================================
    #region 转换Texture2D到Bitmap

    /// <summary>
    /// 安全转换Texture2D到Bitmap
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    private Bitmap T2D2BitmapSafe(Texture2D texture)
    {
      Bitmap bitmap = new Bitmap(texture.width, texture.height, PixelFormat.Format32bppArgb);
      bitmap.SetResolution(ps.DPI, ps.DPI);
      BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

      try
      {
        Color32[] pixels = texture.GetPixels32();
        byte[] bytes = new byte[pixels.Length * 4];

        for (int y = 0; y < texture.height; y++)
        {
          for (int x = 0; x < texture.width; x++)
          {
            int sourceIndex = y * texture.width + x;
            int destY = texture.height - 1 - y;
            int destIndex = destY * texture.width + x;

            bytes[destIndex * 4] = pixels[sourceIndex].b;
            bytes[destIndex * 4 + 1] = pixels[sourceIndex].g;
            bytes[destIndex * 4 + 2] = pixels[sourceIndex].r;
            bytes[destIndex * 4 + 3] = pixels[sourceIndex].a;
          }
        }

        Marshal.Copy(bytes, 0, bitmapData.Scan0, bytes.Length);
      }
      finally
      {
        bitmap.UnlockBits(bitmapData);
      }
      return bitmap;
    }

    #endregion
    // ==================================================
    #region 打印Bitmap

    private void PrintBitmap(Bitmap bitmap)
    {
      using (PrintDocument pd = new PrintDocument())
      {
        pd.PrinterSettings.PrinterName = ps.PrinterName;
        pd.DefaultPageSettings.PrinterResolution = new PrinterResolution()
        {
          Kind = PrinterResolutionKind.Custom,
          X = ps.DPI,
          Y = ps.DPI
        };

        int paperWidth = ps.WidthInch * 100;
        int paperHeight = ps.HeightInch * 100;

        PaperSize photoSize = new PaperSize("Custom", paperWidth, paperHeight);
        pd.DefaultPageSettings.PaperSize = photoSize;
        pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
        pd.OriginAtMargins = false;

        pd.PrintPage += (sender, args) =>
        {
          try
          {
            Debug.Log($"[PM] 开始绘制: 图片{bitmap.Width}x{bitmap.Height} / 页面{args.PageBounds}");

            Rectangle destRect = new Rectangle(
                  (int)-args.PageSettings.HardMarginX,
                  (int)-args.PageSettings.HardMarginY,
                  (int)(args.PageBounds.Width + args.PageSettings.HardMarginX * 2),
                  (int)(args.PageBounds.Height + args.PageSettings.HardMarginY * 2)
              );

            args.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            args.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            args.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            bitmap.RotateFlip(ps.rotateFlip); // 旋转矫正
            args.Graphics.DrawImage(bitmap, destRect);

            Debug.Log("[PM] 绘制完成");
          }
          catch (Exception e)
          {
            Debug.LogError("[PM] 绘制图像时出错: " + e.Message);
            args.Cancel = true;
          }
        };

        try
        {
          pd.Print();
          Debug.Log("[PM] 打印任务已发送");
        }
        catch (Exception e)
        {
          Debug.LogError("[PM] 打印过程中出错: " + e.Message);
        }
      }
    }

    #endregion
    // ==================================================
    #region Config

    [Serializable]
    public class PrinterSetting
    {
      public string PrinterName = "L805";
      public string PaperSizeName = "6x4 Photo";
      public int WidthInch = 4;
      public int HeightInch = 6;
      public int DPI = 100; // 1200x1800是6英寸在300dpi下的像素尺寸 // 400x600是6英寸在100dpi下的像素尺寸

      public bool Landscape = false;
      public Vector4 Margin = Vector4.zero; // 边距 // x,y,z,w
      public RotateFlipType rotateFlip = RotateFlipType.RotateNoneFlipNone;
    }

    #endregion
  }
}
