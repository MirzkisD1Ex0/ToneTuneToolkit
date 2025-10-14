/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Runtime.InteropServices;
using UnityEngine;

namespace ToneTuneToolkit.Other
{
  /// <summary>
  /// 模拟物理按键
  /// </summary>
  public class KeyPressSimulator : MonoBehaviour
  {
    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    public static extern void keybd_event(
        byte bvk, // 虚拟键值，ASCII
        byte bScan, // 0
        int dwFlags, // 0按下，1按住，2释放
        int dwExtraInfo // 0
        );

    // ==================================================

    /// <summary>
    /// 模拟按下物理按键
    /// </summary>
    /// <param name="asciiKeyCode">按键ASCII码</param>
    /// <param name="keyFlags">0按下/1按住/2释放</param>
    public static void KeyAction(int asciiKeyCode, int keyFlags)
    {
      if (keyFlags > 2 || keyFlags < 0)
      {
        Debug.Log("[KeyPressSimulator] KeyFlags Error, check it again...[Er]");
        return;
      }
      keybd_event((byte)asciiKeyCode, 0, keyFlags, 0);
      return;
    }
  }
}
