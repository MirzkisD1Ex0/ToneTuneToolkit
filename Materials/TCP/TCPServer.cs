using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ToneTuneToolkit.Common;

public class TCPServer : SingletonMaster<TCPServer>
{
  private Thread serverThread;
  private TcpListener tcpListener;
  private bool isRunning = false;
  public int port = 1006;

  public delegate void TextReceivedHandler(string text);
  public delegate void ImageReceivedHandler(byte[] imageData);
  public event TextReceivedHandler OnTextReceived;
  public event ImageReceivedHandler OnImageReceived;

  // ==================================================

  private void Start() => Init();
  private void OnDestroy() => UnInit();

  // ==================================================

  private void Init()
  {
    StartServer();
  }

  private void UnInit()
  {
    StopServer();
  }

  // ==================================================

  public void StartServer()
  {
    isRunning = true;
    serverThread = new Thread(ServerThread);
    serverThread.IsBackground = true;
    serverThread.Start();
    Debug.Log($"[TCP]Server started on port {port}");
  }

  public void StopServer()
  {
    isRunning = false;
    tcpListener?.Stop();
    serverThread?.Abort();
    Debug.Log("[TCP]Server stopped");
  }



  private void ServerThread()
  {
    try
    {
      tcpListener = new TcpListener(IPAddress.Any, port);
      tcpListener.Start();

      while (isRunning)
      {
        using (TcpClient client = tcpListener.AcceptTcpClient())
        using (NetworkStream stream = client.GetStream())
        {
          // 读取数据长度(4字节)
          byte[] lengthBytes = new byte[4];
          stream.Read(lengthBytes, 0, 4);
          int dataLength = System.BitConverter.ToInt32(lengthBytes, 0);

          // 读取数据类型(1字节)
          byte[] typeBytes = new byte[1];
          stream.Read(typeBytes, 0, 1);
          byte dataType = typeBytes[0];

          // 读取实际数据
          byte[] data = new byte[dataLength];
          int bytesRead = 0;
          while (bytesRead < dataLength)
          {
            bytesRead += stream.Read(data, bytesRead, dataLength - bytesRead);
          }

          // 处理接收到的数据
          if (dataType == 0) // 文本
          {
            string text = Encoding.UTF8.GetString(data);
            Debug.Log($"[TCP]Received text: {text}");
            UnityMainThreadDispatcher.Instance().Enqueue(() => OnTextReceived?.Invoke(text));
          }
          else if (dataType == 1) // 图片
          {
            // 只传递原始字节数据到主线程
            byte[] imageData = (byte[])data.Clone();
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
              // 在主线程中创建Texture2D
              Texture2D texture = new Texture2D(2, 2);
              texture.LoadImage(imageData);
              OnImageReceived?.Invoke(imageData);
            });
            Debug.Log($"[TCP]Received image");
          }
        }
      }
    }
    catch (SocketException e)
    {
      if (isRunning)
        Debug.LogError($"[TCP]Server error: {e}");
    }
  }
}