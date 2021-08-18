/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace ToneTuneToolkit.UI
{
  /// <summary>
  /// 控件闪烁
  ///
  /// 需要挂在对象上
  /// </summary>
  public class TextFlick : MonoBehaviour
  {
    public float MinAlpha = 102f; // 最小透明度
    public float MaxAlpha = 255f; // 最大透明度
    public float FlickSpeed = 150f; // 速度

    private float floatingValue = 0;
    private bool isFull = false;
    private Color newColor;
    private Text tCmpt;

    private void Start()
    {
      this.tCmpt = GetComponent<Text>();
      this.newColor = this.tCmpt.color;
    }

    private void Update()
    {
      this.TextAlphaPingpong();
    }

    /// <summary>
    /// 文字透明度浮动
    /// </summary>
    private void TextAlphaPingpong()
    {
      if (this.floatingValue < this.MaxAlpha && !this.isFull)
      {
        this.floatingValue += Time.deltaTime * this.FlickSpeed;
        if (this.floatingValue >= this.MaxAlpha)
        {
          this.isFull = true;
        }
      }
      else if (this.floatingValue > this.MinAlpha && this.isFull)
      {
        this.floatingValue -= Time.deltaTime * this.FlickSpeed;
        if (this.floatingValue <= this.MinAlpha)
        {
          this.isFull = false;
        }
      }
      this.newColor.a = this.floatingValue / 255;
      this.tCmpt.color = this.newColor;
      return;
    }
  }
}