/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using ToneTuneToolkit.Common;

using SeaorySDK;
using static SeaorySDK.PrinterApi;

namespace ToneTuneToolkit.IO.Seaory
{
  public class SeaoryManager : SingletonMaster<SeaoryManager>
  {
    private const int STEPSPACE_MSEC = 500;
    private const string PRINTERNAME = "Seaory S26";
    private SEAORY_DOC_PROP sdp; // 打印配置
    private uint code = 0;

    // ==================================================

    private void Start() => Init();

    // ==================================================

    private void Init()
    {
      InitSDP();
    }

    // ==================================================

    public void InitSDP()
    {
      sdp = new SEAORY_DOC_PROP();
      sdp.byOrientation = 2; // 横向输出
      sdp.byRibbonType = 0; // 色带类型 // 0 = YMCKO
      sdp.byRotate180 = 0x00; // 指定旋转面 // 0x00 = 都不旋转
      sdp.byInputBin = 0; // 进卡位置 // 0 = 自动进卡器
      sdp.byOutputBin = 0; // 出卡位置 // 0 = 出卡盒
      sdp.byEjectCardMode = 0; // 直接弹出卡
      sdp.byAutoDetectRibbon = 1; // 自动检测色带类型
      sdp.byPaperSize = 0; // 打印卡尺寸 // 0 = CR-80

      sdp.byPrintSide = 0x01; // 正面打印
      sdp.byPrintPanelFront = 0; // 正面色带类型 // 0 = YMCKO
    }

    // ==================================================
    #region 封装指令

    /// <summary>
    /// 打印图片
    /// </summary>
    /// <param name="imagePath"></param>
    /// <returns></returns>
    public async void PrintImage(string imagePath)
    {
      IntPtr ip = IntPtr.Zero;
      code = SOY_PR_StartPrinting2W(PRINTERNAME, ref sdp, ref ip);
      Debug.Log(@$"[SM] (1/5){code}");
      if (code != 0) { return; }

      await Task.Delay(STEPSPACE_MSEC);
      code = SOY_PR_StartPage2(ip);
      Debug.Log(@$"[SM] (2/5){code}");
      if (code != 0) { return; }

      // 打印图片
      await Task.Delay(STEPSPACE_MSEC);
      int imgX = 0;
      int imgY = 0;
      int imgW = 0;
      int imgH = 0;
      code = SOY_PR_PrintImage2W(ip, imgX, imgY, imgW, imgH, imagePath);
      Debug.Log(@$"[SM] (3/5){code}");
      if (code != 0) { Debug.Log(@$"[SM] 图片尺寸不能超过1012x648"); return; }

      await Task.Delay(STEPSPACE_MSEC);
      code = SOY_PR_EndPage2(ip);
      Debug.Log(@$"[SM] (4/5){code}");
      if (code != 0) { return; }

      await Task.Delay(STEPSPACE_MSEC);
      code = SOY_PR_EndPrinting2(ip, true);
      Debug.Log(@$"[SM] (5/5){code}");
    }

    #endregion
    // ==================================================
    #region 指令集

    /// <summary>
    /// 将卡片移动到指定位置
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint MoveCard2(CardPosition value)
    {
      uint code = SOY_PR_ExecCommandW(PRINTERNAME, (uint)value);
      return code;
    }
    [Serializable]
    public enum CardPosition
    {
      HOPPER = 1, // 出卡盒(后)
      STANDBY_BACK = 4, // 到后端待取
      REJECT_BOX_FRONT = 5, // 前侧废卡槽
      STANDBY_FRONT = 7, // 前端待取
      // FRONT = 9, // 前端位置
      PREPARE = 10, // 准备位置
    }

    /// <summary>
    /// 重置打印机
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static uint ResetPrinter(ResetLevel value)
    {
      uint code = SOY_PR_ExecCommandW(PRINTERNAME, (uint)value);
      return code;
    }
    [Serializable]
    public enum ResetLevel
    {
      HARD = 21, // 硬复位 // 吐卡
      JAM = 22, // 软复位 // 不吐卡
    }

    // ==================================================

    /// <summary>
    /// 获取SDK版本号
    /// </summary>
    /// <returns></returns>
    public static string GetSDKVersion()
    {
      char[] value = new char[256];
      SOY_PR_SdkVersionW(value);
      return new string(value);
    }

    /// <summary>
    /// 获取打印机状态
    /// </summary>
    /// <returns></returns>
    public static uint GetPrinterStatus()
    {
      uint code = 0;
      SOY_PR_GetPrinterStatusW(PRINTERNAME, ref code);
      return code;
    }

    /// <summary>
    /// 获取色带余量
    /// </summary>
    /// <returns></returns>
    public static string GetRibbonCount()
    {
      char[] code = new char[256];
      SOY_PR_GetPrinterInfoW(PRINTERNAME, 6, code);
      return new string(code);
    }

    //   private static WaitPrinterBusy(){
    // SOY_PR_WaitPrinterBusyW
    //     SOY_PR_WaitPrinterBusyW();
    //   }

    #endregion
  }
}
