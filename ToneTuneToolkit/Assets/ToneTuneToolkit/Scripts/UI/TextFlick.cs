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
    private Text textCOM;

    // ==================================================

    private void Start()
    {
      Init();
    }

    private void Update()
    {
      TextAlphaPingpong();
    }

    // ==================================================

    private void Init()
    {
      textCOM = GetComponent<Text>();
      newColor = textCOM.color;
      return;
    }

    /// <summary>
    /// 文字透明度浮动
    /// </summary>
    private void TextAlphaPingpong()
    {
      if (floatingValue < MaxAlpha && !isFull)
      {
        floatingValue += Time.deltaTime * FlickSpeed;
        if (floatingValue >= MaxAlpha)
        {
          isFull = true;
        }
      }
      else if (floatingValue > MinAlpha && isFull)
      {
        floatingValue -= Time.deltaTime * FlickSpeed;
        if (floatingValue <= MinAlpha)
        {
          isFull = false;
        }
      }
      newColor.a = floatingValue / 255;
      textCOM.color = newColor;
      return;
    }
  }
}