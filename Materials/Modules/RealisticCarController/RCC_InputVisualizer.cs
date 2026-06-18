/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace ToneTuneToolkit.RealisticCarController
{
  public class RCC_InputVisualizer : MonoBehaviour
  {
    [SerializeField] private Image throttleBar;
    [SerializeField] private Image brakeBar;

    private RCC_CarControllerV4 playerCar;

    // ==================================================

    private void Update()
    {
      if (RCC_SceneManager.Instance.activePlayerVehicle != null)
      {
        playerCar = RCC_SceneManager.Instance.activePlayerVehicle;
      }

      if (playerCar == null) { return; }

      float t = playerCar.throttleInput;
      float b = playerCar.brakeInput;

      if (throttleBar != null) throttleBar.fillAmount = t;
      if (brakeBar != null) brakeBar.fillAmount = b;
    }
  }
}
