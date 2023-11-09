/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// 数据转换器
  /// </summary>
  public static class DataConverter
  {
    /// <summary>
    /// 字符串转二进制
    /// </summary>
    /// <param name="str">数据</param>
    /// <returns>二进制数据</returns>
    public static string String2Binary(string str)
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
    /// <param name="str">数据</param>
    /// <returns>字符串数据</returns>
    public static string Binary2String(string str)
    {
      CaptureCollection cs = Regex.Match(str, @"([01]{8})+").Groups[1].Captures;
      byte[] data = new byte[cs.Count];
      for (int i = 0; i < cs.Count; i++)
      {
        data[i] = Convert.ToByte(cs[i].Value, 2);
      }
      return Encoding.Default.GetString(data, 0, data.Length);
    }

    /// <summary>
    /// 时间戳转为C#格式时间
    /// </summary>
    /// <param name="millsecondTimeStamp">js时间戳(毫秒)</param>
    /// <returns>C#日期</returns>
    public static DateTime ConvertTimestamp2DateTime(long millsecondTimeStamp)
    {
      DateTime gmtDT = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 8, 0, 0), TimeZoneInfo.Local); // 格林威治时间
      TimeSpan ts = new TimeSpan(long.Parse(millsecondTimeStamp + "0000")); // 转成戳 // 不明白为什么要加0000
      return gmtDT.Add(ts);
    }

    /// <summary>
    /// 将字典转化为json
    /// </summary>
    /// <param name="jsonDic">字典</param>
    /// <returns>json</returns>
    public static string Dic2Json(Dictionary<string, string> jsonDic)
    {
      return JsonConvert.SerializeObject(jsonDic);
    }

    /// <summary>
    /// json转为字典
    /// </summary>
    /// <param name="jsonString">json</param>
    /// <returns>字典</returns>
    public static Dictionary<string, string> Json2Dic(string jsonString)
    {
      Dictionary<string, string> jsonDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
      return jsonDic;
    }
  }
}