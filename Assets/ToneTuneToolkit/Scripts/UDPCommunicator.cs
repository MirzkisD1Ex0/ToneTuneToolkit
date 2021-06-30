using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ToneTuneToolkit
{
    /// <summary>
    /// UDP收发工具
    /// 测试前务必关闭所有防火墙
    /// 设备间需要ping通
    /// </summary>
    public class UDPCommunicator : MonoBehaviour
    {
        public static UDPCommunicator Instance; // 单例

        // IP端口设置
        private byte[] localIP = new byte[] { 10, 229, 24, 148 }; // 本地IP
        private int localPort = 4399;

        private byte[] targetIP = new byte[] { 10, 229, 24, 131 }; // 目标IP
        private int targetPort = 4398;

        // 设置
        private float detectSpacing = 1f; // 循环检测间隔

        // 其它
        public string ReceiveMessage; // 接受到的消息
        private UdpClient udpClient; // UDP客户端
        private Thread thread = null; // 单开线程
        private IPEndPoint remoteAddress;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Presetting();
            // MessageSend(targetIP, targetPort, "sdasdasfaghhh");
        }

        private void OnDestroy()
        {
            SocketQuit();
        }

        private void OnApplicationQuit()
        {
            SocketQuit();
        }

        /// <summary>
        /// 预设置
        /// </summary>
        private void Presetting()
        {
            remoteAddress = new IPEndPoint(IPAddress.Any, 0);
            thread = new Thread(MessageReceive); // 单开线程接收消息
            thread.Start();
            InvokeRepeating("RepeatAction", 0f, detectSpacing); // 每隔2秒检测一次是否有消息传入
        }

        /// <summary>
        /// 重复操作检测操作
        /// </summary>
        private void RepeatAction()
        {
            if (string.IsNullOrEmpty(ReceiveMessage)) // 如果消息为空
            {
                return;
            }
            Debug.Log(ReceiveMessage); // 输出接收结果
            ReceiveMessage = string.Empty; // 清空接收结果
            return;
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        private void MessageReceive()
        {
            while (true)
            {
                udpClient = new UdpClient(localPort);
                byte[] receiveData = udpClient.Receive(ref remoteAddress); // 接收数据
                ReceiveMessage = Encoding.UTF8.GetString(receiveData);
                udpClient.Close();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="sendMessage"></param>
        public void MessageSend(byte[] ip, int port, string sendMessage)
        {
            IPAddress tempIPAddress = new IPAddress(ip);
            IPEndPoint tempRemoteAddress = new IPEndPoint(tempIPAddress, port); // 实例化一个远程端点
            if (sendMessage != null)
            {
                byte[] sendData = Encoding.Unicode.GetBytes(sendMessage);
                UdpClient client = new UdpClient(); // this.localPort + 1 // 端口不可复用 // 否则无法区分每条消息
                client.Send(sendData, sendData.Length, tempRemoteAddress); // 将数据发送到远程端点
                client.Close(); // 关闭连接
            }
            return;
        }

        /// <summary>
        /// 退出套接字
        /// </summary>
        private void SocketQuit()
        {
            thread.Abort();
            thread.Interrupt();
            udpClient.Close();
            return;
        }

        /// <summary>
        /// 固定向发消息
        /// 偷懒方法
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage2Target(string message)
        {
            MessageSend(targetIP, targetPort, message);
            Debug.Log("Send <<color=#FF0000>" + message + "</color>> to <<color=#FF0000>" + targetIP[0] + "." + targetIP[1] + "." + targetIP[2] + "." + targetIP[3] + ":" + targetPort + "</color>>");
        }
    }
}