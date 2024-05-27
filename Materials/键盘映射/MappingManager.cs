using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.Data;

namespace SignalProcessingModule
{
  public class MappingManager : MonoBehaviour
  {
    public static MappingManager Instance;

    private string configFilePath = "/configs/keymapping.json";

    private string
      keyCodeAlpha0, keyCodeAlpha1, keyCodeAlpha2, keyCodeAlpha3, keyCodeAlpha4,
      keyCodeAlpha5, keyCodeAlpha6, keyCodeAlpha7, keyCodeAlpha8, keyCodeAlpha9,
      keyCodeUpArrow, keyCodeDownArrow;

    private bool isInit = false;

    // ========================================

    private void Awake()
    {
      Instance = this;
      Init();
    }

    private void Update()
    {
      KeyboardMapping();
    }

    // ========================================

    private void Init()
    {
      configFilePath = Application.streamingAssetsPath + configFilePath;
      keyCodeAlpha0 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha0");
      keyCodeAlpha1 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha1");
      keyCodeAlpha2 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha2");
      keyCodeAlpha3 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha3");
      keyCodeAlpha4 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha4");
      keyCodeAlpha5 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha5");
      keyCodeAlpha6 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha6");
      keyCodeAlpha7 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha7");
      keyCodeAlpha8 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha8");
      keyCodeAlpha9 = JsonManager.GetJsonAsString(configFilePath, "KeyCodeAlpha9");
      keyCodeUpArrow = JsonManager.GetJsonAsString(configFilePath, "KeyCodeUpArrow");
      keyCodeDownArrow = JsonManager.GetJsonAsString(configFilePath, "KeyCodeDownArrow");
      isInit = true;
      return;
    }

    private void KeyboardMapping()
    {
      if (!isInit)
      {
        return;
      }

      if (Input.GetKeyDown(keyCodeAlpha0))
      {
        Debug.Log("0");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha0, "0");
      }
      if (Input.GetKeyDown(keyCodeAlpha1))
      {
        Debug.Log("1");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha1, "1");
      }
      if (Input.GetKeyDown(keyCodeAlpha2))
      {
        Debug.Log("2");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha2, "2");
      }
      if (Input.GetKeyDown(keyCodeAlpha3))
      {
        Debug.Log("3");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha3, "3");
      }
      if (Input.GetKeyDown(keyCodeAlpha4))
      {
        Debug.Log("4");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha4, "4");
      }
      if (Input.GetKeyDown(keyCodeAlpha5))
      {
        Debug.Log("5");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha5, "5");
      }
      if (Input.GetKeyDown(keyCodeAlpha6))
      {
        Debug.Log("6");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha6, "6");
      }
      if (Input.GetKeyDown(keyCodeAlpha7))
      {
        Debug.Log("7");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha7, "7");
      }
      if (Input.GetKeyDown(keyCodeAlpha8))
      {
        Debug.Log("8");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha8, "8");
      }
      if (Input.GetKeyDown(keyCodeAlpha9))
      {
        Debug.Log("9");
        TestManager.Instance.UpdateDebugText(keyCodeAlpha9, "9");
      }
      if (Input.GetKeyDown(keyCodeUpArrow))
      {
        Debug.Log("Up");
        TestManager.Instance.UpdateDebugText(keyCodeUpArrow, "Up Arrow");
      }
      if (Input.GetKeyDown(keyCodeDownArrow))
      {
        Debug.Log("Down");
        TestManager.Instance.UpdateDebugText(keyCodeDownArrow, "Down Arrow");
      }

      // if (Input.anyKeyDown)
      // {

      // }

      return;
    }

  }
}