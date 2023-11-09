/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;

namespace ToneTuneToolkit.UDP
{
  public class UDPResponder : MonoBehaviour
  {
    private void Update()
    {
      Responder();
    }

    // ==================================================

    private void Responder()
    {
      if (UDPHandler.UDPMessage == null)
      {
        return;
      }

      switch (UDPHandler.UDPMessage)
      {
        default: break;
        case "Test": Debug.Log("Testing."); break;
      }

      UDPHandler.UDPMessage = null;
      return;
    }
  }
}