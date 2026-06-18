/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;
using OSCsharp.Data;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.IO.Canon
{
  public class UniOSCManager : SingletonMaster<UniOSCManager>
  {
    private static UniOSCConnection uniOSCConnection;

    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => Uninit();

    // ==================================================

    private void Init()
    {
      uniOSCConnection = GetComponent<UniOSCConnection>();
      StartCoroutine(KeepCanonAlive());
    }

    private void Uninit()
    {
      SendOSCMessage(Configer.LiveViewOff, 0);
    }

    // ==================================================

    private IEnumerator KeepCanonAlive()
    {
      yield return new WaitForSeconds(3f);
      SendOSCMessage(Configer.LiveViewOn, 0);

      while (true)
      {
        SendOSCMessage(Configer.LiveViewOn, 0);
        yield return new WaitForSeconds(60f);
      }
    }

    // ==================================================

    /// <summary>
    /// 消息发送
    /// </summary>
    /// <param name="address"></param>
    /// <param name="value"></param>
    public static void SendOSCMessage(string address, object value)
    {
      OscMessage oscMessage = new OscMessage("/");
      oscMessage.Address = address;
      oscMessage.ClearData();
      oscMessage.Append(value);

      UniOSCEventArgs uniOSCEvent = new UniOSCEventArgs(uniOSCConnection.oscOutPort, oscMessage)
      {
        IPAddress = uniOSCConnection.oscOutIPAddress
      };

      uniOSCConnection.SendOSCMessage(null, uniOSCEvent);
    }
  }
}
