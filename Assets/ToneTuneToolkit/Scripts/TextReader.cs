using UnityEngine;
using System.IO;

namespace ToneTuneToolkit
{
    public class TextReader : MonoBehaviour
    {
        /// <summary>
        /// 读取文本内容
        /// </summary>
        /// <param name="url">文件路径</param>
        /// <param name="line">要读取的文件行数</param>
        /// <returns></returns>
        protected static string GetConfig(string url, int line)
        {
            string[] tempStringArray = File.ReadAllLines(url);
            if (line > 0)
            {
                return tempStringArray[line - 1].Split('=')[1]; // 等号分隔 // 读取第二部分
            }
            else
            {
                return null;
            }
        }
    }
}