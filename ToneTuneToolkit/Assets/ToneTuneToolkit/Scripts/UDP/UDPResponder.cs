/// <summary>
/// Copyright (c) 2024 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.1
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.UDP
{
  public class UDPResponder : MonoBehaviour
  {
    private void Start()
    {
      UDPCommunicatorLite.Instance.AddEventListener(Responder);
    }

    private void OnDestroy()
    {
      UDPCommunicatorLite.Instance.RemoveEventListener(Responder);
    }

    private void OnApplicationQuit()
    {
      UDPCommunicatorLite.Instance.RemoveEventListener(Responder);
    }

    // ==================================================

    private void Responder(string message)
    {
      if (message == null)
      {
        return;
      }

      switch (message)
      {
        default: break;
        case "Test": Debug.Log("Testing."); break;
      }
      return;
    }
  }
}