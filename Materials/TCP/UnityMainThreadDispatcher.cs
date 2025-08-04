using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
  private static UnityMainThreadDispatcher _instance;
  private static readonly Queue<System.Action> _executionQueue = new Queue<System.Action>();

  // 确保在Awake中初始化实例
  void Awake()
  {
    if (_instance == null)
    {
      _instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public static UnityMainThreadDispatcher Instance()
  {
    if (_instance == null)
    {
      Debug.LogError("UnityMainThreadDispatcher not found in scene. Please add it to a GameObject.");
    }
    return _instance;
  }

  public void Enqueue(System.Action action)
  {
    lock (_executionQueue)
    {
      _executionQueue.Enqueue(action);
    }
  }

  void Update()
  {
    lock (_executionQueue)
    {
      while (_executionQueue.Count > 0)
      {
        _executionQueue.Dequeue().Invoke();
      }
    }
  }
}