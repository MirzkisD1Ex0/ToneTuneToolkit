/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>



using UnityEngine;

namespace ToneTuneToolkit.UDP
{
  public class UDPResponder : MonoBehaviour
  {
    private void Start()
    {
      UDPCommunicatorLite.AddEventListener(Responder);
    }

    private void OnDestroy()
    {
      UDPCommunicatorLite.RemoveEventListener(Responder);
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
