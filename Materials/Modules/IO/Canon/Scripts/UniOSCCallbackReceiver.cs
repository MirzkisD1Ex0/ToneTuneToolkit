/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UniOSC;
using UnityEngine;
using UnityEngine.Events;

namespace ToneTuneToolkit.IO.Canon
{
  public class UniOSCCallbackReceiver : UniOSCEventTarget
  {
    public static UniOSCCallbackReceiver Instance;

    public static UnityAction<string> OnTakePhotoFinished;

    private float unlockAt;
    private const float MESSAGE_LOCKTIME = 0.2f;

    // ==================================================

    private void Awake() => Instance = this;

    // ==================================================

    /// <summary>
    /// 消息接收重写
    /// </summary>
    /// <param name="args"></param>
    public override void OnOSCMessageReceived(UniOSCEventArgs args)
    {
      if (Time.unscaledTime < unlockAt) { return; }
      unlockAt = Time.unscaledTime + MESSAGE_LOCKTIME;
      AnalyseMessage(args);
    }

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
          Debug.Log(@$"[Canon CR] 设备初始化完成: {args.Packet.Data[1]}");
          break;
        case "/callback/takephoto":
          Debug.Log(@$"[Canon CR] 拍照完成,图片位于[{args.Packet.Data[1]}]");
          OnTakePhotoFinished?.Invoke(args.Packet.Data[1].ToString());
          break;
      }
    }
  }
}
