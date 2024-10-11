using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniOSC;
using OSCsharp.Data;

namespace ToneTuneToolkit.OSC
{
  /// <summary>
  /// OSC管理器
  /// UniOSCManager.Instance.SendOSCMessage("/callback/starttutorial", 1);
  /// UniOSCManager.Instance.UpdateOutIPAddress("192.168.50.14");
  /// </summary>
  public class UniOSCManager : MonoBehaviour
  {
    public static UniOSCManager Instance;

    private UniOSCConnection uniOSCConnection;

    // ==================================================

    private void Awake()
    {
      Instance = this;
      uniOSCConnection = GetComponent<UniOSCConnection>();
    }

    private void OnApplicationQuit()
    {
      uniOSCConnection.DisconnectOSC();
      uniOSCConnection.DisconnectOSCOut();
    }

    // ==================================================

    /// <summary>
    /// 轻量版消息发射器
    /// </summary>
    /// <param name="address"></param>
    /// <param name="message"></param>
    public void SendOSCMessageLite(string ip, string port, string message)
    {
      UpdateOutIPAddress(ip, port);
      SendOSCMessage(message, 1);
      return;
    }

    /// <summary>
    /// 超轻量版消息发射器
    /// 需要确保地址正确
    /// </summary>
    /// <param name="message"></param>
    public void SendOSCMessageLite(string message)
    {
      SendOSCMessage(message, 1);
      return;
    }

    /// <summary>
    /// 更新本地地址
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void UpdateInIPAddress(string ip, string port)
    {
      if (uniOSCConnection.oscInIPAddress != ip || uniOSCConnection.oscPort != int.Parse(port))
      {
        uniOSCConnection.oscInIPAddress = ip;
        uniOSCConnection.oscPort = int.Parse(port);
        uniOSCConnection.ConnectOSC();
      }
      return;
    }

    /// <summary>
    /// 更新目标地址
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void UpdateOutIPAddress(string ip, string port)
    {
      if (uniOSCConnection.oscOutIPAddress != ip || uniOSCConnection.oscOutPort != int.Parse(port))
      {
        uniOSCConnection.oscOutIPAddress = ip;
        uniOSCConnection.oscOutPort = int.Parse(port);
        uniOSCConnection.ConnectOSCOut();
      }
      return;
    }

    /// <summary>
    /// 消息发射器
    /// </summary>
    /// <param name="address"></param>
    /// <param name="value"></param>
    private void SendOSCMessage(string address, object value = null)
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

      UniOSCEventArgs uniOSCEvent = new UniOSCEventArgs(uniOSCConnection.oscOutPort, oscMessage)
      {
        IPAddress = uniOSCConnection.oscOutIPAddress
      };
      uniOSCEvent.IPAddress = uniOSCConnection.oscOutIPAddress;
      uniOSCConnection.SendOSCMessage(null, uniOSCEvent);

      DEBUG_UIManager.Instance.UpdateOSCText(oscMessage.Address);
      Debug.Log($"[UniOSC Manager] Send <color=white>{oscMessage.Address}</color> to <color=white>{uniOSCConnection.oscOutIPAddress}:{uniOSCConnection.oscOutPort}</color>...[<color=green>OK</color>]");
      return;
    }
  }
}