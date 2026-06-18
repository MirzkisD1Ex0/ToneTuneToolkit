/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using UnityEngine;
using System.IO;
using System.Text;
using LitJson;

namespace ToneTuneToolkit.Data
{
  public class LitJsonEditor : MonoBehaviour
  {
    public static object GetJson(string url, string keyName)
    {
      string jsonString = File.ReadAllText(url, Encoding.UTF8);
      JsonData jd = JsonMapper.ToObject(jsonString);
      return jd[keyName];
    }
  }
}
