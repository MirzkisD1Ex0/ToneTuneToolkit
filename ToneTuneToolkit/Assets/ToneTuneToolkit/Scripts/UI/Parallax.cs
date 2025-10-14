/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.UI
{
  /// <summary>
  /// 多层次视差
  /// </summary>
  public class Parallax : MonoBehaviour
  {
    public GameObject[] ParallaxGO; // 通常第一个为壁纸
    public float[] parallaxLevel = new float[] {
      0.05f,
      0.1f }; // 视差等级 // 数值越高偏移越大

    private Vector2 parallaxOffset = Vector2.zero;
    private Vector2 screenOffset = Vector2.zero;
    private Vector2[] specialOffset;

    // ==================================================

    private void Start()
    {
      if (ParallaxGO.Length == 0)
      {
        Debug.Log("[Parallax] Cant find Parallax Object(s)...[Er]");
        enabled = false;
        return;
      }
      for (int i = 0; i < ParallaxGO.Length; i++)
      {
        if (!ParallaxGO[i])
        {
          Debug.Log("[Parallax] Parallax Object(s) missing...[Er]");
          enabled = false;
          return;
        }
      }
      Init();
    }

    private void Update()
    {
      ParallaxMethod();
    }

    // ==================================================

    private void Init()
    {
      screenOffset = new Vector2(Screen.width / 2, Screen.height / 2);
      specialOffset = new Vector2[ParallaxGO.Length];
      for (int i = 0; i < ParallaxGO.Length; i++)
      {
        specialOffset[i] = ParallaxGO[i].transform.localPosition;
      }
      ParallaxGO[0].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f); // 背景图片增加至1.1倍
      return;
    }

    private void ParallaxMethod()
    {
      parallaxOffset.x = Input.mousePosition.x - screenOffset.x;
      parallaxOffset.y = Input.mousePosition.y - screenOffset.y;

      ParallaxGO[0].transform.localPosition = parallaxOffset * parallaxLevel[0] + specialOffset[0];
      ParallaxGO[1].transform.localPosition = parallaxOffset * parallaxLevel[1] + specialOffset[1];
      return;
    }
  }
}
