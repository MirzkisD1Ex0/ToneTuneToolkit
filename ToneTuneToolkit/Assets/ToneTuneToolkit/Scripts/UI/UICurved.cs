/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
  /// <summary>
  /// UI Curved
  /// 对Text的操作就和 shadow 和 outline 组件类似。
  /// </summary>
  [AddComponentMenu("UI/Effects/Extensions/UI Curved")]
  public class UICurved : BaseMeshEffect
  {
    [SerializeField] private AnimationCurve curveForText = AnimationCurve.Linear(0, 0, 1, 10); // 曲线类型
    [SerializeField] private float curveMultiplier = 1; // 曲线程度

    private RectTransform rectTransform;

    public void ChangeCurveultiplier(float value)
    {
      curveMultiplier = value;
      OnValidate(); // 值改变后会调用
      return;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
      base.OnValidate();
      if (curveForText[0].time != 0)
      {
        var tmpRect = curveForText[0];
        tmpRect.time = 0;
        curveForText.MoveKey(0, tmpRect);
      }
      if (rectTransform == null)
      {
        rectTransform = GetComponent<RectTransform>();
      }
      if (curveForText[curveForText.length - 1].time != rectTransform.rect.width)
      {
        OnRectTransformDimensionsChange();
      }
      return;
    }
#endif

    protected override void Awake()
    {
      base.Awake();
      rectTransform = GetComponent<RectTransform>();
      OnRectTransformDimensionsChange();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      rectTransform = GetComponent<RectTransform>();
      OnRectTransformDimensionsChange();
    }

    /// <summary>
    /// Modifies the mesh.
    /// 最重要的重载函数
    /// </summary>
    /// <param name="mesh">Mesh.</param>
    public override void ModifyMesh(VertexHelper vh)
    {
      if (!IsActive())
      {
        return;
      }

      // 从mesh 得到 顶点集
      List<UIVertex> verts = new List<UIVertex>();

      vh.GetUIVertexStream(verts);

      // 顶点的 y值按曲线变换
      for (int index = 0; index < verts.Count; index++)
      {
        var uiVertex = verts[index];
        uiVertex.position.y += curveForText.Evaluate(rectTransform.rect.width * rectTransform.pivot.x + uiVertex.position.x) * curveMultiplier;
        verts[index] = uiVertex;
      }

      // 在合成mesh
      vh.AddUIVertexTriangleStream(verts);
      return;
    }

    protected override void OnRectTransformDimensionsChange()
    {
      var tmpRect = curveForText[curveForText.length - 1];
      if (rectTransform != null)
      {
        tmpRect.time = rectTransform.rect.width;
        curveForText.MoveKey(curveForText.length - 1, tmpRect);
      }
      return;
    }
  }
}
