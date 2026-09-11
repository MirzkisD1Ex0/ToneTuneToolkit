/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.1
/// </summary>
using UnityEngine;
using Klak.Spout;

namespace ToneTuneToolkit.IO.Canon
{
  [RequireComponent(typeof(SpoutSender))]
  public class CanonLiveViewRenderer : MonoBehaviour
  {
    [Header("Spout 输出")]
    [Tooltip("Spout 发端名称，外部应用按此名接收")]
    public string spoutName = "CanonLiveView";
    [Tooltip("用于 Blit 的 SpoutResources；留空则自动从 Resources/CanonSpoutResources 加载，再退回 Shader.Find")]
    public SpoutResources resources;

    [Header("解码纹理")]
    [Tooltip("复用的解码目标纹理（自动创建）")]
    public Texture2D workingTexture;

    private SpoutSender _sender;
    private Session _session;

    public bool IsReady { get { return _sender != null && _sender.sourceTexture != null; } }

    private void OnEnable()
    {
      PrepareSender();
      // 缓存会话引用：直接订阅，避免在 OnDisable(场景拆除) 再访问 Instance 触发单例自动创建
      _session = Session.Instance;
      if (_session != null) { _session.OnLiveViewFrame += OnLiveViewFrame; }
    }

    private void OnDisable()
    {
      if (_session != null) { _session.OnLiveViewFrame -= OnLiveViewFrame; }
      _session = null;
    }

    private void PrepareSender()
    {
      _sender = GetComponent<SpoutSender>();
      if (_sender == null) { _sender = gameObject.AddComponent<SpoutSender>(); }

      _sender.spoutName = spoutName;
      _sender.captureMethod = CaptureMethod.Texture;
      _sender.sourceTexture = workingTexture;

      if (resources == null)
      {
        resources = Resources.Load<SpoutResources>("CanonSpoutResources");
      }
      if (resources != null)
      {
        _sender.SetResources(resources);
      }
      else
      {
        Debug.LogWarning("[CanonLiveView] 未找到 SpoutResources，尝试 Shader.Find 兜底。");
        SpoutResources runtime = ScriptableObject.CreateInstance<SpoutResources>();
        Shader blit = Shader.Find("Hidden/Klak/Spout/Blit");
        if (blit != null) { runtime.blitShader = blit; _sender.SetResources(runtime); }
      }
    }

    private void OnLiveViewFrame(byte[] jpeg)
    {
      if (jpeg == null || jpeg.Length == 0) { return; }

      if (workingTexture == null) { workingTexture = new Texture2D(2, 2); }

      if (ImageConversion.LoadImage(workingTexture, jpeg, true))
      {
        _sender.sourceTexture = workingTexture;
      }
    }
  }
}
