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
  /// </summary>
  /// https://tool.lu/timestamp/
  public class TimestampCapturer : MonoBehaviour
  {
    private static string StampURL = "http://api.m.taobao.com/rest/api3.do?api=mtop.common.getTimestamp"; // 时间戳提供者

    public string a;

    private void Start()
    {
      a = GetTimestamp();
      Debug.Log(a);
    }

    public string GetTimestamp()
    {
      StartCoroutine(RequestTimestamp(StampURL, GGO));
      return null;
    }

    private void GGO(string tt)
    {
      // string dd
      this.a = tt;
      Debug.Log(tt);
      return;
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    private IEnumerator RequestTimestamp(string stampURL, Action<string> timestamp)
    {
      UnityWebRequest webRequest = UnityWebRequest.Get(stampURL);
      yield return webRequest.SendWebRequest();
      if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
      {
        Debug.Log(webRequest.error);
      }

      JObject jb = JObject.Parse(webRequest.downloadHandler.text);


      long longTime = long.Parse(jb["data"]["t"].ToString());
      Debug.Log("Timestamp: " + longTime);

      string stringTime = jb["data"]["t"].ToString();
      Debug.Log("DataTime: " + ConvertStringToDateTime(stringTime));

      if (timestamp != null)
      {
        timestamp(stringTime);
      }
    }

    /// <summary>
    /// 时间戳转为C#格式时间
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    private DateTime ConvertStringToDateTime(string timeStamp)
    {
      DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
      long lTime = long.Parse(timeStamp + "0000");
      TimeSpan toNow = new TimeSpan(lTime);
      return dtStart.Add(toNow);
    }
  }
}