using System.Collections;
using System.Collections.Generic;
using UniOSC;
using UnityEngine;

public class CallbackReceiver : UniOSCEventTarget
{

  /// <summary>
  /// 消息接收重写
  /// </summary>
  /// <param name="args"></param>
  public override void OnOSCMessageReceived(UniOSCEventArgs args)
  {
    AnalyseMessage(args);
    return;
  }

  // ==================================================

  /// <summary>
  /// 消息分析
  /// </summary>
  /// <param name="args"></param>
  private void AnalyseMessage(UniOSCEventArgs args)
  {
    switch (args.Address)
    {
      default: break;
      case "/callback/setup":
        Debug.Log("设备初始化完成;" + args.Packet.Data[1]);
        break;
      case "/callback/takephoto":
        Debug.Log("拍照完成,图片位于[" + args.Packet.Data[1] + "]");
        break;
    }


    // Debug.Log(args.Address);
    // if (args.Packet.Data.Count > 0)
    // {
    //   foreach (var VARIABLE in args.Packet.Data)
    //   {
    //     Debug.Log(VARIABLE);
    //   }
    // }
    return;
  }

}