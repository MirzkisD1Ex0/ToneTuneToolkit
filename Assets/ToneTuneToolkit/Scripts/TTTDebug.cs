// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit
{
    /// <summary>
    /// Debug专用
    /// </summary>
    public class TTTDebug : MonoBehaviour
    {
        public static void Warning(string text)
        {
            Debug.Log(@"<color=" + "#FF0000" + ">" + "[TTT Warning] >" + "</color>" + text);
        }
    }
}