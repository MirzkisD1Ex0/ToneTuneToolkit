/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.1
/// </summary>

using UnityEngine;
using Klak.Spout;

namespace ToneTuneToolkit.IO.Canon
{
  /// <summary>
  /// LiveView投射器
  /// </summary>
  [RequireComponent(typeof(SpoutSender))]
  public class CanonLiveViewRenderer : MonoBehaviour
  {
    [Header("Spout 输出")]

    [SerializeField] private string spoutName = "CanonLiveView"; // Spout 发端名称，外部应用按此名接收
    [SerializeField] private SpoutResources resources; // 用于 Blit 的 SpoutResources；留空则自动从 Resources/CanonSpoutResources 加载，再退回 Shader.Find



    [Header("解码纹理")]
    [SerializeField] private Texture2D workingTexture; // 复用的解码目标纹理（自动创建）

    private SpoutSender _spoutSender;
    private CanonSession _canonSession;

    public bool IsReady { get { return _spoutSender != null && _spoutSender.sourceTexture != null; } }

    // ============================================================

    private void OnEnable() => Init();
    private void OnDisable() => UnInit();

    // ============================================================

    private void Init()
    {
      PrepareSender();
      // 缓存会话引用：直接订阅，避免在 OnDisable(场景拆除) 再访问 Instance 触发单例自动创建
      _canonSession = CanonSession.Instance;
      if (_canonSession != null) { _canonSession.OnLiveViewFrame += OnLiveViewFrame; }
    }

    private void UnInit()
    {
      if (_canonSession != null) { _canonSession.OnLiveViewFrame -= OnLiveViewFrame; }
      _canonSession = null;
    }

    // ============================================================

    private void PrepareSender()
    {
      _spoutSender = GetComponent<SpoutSender>();
      if (_spoutSender == null) { _spoutSender = gameObject.AddComponent<SpoutSender>(); }

      _spoutSender.spoutName = spoutName;
      _spoutSender.captureMethod = CaptureMethod.Texture;
      _spoutSender.sourceTexture = workingTexture;

      if (resources == null) { resources = Resources.Load<SpoutResources>("CanonSpoutResources"); }

      if (resources != null)
      {
        _spoutSender.SetResources(resources);
      }
      else
      {
        Debug.LogWarning("[Canon LiveView Renderer] 未找到 SpoutResources，尝试 Shader.Find 兜底。");
        SpoutResources runtime = ScriptableObject.CreateInstance<SpoutResources>();
        Shader blit = Shader.Find("Hidden/Klak/Spout/Blit");
        if (blit != null) { runtime.blitShader = blit; _spoutSender.SetResources(runtime); }
      }
    }

    private void OnLiveViewFrame(byte[] jpeg)
    {
      if (jpeg == null || jpeg.Length == 0) { return; }

      if (workingTexture == null) { workingTexture = new Texture2D(2, 2); }

      if (ImageConversion.LoadImage(workingTexture, jpeg, true))
      {
        _spoutSender.sourceTexture = workingTexture;
      }
    }
  }
}