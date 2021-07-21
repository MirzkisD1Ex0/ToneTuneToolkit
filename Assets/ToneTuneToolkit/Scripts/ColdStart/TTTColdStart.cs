using UnityEngine;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ToneTuneToolkit
{
    /// <summary>
    /// OK
    /// 设备冷启动
    /// </summary>
    public class TTTColdStart : MonoBehaviour
    {
        public static TTTColdStart Instance;

        private static string targetMAC;
        private static string targetIP;
        private static string targetMask;
        private static string targetPort;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            targetMAC = TTTTextLoader.GetJson(TTTColdStartHandler.WOLConfigPath, TTTColdStartHandler.TargetMACName);
            targetIP = TTTTextLoader.GetJson(TTTColdStartHandler.WOLConfigPath, TTTColdStartHandler.TargetIPName);
            targetMask = TTTTextLoader.GetJson(TTTColdStartHandler.WOLConfigPath, TTTColdStartHandler.TargetMaskName);
            targetPort = TTTTextLoader.GetJson(TTTColdStartHandler.WOLConfigPath, TTTColdStartHandler.TargetPortName);
            return;
        }

        /// <summary>
        /// 冷启动
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <param name="port"></param>
        private void ColdStartDevice(string mac, string ip, string mask, string port = "7")
        {
            string command = (TTTColdStartHandler.WOLAppPath + "wolcmd " + mac + " " + ip + " " + mask + " " + port).Replace(@"/", @"\");

            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;

            p.StartInfo = psi;
            p.Start();

            p.StandardInput.WriteLine(command);
            p.StandardInput.AutoFlush = true;
            p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            return;
        }

        /// <summary>
        /// 偷懒方法
        /// </summary>
        public void WakeOnLan()
        {
            ColdStartDevice(targetMAC, targetIP, targetMask, targetPort);
            return;
        }
    }
}