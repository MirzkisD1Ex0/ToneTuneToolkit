/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace ToneTuneToolkit.LED
{
  /// <summary>
  /// 核弹秀
  /// </summary>
  public class LEDNuclearShow : MonoBehaviour
  {
    private GameObject nuclearGO;
    private Image nImC;
    private Button nBuC;
    private Color color = Color.white;
    private bool isShowing = false;

    private void Start()
    {
      this.nuclearGO = GameObject.Find("Button - Nuclear");
      this.nImC = this.nuclearGO.GetComponent<Image>();
      this.nBuC = this.nuclearGO.GetComponent<Button>();

      this.nBuC.onClick.AddListener(this.StartNuclear);
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartNuclear()
    {
      if (!this.isShowing)
      {
        InvokeRepeating("RandomColor", 0, .1f);
      }
      else
      {
        CancelInvoke();
        this.nImC.color = new Color(0, 0, 0, 0);
      }
      this.isShowing = !this.isShowing;
      return;
    }

    private void RandomColor()
    {
      this.color = new Color(Random.Range(0f, 255f) / 255, Random.Range(0f, 255f) / 255, Random.Range(0f, 255f) / 255, 1);
      this.nImC.color = this.color;

      LEDCommandCenter.Instance.SLDimColor("#" + ColorUtility.ToHtmlStringRGB(this.color));
      return;
    }
  }
}