using UnityEngine;
using System.Net.Sockets;
using System.Text;
using ToneTuneToolkit.Common;

/// <summary>
/// TCP发图片和文本
/// </summary>
public class TCPClient : SingletonMaster<TCPClient>
{
  public string serverIP = "192.168.1.100"; // Windows服务器的IP地址
  public int serverPort = 1006;

  private TcpClient client;
  private NetworkStream stream;

  // ==================================================

  private void Start() => Init();
  private void OnDestroy() => UnInit();

  // ==================================================

  private void Init()
  {
    // Connect();
  }

  private void UnInit()
  {
    Disconnect();
  }

  // ==================================================

  public void SetServerIP(string value) => serverIP = value;
  public void SetServerPort(string value) => serverPort = int.Parse(value);

  // ==================================================

  // 连接到服务器
  public void Connect()
  {
    try
    {
      client = new TcpClient();
      client.Connect(serverIP, serverPort);
      stream = client.GetStream();
      Debug.Log("[TCP]Connected to server");
    }
    catch (System.Exception e)
    {
      Debug.LogError("[TCP]Connection error: " + e.Message);
    }
  }

  // 断开连接
  public void Disconnect()
  {
    if (stream != null) { stream.Close(); }
    if (client != null) { client.Close(); }
    Debug.Log("[TCP]Disconnected from server");
  }



  // 发送文本消息
  public void SendText(string message)
  {
    if (client == null || !client.Connected) { return; }

    try
    {
      byte[] textBytes = Encoding.UTF8.GetBytes(message);
      byte[] lengthBytes = System.BitConverter.GetBytes(textBytes.Length);
      byte[] dataType = new byte[] { 0 }; // 0 = text

      stream.Write(lengthBytes, 0, 4);
      stream.Write(dataType, 0, 1);
      stream.Write(textBytes, 0, textBytes.Length);
      Debug.Log("[TCP]Sent text: " + message);
    }
    catch (System.Exception e)
    {
      Debug.LogError("[TCP]Send text error: " + e.Message);
    }
  }

  // 发送图片
  public void SendImage(Texture2D texture)
  {
    if (client == null || !client.Connected) { Debug.LogWarning("Not connected to server"); return; }

    try
    {
      byte[] imageBytes = texture.EncodeToPNG();
      byte[] lengthBytes = System.BitConverter.GetBytes(imageBytes.Length);
      byte[] dataType = new byte[] { 1 }; // 1 = image

      stream.Write(lengthBytes, 0, 4);
      stream.Write(dataType, 0, 1);
      stream.Write(imageBytes, 0, imageBytes.Length);
      Debug.Log("[TCP]Sent image with size: " + texture.width + "x" + texture.height);
    }
    catch (System.Exception e)
    {
      Debug.LogError("[TCP]Send image error: " + e.Message);
      Disconnect();
    }
  }
}