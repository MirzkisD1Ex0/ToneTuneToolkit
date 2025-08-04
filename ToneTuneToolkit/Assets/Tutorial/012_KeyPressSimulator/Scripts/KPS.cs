using UnityEngine;
using ToneTuneToolkit.Other;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class KPS : MonoBehaviour
  {
    private void Start()
    {
      KeyPressSimulator.KeyAction(65, 0);
      KeyPressSimulator.KeyAction(65, 2);
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.A))
      {
        Debug.Log("模拟物理按键A成功");
      }
    }
  }
}