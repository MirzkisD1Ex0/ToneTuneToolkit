using System;
using System.Text;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// OK
  /// 数据转换器
  /// </summary>
  public class DataConverter
  {
    /// <summary>
    /// 字符串转二进制
    /// </summary>
    /// <param name="str">数据</param>
    /// <returns>二进制数据</returns>
    public static string StringToBinary(string str)
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
    public static string BinaryToString(string str)
    {
      System.Text.RegularExpressions.CaptureCollection cs = System.Text.RegularExpressions.Regex.Match(str, @"([01]{8})+").Groups[1].Captures;
      byte[] data = new byte[cs.Count];
      for (int i = 0; i < cs.Count; i++)
      {
        data[i] = Convert.ToByte(cs[i].Value, 2);
      }
      return Encoding.Default.GetString(data, 0, data.Length);
    }
  }
}