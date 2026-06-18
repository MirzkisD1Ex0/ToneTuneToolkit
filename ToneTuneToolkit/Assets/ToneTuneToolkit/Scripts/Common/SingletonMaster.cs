/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

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
  public class SingletonMaster<T> : MonoBehaviour where T : MonoBehaviour
  {
    private static T _instance;

    public static T Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = FindFirstObjectByType<T>();
          if (_instance == null)
          {
            GameObject go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();
          }
        }
        return _instance;
      }
    }

    protected virtual void Awake()
    {
      if (_instance == null)
      {
        _instance = this as T;
      }
      else if (_instance != this)
      {
        Destroy(gameObject);
      }
    }
  }
}
