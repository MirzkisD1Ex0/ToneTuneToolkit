/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.UDP
{
  public class UDPResponder : MonoBehaviour
  {
    private void Start()
    {
      UDPCommunicator.AddEventListener(Responder);
    }

    private void OnDestroy()
    {
      UDPCommunicator.RemoveEventListener(Responder);
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
