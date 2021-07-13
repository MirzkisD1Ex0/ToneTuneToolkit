using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace ToneTuneToolkit
{
    /// <summary>
    /// OK
    /// Verifikado
    /// 验证系统
    /// 需要正确的配置文件
    /// TS ms
    /// http://www.txttool.com/
    /// https://tool.lu/timestamp
    /// </summary>
    public class TTTVerifier : MonoBehaviour
    {
        private string stampURL = "http://api.m.taobao.com/rest/api3.do?api=mtop.common.getTimestamp";
        private string verifikadoCode;
        private string verifikadoMAC;
        private string verifikadoStamp;

        private bool checker = false;

        // 弹窗
        [DllImport("user32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr handle, String message, String title, int type);

        // 网络
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        #region DEBUG
        private GameObject dtGO;
        private TextMesh dtTMCmpt;
        #endregion

        private void Start()
        {
            PreloadDebugInfo();
            VerifikadoSystem();
        }

        /// <summary>
        /// 预创建Debug文字
        /// </summary>
        private void PreloadDebugInfo()
        {
            dtGO = new GameObject("DebugText");
            dtGO.transform.position = Vector3.zero;
            dtGO.AddComponent<TextMesh>();

            dtGO.GetComponent<MeshRenderer>().enabled = true; // 关闭检测文字

            dtTMCmpt = dtGO.GetComponent<TextMesh>();
            dtTMCmpt.characterSize = .25f;
            dtTMCmpt.fontSize = 24;
            dtTMCmpt.anchor = TextAnchor.MiddleCenter;
            dtTMCmpt.alignment = TextAlignment.Left;
            dtTMCmpt.text = "> Verifying...";
            return;
        }

        /// <summary>
        /// Verifikado系统
        /// </summary>
        private void VerifikadoSystem()
        {
            checker = CheckFileExist(TTTVerifierHandler.AuthorizationFilePath); // s1 file
            dtTMCmpt.text += "\n> Check the Files Exists: <color=#FF0000>" + checker + "</color>"; // DEBUG
            if (!checker) // 如果为否
            {
                ApplicationError("无效的程序验证流程。");
                return;
            }

            checker = CheckNetwork(); // s2 net
            dtTMCmpt.text += "\n> Check the Network: <color=#FF0000>" + checker + "</color>"; // DEBUG
            if (!checker)
            {
                ApplicationError("无网络链接，请检查后重试。");
                return;
            }

            verifikadoCode = BinaryToString(TTTTextLoader.GetJson(TTTVerifierHandler.AuthorizationFilePath, TTTVerifierHandler.UCName));
            checker = CheckUniqueCode(verifikadoCode); // s3 uc
            dtTMCmpt.text += "\n> Check the Code: <color=#FF0000>" + checker + "</color>"; // DEBUG
            if (!checker)
            {
                ApplicationError("无效的授权。");
                return;
            }

            verifikadoMAC = BinaryToString(TTTTextLoader.GetJson(TTTVerifierHandler.AuthorizationFilePath, TTTVerifierHandler.MCName));
            checker = CheckMACCode(verifikadoMAC); // s4 mc
            dtTMCmpt.text += "\n> Check the Address: <color=#FF0000>" + checker + "</color>"; // DEBUG
            if (!checker)
            {
                ApplicationError("无效的地址。");
                return;
            }

            verifikadoStamp = BinaryToString(TTTTextLoader.GetJson(TTTVerifierHandler.AuthorizationFilePath, TTTVerifierHandler.TSName));
            StartCoroutine(CheckTimeStampChain(stampURL)); // s5 ts
            return;
        }

        #region Check
        private bool CheckFileExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                return true;
            }
            return false;
        }

        private bool CheckNetwork()
        {
            int i = 0;
            if (InternetGetConnectedState(out i, 0)) // C#网络判断
            {
                return true;
            }
            return false;
        }

        private bool CheckUniqueCode(string vC)
        {
            if (vC == SystemInfo.deviceUniqueIdentifier)
            {
                return true;
            }
            return false;
        }

        private bool CheckMACCode(string vM)
        {
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces(); // Get全部网卡
            for (int i = 0; i < nis.Length; i++)
            {
                if (nis[i].NetworkInterfaceType.ToString() == "Ethernet") // Get以太网
                {
                    if (vM == nis[i].GetPhysicalAddress().ToString()) // Mac地址确认
                    {
                        return true;
                    };
                }
            }
            return false;
        }

        /// <summary>
        /// 时间验证链
        /// </summary>
        /// <param name="stampURL"></param>
        /// <returns></returns>
        private IEnumerator CheckTimeStampChain(string stampURL)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(stampURL);
            yield return webRequest.SendWebRequest();
            // if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            // {
            //     // Debug.Log(webRequest.error);
            // }
            JObject jb = JObject.Parse(webRequest.downloadHandler.text);
            long networkStamp = long.Parse(jb["data"]["t"].ToString());
            // Debug.Log(jb["data"]["t"]); // 时间戳

            long localStamp = long.Parse(verifikadoStamp); // 转long
            if (networkStamp > localStamp)
            {
                checker = false;
            }
            else
            {
                checker = true;
            }

            dtTMCmpt.text += "\n> Check the Authorization Date: <color=#FF0000>" + checker + "</color>"; // DEBUG
            if (!checker)
            {
                ApplicationError("授权可能已过期。");
                yield break;
            }

            // 跳转
            dtTMCmpt.text += "\n> Done.";
            LoadNextScene();
            yield break;
        }
        #endregion

        #region TextConvert
        /// <summary>
        /// 字符串转二进制
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected string StringToBinary(string str)
        {
            byte[] data = Encoding.Default.GetBytes(str);
            StringBuilder sb = new StringBuilder(data.Length * 8);
            foreach (byte item in data)
            {
                sb.Append(Convert.ToString(item, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 二进制转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected string BinaryToString(string str)
        {
            System.Text.RegularExpressions.CaptureCollection cs = System.Text.RegularExpressions.Regex.Match(str, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[cs.Count];
            for (int i = 0; i < cs.Count; i++)
            {
                data[i] = Convert.ToByte(cs[i].Value, 2);
            }
            return Encoding.Default.GetString(data, 0, data.Length);
        }
        #endregion

        #region Other
        private void ApplicationError(string message)
        {
            MessageBox(IntPtr.Zero, message + "\n您可能是盗版程序的受害者，程序即将退出。\n\n<" + DateTime.Now.ToString() + ">", "Verify - Application Error", 0);
            Invoke("ApplicationQuit", 3f);
            return;
        }

        private void ApplicationQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return;
        }

        private void LoadNextScene()
        {
            if (SceneManager.sceneCountInBuildSettings >= 2) // 场景大于2加载下一个场景
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            return;
        }
        #endregion
    }
}