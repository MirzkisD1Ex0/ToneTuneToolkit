using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit.SerialPort
{
  public class SerialPortUtilityProResponder : MonoBehaviour
  {
    public static SerialPortUtilityProResponder Instance;

    // ==============================

    private void Awake() => Instance = this;
    private void Start() => Init();
    private void OnDestroy() => Uninit();

    // ==============================

    private void Init()
    {
      SerialPortUtilityProManager.OnReceiveMessage += MessageProcessor;
      return;
    }

    private void Uninit()
    {
      SerialPortUtilityProManager.OnReceiveMessage -= MessageProcessor;
      return;
    }

    // ==============================

    // AA 00 09 00 00 BB
    // AA 00 09 00 04 BB

    /// <summary>
    /// 消息翻译器
    /// </summary>
    /// <param name="value"></param>
    private void MessageProcessor(string value)
    {
      string[] parts = value.Split(" ");
      for (int i = 0; i < parts.Length; i++)
      {
        if (parts[i] == "04")
        {
          // GameManager.Instance.EnterStage03SerialPort();
          // GameManager.Instance.SetShootingGoal(true);
          Debug.Log("asdas");
        }
      }
      return;
    }
  }
}