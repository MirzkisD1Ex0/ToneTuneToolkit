using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// Unfinished
  /// Json处理工具
  /// </summary>
  public class JsonConstructer : MonoBehaviour
  {
    /// <summary>
    /// 将字典转化为json
    /// </summary>
    /// <param name="jsonDic">字典</param>
    /// <returns>字符串</returns>
    public static string Dic2Json(Dictionary<string, string> jsonDic)
    {
      string jsonString = JsonConvert.SerializeObject(jsonDic);
      return jsonString;
    }

    /// <summary>
    /// json转为字典
    /// </summary>
    /// <param name="jsonString">json</param>
    /// <returns>字典</returns>
    public static Dictionary<string, string> Json2Dic(string jsonString)
    {
      Dictionary<string, string> jsonDic = new Dictionary<string, string>();
      jsonDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
      return jsonDic;
    }

    // /// <summary>
    // /// 数据类
    // /// </summary>
    // public class JsonPackage
    // {
    //   public string field01; // 字段
    //   public string field02;
    //   // public Dictionary<string, string> dictionary = new Dictionary<string, string>(); // 对象
    //   // product.dictionary["Local IP"] = "192.168.1.1";
    //   // product.dictionary["Target IP"] = "192.168.1.1";
    // }
  }
}