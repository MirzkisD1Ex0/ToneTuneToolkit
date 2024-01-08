using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MartellController
{
  /// <summary>
  /// 日志管理器
  /// </summary>
  public class LogManager : MonoBehaviour
  {
    public static LogManager Instance;

    private static Text TextLog;

    // ==================================================

    private void Awake()
    {
      Instance = this;
      TextLog = GameObject.Find("Text - Log").GetComponent<Text>();
    }

    public static void Log(string logMessage)
    {
      string logCache = $"Log >>> {logMessage}...[<color=green>OK</color>]";
      TextLog.text += "\n" + logCache;
      Debug.Log(logCache);
      return;
    }

    public static void ErrorLog(string logMessage)
    {
      string logCache = $"Log >>> {logMessage}...[<color=red>ER</color>]";
      TextLog.text += "\n" + logCache;
      Debug.Log(logCache);
    }
  }
}