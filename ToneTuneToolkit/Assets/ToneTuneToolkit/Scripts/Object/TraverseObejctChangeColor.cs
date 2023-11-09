/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 遍历物体及所有子对象并改变颜色
  /// 材质、图片、Raw图片
  /// </summary>
  public class TraverseObejctChangeColor : MonoBehaviour
  {
    public Color PresettingColor = Color.white;

    // ==================================================

    private void Start()
    {
      GivingChildsColor();
    }

    // ==================================================

    /// <summary>
    /// 改变三种子对象包括自己的颜色
    /// </summary>
    private void GivingChildsColor()
    {
      Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
      foreach (Transform child in allChildren)
      {
        if (child.GetComponent<MeshRenderer>())
        {
          child.GetComponent<MeshRenderer>().material.color = PresettingColor;
          continue;
        }
        if (child.GetComponent<Image>())
        {
          child.GetComponent<Image>().color = PresettingColor;
          continue;
        }
        if (child.GetComponent<RawImage>())
        {
          child.GetComponent<RawImage>().color = PresettingColor;
          continue;
        }
      }
      return;
    }
  }
}