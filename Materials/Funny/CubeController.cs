using UnityEngine;

public class CubeController : MonoBehaviour
{
  [SerializeField] private bool isDebug = false;
  [Header("基础旋转设置")]
  [SerializeField] private float baseRotationSpeed = 30f;
  [SerializeField] private float directionChangeInterval = 3f;

  [Header("随机范围")]
  [SerializeField] private Vector3 minRotationSpeed = new Vector3(-50, -50, -50);
  [SerializeField] private Vector3 maxRotationSpeed = new Vector3(50, 50, 50);

  [Header("增强效果设置")]
  [SerializeField] private bool useEaseInOut = true;
  [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
  [SerializeField] private bool useRandomCurve = false;
  [SerializeField] private AnimationCurve[] randomCurves;

  private Vector3 targetRotationSpeed;
  private Vector3 currentRotationSpeed;
  private Vector3 previousRotationSpeed;
  private float timeSinceLastDirectionChange;
  private AnimationCurve currentCurve;

  // ==================================================

  private void Start() => Init();

  private void Update()
  {
    // 更新时间
    timeSinceLastDirectionChange += Time.deltaTime;

    // 计算插值进度（0到1）
    float progress = Mathf.Clamp01(timeSinceLastDirectionChange / directionChangeInterval);

    // 应用缓动插值
    if (useEaseInOut && currentCurve != null)
    {
      float easedProgress = currentCurve.Evaluate(progress);
      currentRotationSpeed = Vector3.Lerp(previousRotationSpeed, targetRotationSpeed, easedProgress);
    }
    else
    {
      // 线性插值
      currentRotationSpeed = Vector3.Lerp(previousRotationSpeed, targetRotationSpeed, progress);
    }

    // 应用旋转
    transform.Rotate(currentRotationSpeed * Time.deltaTime);

    // 定期改变旋转方向和曲线
    if (timeSinceLastDirectionChange >= directionChangeInterval)
    {
      GenerateNewRotationTarget();
      if (useRandomCurve)
      {
        SelectRandomCurve();
      }
      timeSinceLastDirectionChange = 0f;
    }
  }

  private void OnGUI() => DEBUG_ShowInfo();
  private void OnDrawGizmos() => DEBUG_DrawGizmo();
  private void OnDestroy() => UnInit();

  // ==================================================

  private void Init()
  {
    // 初始化随机曲线数组（如果没有手动赋值）
    if (randomCurves == null || randomCurves.Length == 0) { SetDefaultCurves(); }

    // 初始化旋转速度
    GenerateNewRotationTarget();
    currentRotationSpeed = targetRotationSpeed;
    previousRotationSpeed = currentRotationSpeed;

    // 选择初始曲线
    SelectRandomCurve();

    // Wireframe
    meshFilter = GetComponent<MeshFilter>();
    CreateLineMaterial();

    // 隐藏原来的网格渲染器
    MeshRenderer originalRenderer = GetComponent<MeshRenderer>();
    if (originalRenderer != null)
    {
      originalRenderer.enabled = false;
    }
  }

  private void UnInit()
  {
    if (lineMaterial != null) { DestroyImmediate(lineMaterial); }
  }

  // ==================================================

  /// <summary>
  /// 设置默认曲线
  /// </summary>
  private void SetDefaultCurves()
  {
    // 创建一组默认的动画曲线
    randomCurves = new AnimationCurve[6];

    // 1. 标准缓入缓出
    randomCurves[0] = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // 2. 快速开始慢速结束
    randomCurves[1] = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.2f, 0.8f),
        new Keyframe(1, 1)
    );

    // 3. 慢速开始快速结束
    randomCurves[2] = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.8f, 0.2f),
        new Keyframe(1, 1)
    );

    // 4. 弹性效果
    randomCurves[3] = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.3f, 1.1f),
        new Keyframe(0.6f, 0.9f),
        new Keyframe(1, 1)
    );

    // 5. 阶梯效果
    randomCurves[4] = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.3f, 0.3f),
        new Keyframe(0.31f, 0.7f),
        new Keyframe(1, 1)
    );

    // 6. 平滑线性
    randomCurves[5] = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(1, 1)
    );
  }



  private void GenerateNewRotationTarget()
  {
    // 保存当前速度作为插值起点
    previousRotationSpeed = currentRotationSpeed;

    // 生成新的随机旋转速度
    targetRotationSpeed = new Vector3(
        Random.Range(minRotationSpeed.x, maxRotationSpeed.x),
        Random.Range(minRotationSpeed.y, maxRotationSpeed.y),
        Random.Range(minRotationSpeed.z, maxRotationSpeed.z)
    );
  }

  private void SelectRandomCurve()
  {
    if (randomCurves != null && randomCurves.Length > 0)
    {
      int randomIndex = Random.Range(0, randomCurves.Length);
      currentCurve = randomCurves[randomIndex];
    }
    else
    {
      // 回退到默认缓动曲线
      currentCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }
  }


  // 编辑器方法：重新生成目标
  [ContextMenu("重新生成旋转目标")]
  public void RegenerateTarget()
  {
    GenerateNewRotationTarget();
    if (useRandomCurve)
    {
      SelectRandomCurve();
    }
    timeSinceLastDirectionChange = 0f;
  }

  // 编辑器方法：设置基础速度
  [ContextMenu("应用基础速度")]
  public void ApplyBaseSpeed()
  {
    minRotationSpeed = new Vector3(-baseRotationSpeed, -baseRotationSpeed, -baseRotationSpeed);
    maxRotationSpeed = new Vector3(baseRotationSpeed, baseRotationSpeed, baseRotationSpeed);
    RegenerateTarget();
  }

  // ==================================================
  #region Wireframe

  [Header("线框设置")]
  [SerializeField] private Color wireframeColor = Color.cyan;
  [SerializeField] private float lineWidth = 0.02f;

  private MeshFilter meshFilter;
  private Material lineMaterial;

  private void CreateLineMaterial()
  {
    // 创建用于绘制线的材质
    lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
    lineMaterial.hideFlags = HideFlags.HideAndDontSave;
  }

  private void OnRenderObject()
  {
    if (meshFilter == null || meshFilter.sharedMesh == null)
      return;

    // 设置材质
    lineMaterial.SetPass(0);

    GL.PushMatrix();
    GL.MultMatrix(transform.localToWorldMatrix);

    GL.Begin(GL.LINES);
    GL.Color(wireframeColor);

    Mesh mesh = meshFilter.sharedMesh;
    Vector3[] vertices = mesh.vertices;
    int[] triangles = mesh.triangles;

    // 绘制所有三角形的边
    for (int i = 0; i < triangles.Length; i += 3)
    {
      DrawTriangleLine(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
    }

    GL.End();
    GL.PopMatrix();
  }

  private void DrawTriangleLine(Vector3 a, Vector3 b, Vector3 c)
  {
    GL.Vertex(a);
    GL.Vertex(b);

    GL.Vertex(b);
    GL.Vertex(c);

    GL.Vertex(c);
    GL.Vertex(a);
  }

  #endregion
  // ==================================================
  #region DEBUG

  // 在Inspector中显示调试信息
  private void DEBUG_ShowInfo()
  {
    if (!isDebug) { return; }
    if (Application.isPlaying)
    {
      GUILayout.BeginArea(new Rect(10, 10, 300, 150));
      GUILayout.Label($"当前旋转速度: {currentRotationSpeed}");
      GUILayout.Label($"目标旋转速度: {targetRotationSpeed}");
      GUILayout.Label($"时间进度: {timeSinceLastDirectionChange:F1}/{directionChangeInterval}");
      GUILayout.Label($"进度百分比: {timeSinceLastDirectionChange / directionChangeInterval * 100:F1}%");
      GUILayout.EndArea();
    }
  }

  // 可视化调试（在Scene视图中显示）
  private void DEBUG_DrawGizmo()
  {
    if (!isDebug) { return; }
    if (Application.isPlaying)
    {
      // 绘制旋转轴指示器
      Gizmos.color = Color.red;
      Vector3 direction = currentRotationSpeed.normalized;
      Gizmos.DrawRay(transform.position, direction * 2f);

      // 绘制速度大小的球体
      Gizmos.color = Color.blue;
      float speed = currentRotationSpeed.magnitude / 100f;
      Gizmos.DrawWireSphere(transform.position, speed);
    }
  }

  #endregion
}