/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// 暂未确定用途
/// </summary>
[RequireComponent(typeof(ARAnchor))]
public class ARAnchorHandler : MonoBehaviour
{

  // ==================================================

  private void Start() => Init();

  // ==================================================

  private void Init()
  {
    LoadAnchorModel();
  }

  // ==================================================

  private void LoadAnchorModel()
  {
    if (ARAnchorSetter.Instance.isSaveARAnchorData)
    {
      GameObject mapobj = Instantiate(Resources.Load("AY5 - Static"), transform) as GameObject;
      Debug.Log(@$"位置:{mapobj.transform.position}");
      Debug.Log(@$"父对象名称:{mapobj.transform.parent}");
      Debug.Log(@$"爷对象名称:{mapobj.transform.parent.parent}");
      Debug.Log(@$"本地位置:{mapobj.transform.localPosition}");

      // // 未知问题导致位置对不上 // 生成物的父对象难以确定
      // GameObject NodeAllInOneGo = GameObject.Find("Node - All In One");
      // Debug.Log("已找到AllInOne");
      // NodeAllInOneGo.transform.position = mapobj.transform.position;
      // NodeAllInOneGo.transform.rotation = mapobj.transform.rotation;
      // Debug.Log(@$"已移动AllInOne到{NodeAllInOneGo.transform.position}");
    }
  }
}
