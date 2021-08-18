/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
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

    private void Start()
    {
      if (this.ParallaxGO.Length == 0)
      {
        TipTools.Error("[Parallax] Cant find Parallax Object(s).");
        this.enabled = false;
        return;
      }
      for (int i = 0; i < this.ParallaxGO.Length; i++)
      {
        if (!this.ParallaxGO[i])
        {
          TipTools.Error("[Parallax] Parallax Object(s) missing.");
          this.enabled = false;
          return;
        }
      }
      this.Presetting();
    }

    private void Update()
    {
      this.ParallaxMethod();
    }

    private void Presetting()
    {
      this.screenOffset = new Vector2(Screen.width / 2, Screen.height / 2);
      this.specialOffset = new Vector2[this.ParallaxGO.Length];
      for (int i = 0; i < this.ParallaxGO.Length; i++)
      {
        this.specialOffset[i] = this.ParallaxGO[i].transform.localPosition;
      }
      this.ParallaxGO[0].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f); // 背景图片增加至1.1倍
      return;
    }

    private void ParallaxMethod()
    {
      this.parallaxOffset.x = Input.mousePosition.x - this.screenOffset.x;
      this.parallaxOffset.y = Input.mousePosition.y - this.screenOffset.y;

      this.ParallaxGO[0].transform.localPosition = this.parallaxOffset * this.parallaxLevel[0] + this.specialOffset[0];
      this.ParallaxGO[1].transform.localPosition = this.parallaxOffset * this.parallaxLevel[1] + this.specialOffset[1];
      return;
    }
  }
}