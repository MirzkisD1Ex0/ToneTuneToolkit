using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// OK
  /// 时间戳获取器
  /// 自带一个时间戳转日期的方法
  /// </summary>
  /// https://tool.lu/timestamp/
  public class TimestampCapturer : MonoBehaviour
  {
    public static TimestampCapturer Instance;
    private static string stampURL = "http://api.m.taobao.com/rest/api3.do?api=mtop.common.getTimestamp"; // 时间戳提供者

    private void Awake()
    {
      Instance = this;
    }

    /// <summary>
    /// 获取本地时间戳
    /// </summary>
    public static long GetLocalTimestamp()
    {
      TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0);
      long millsecondTimeStamp = (long)Convert.ToUInt64(ts.TotalMilliseconds);
      return millsecondTimeStamp;
    }

    /// <summary>
    /// 获取网络时间戳的简易方法
    /// </summary>
    public void GetNetTimestamp()
    {
      StartCoroutine(RequestTimestamp(stampURL));
      return;
    }

    /// <summary>
    /// Undone
    /// 获取网络时间戳
    /// 我不知道如何从协程中返回时间戳
    /// 如果不用协程网络请求还没有结果的时候下一行语句就已经执行了
    /// </summary>
    /// <returns></returns>
    private IEnumerator RequestTimestamp(string stampURL)
    {
      UnityWebRequest webRequest = UnityWebRequest.Get(stampURL);
      yield return webRequest.SendWebRequest();
      if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) // 报错预警
      {
        TipTools.Error(webRequest.error);
        yield break;
      }
      JObject jb = JObject.Parse(webRequest.downloadHandler.text);

      long longTime = long.Parse(jb["data"]["t"].ToString());

      TipTools.Notice("Timestamp=>" + longTime);
      TipTools.Notice("DataTime=>" + DataConverter.ConvertTimestamp2DateTime(longTime));
      yield break;
    }
  }
}