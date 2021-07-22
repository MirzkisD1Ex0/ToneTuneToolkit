using UnityEngine;
using UnityEngine.UI;

namespace ToneTuneToolkit
{
  /// <summary>
  /// OK
  /// 改变对象及所有子对象的颜色
  /// 材质、图片、Raw图片
  /// </summary>
  public class TTTTraverseObejctChangeColor : MonoBehaviour
  {
    public Color PresettingColor = Color.white;

    private void Start()
    {
      ChildsColorGiving();
    }

    /// <summary>
    /// 改变三种子对象包括自己的颜色
    /// </summary>
    private void ChildsColorGiving()
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