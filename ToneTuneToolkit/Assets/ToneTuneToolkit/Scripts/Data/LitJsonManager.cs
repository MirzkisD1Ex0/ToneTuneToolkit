/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using LitJson;

namespace ToneTuneToolkit.Data
{
  public class LitJsonManager : MonoBehaviour
  {
    public static object GetJson(string url, string keyName)
    {
      string jsonString = File.ReadAllText(url, Encoding.UTF8);
      JsonData jd = JsonMapper.ToObject(jsonString);
      return jd[keyName];
    }
  }
}
