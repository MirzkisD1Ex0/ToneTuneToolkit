/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Common;
using UnityEngine;

namespace ToneTuneToolkit.RealisticCarController
{
  public class RCC_PlayerManager : SingletonMaster<RCC_PlayerManager>
  {
    [SerializeField] private RCC_CarControllerV4 spawnVehiclePrefab;
    [SerializeField] private GameObject goSpawnPoint;

    private RCC_CarControllerV4 currentVehiclePrefab;

    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    private void Init()
    {
      RCC_LogiSteeringManager.OnSteeringKey23Pressed += SwitchCamera;
      SpawnVehicle();
    }

    private void UnInit()
    {
      RCC_LogiSteeringManager.OnSteeringKey23Pressed -= SwitchCamera;
    }

    // ==================================================

    public void SpawnVehicle()
    {
      if (currentVehiclePrefab != null) { return; }
      currentVehiclePrefab = RCC.SpawnRCC(spawnVehiclePrefab, goSpawnPoint.transform.position, goSpawnPoint.transform.rotation, false, false, false);
      SwitchPlayer(true);
      SwitchEngine(true);
    }

    public void DestroyVehicle()
    {
      if (currentVehiclePrefab == null) { return; }
      SwitchControl(false);
      SwitchEngine(false);
      SwitchPlayer(false);
      ForceBrake();
      Destroy(currentVehiclePrefab.gameObject);
    }

    public void SwitchPlayer(bool value)
    {
      if (value) { RCC.RegisterPlayerVehicle(currentVehiclePrefab); }
      else { RCC.DeRegisterPlayerVehicle(); }
    }

    public void SwitchControl(bool control)
    {
      RCC.SetControl(currentVehiclePrefab, control);
    }

    public void SwitchEngine(bool engine)
    {
      RCC.SetEngine(currentVehiclePrefab, engine);
    }

    private bool isCameaFPS = false;
    public void SwitchCamera()
    {
      if (isCameaFPS)
      { RCC_SceneManager.Instance.activePlayerCamera.ChangeCamera(RCC_Camera.CameraMode.FPS); }
      else
      { RCC_SceneManager.Instance.activePlayerCamera.ChangeCamera(RCC_Camera.CameraMode.TPS); }
      isCameaFPS = !isCameaFPS;
    }

    public void ForceBrake()
    {
      RCC_Inputs forceBrakeInputs = new RCC_Inputs();
      forceBrakeInputs.throttleInput = 0f;    // 强制切断油门
      forceBrakeInputs.brakeInput = 1f;       // 强制踩死刹车
      forceBrakeInputs.handbrakeInput = 1f;   // 强制拉起手刹
      currentVehiclePrefab.OverrideInputs(forceBrakeInputs);
    }
  }
}
