/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit.Common
{
  /// <summary>
  /// 单例大师基类
  /// 你值得拥有
  /// ScreenshotMaster : SingletonMaster<ScreenshotMaster>
  /// private new void Awake()
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class SingletonMaster<T> : MonoBehaviour where T : SingletonMaster<T>, new()
  {
    private static T _instance;
    public static T Instance
    {
      get
      {
        _instance = FindObjectOfType<T>();
        if (_instance == null)
        {
          _instance = new T();
        }
        return _instance;
      }
    }

    private void Awake()
    {
      if (_instance == null)
      {
        _instance = this as T;
      }
    }
  }
}
