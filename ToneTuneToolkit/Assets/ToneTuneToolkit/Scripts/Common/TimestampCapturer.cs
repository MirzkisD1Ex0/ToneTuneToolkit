/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// 时间戳获取器
  /// 自带一个时间戳转日期的方法
  /// https://tool.lu/timestamp/
  /// </summary>
  public class TimestampCapturer : MonoBehaviour
  {
    public static TimestampCapturer Instance;
    private string stampURL = "http://api.m.taobao.com/rest/api3.do?api=mtop.common.getTimestamp"; // 时间戳提供者

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
      StartCoroutine(RequestTimestamp(this.stampURL));
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
        Debug.Log($"[TimestampCapturer] WebRequest error [<color=red>{webRequest.error}</color>]...[Er]");
        yield break;
      }
      JObject jb = JObject.Parse(webRequest.downloadHandler.text);

      long longTime = long.Parse(jb["data"]["t"].ToString());

      Debug.Log($"[TimestampCapturer] Timestamp=> [<color=green>{longTime}</color>]...[OK]");
      Debug.Log($"[TimestampCapturer] DataTime=> [<color=green>{DataConverter.ConvertTimestamp2DateTime(longTime)}</color>]...[OK]");
      yield break;
    }
  }
}