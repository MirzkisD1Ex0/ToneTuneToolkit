using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;
using OSCsharp.Data;
using UnityEngine.Video;

public class UniOSCManager : MonoBehaviour
{
  public static UniOSCManager Instance;
  private UniOSCConnection uniOSCConnection;

  // ==================================================

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    uniOSCConnection = GetComponent<UniOSCConnection>();
  }

  // ==================================================

  /// <summary>
  /// 消息发射器
  /// </summary>
  /// <param name="address"></param>
  /// <param name="value"></param>
  public void SendOSCMessage(string address, object value = null)
  {
    // OscMessage oscMessage = new OscMessage(address);
    OscMessage oscMessage = new OscMessage("/");
    oscMessage.Address = address;
    oscMessage.ClearData();
    if (value != null)
    {
      oscMessage.Append(value);
    }
    else
    {
      oscMessage.Append("");
    }
    Debug.Log(oscMessage.Address);

    UniOSCEventArgs uniOSCEvent = new UniOSCEventArgs(uniOSCConnection.oscOutPort, oscMessage)
    {
      IPAddress = uniOSCConnection.oscOutIPAddress
    };
    uniOSCEvent.IPAddress = uniOSCConnection.oscOutIPAddress;
    uniOSCConnection.SendOSCMessage(null, uniOSCEvent);
    return;
  }
}