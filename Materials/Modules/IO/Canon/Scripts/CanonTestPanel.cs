/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.1
/// </summary>
using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit.IO.Canon
{
  public class CanonTestPanel : MonoBehaviour
  {
    [Header("状态日志")]
    public int maxLogs = 8;

    private readonly Queue<string> _logs = new Queue<string>();
    private Vector2 _scroll;
    private Session _session;

    private void OnEnable()
    {
      Bind();
    }

    private void OnDisable()
    {
      Unbind();
    }

    private void Bind()
    {
      _session = Session.Instance;
      if (_session == null) { return; }
      _session.OnCameraConnected += OnCameraConnected;
      _session.OnCameraNotConnected += OnCameraNotConnected;
      _session.OnCameraShutdown += OnCameraShutdown;
      _session.OnLiveViewStarted += OnLiveViewStarted;
      _session.OnLiveViewStopped += OnLiveViewStopped;
      _session.OnPhotoSaved += OnPhotoSaved;
      _session.OnError += OnError;
    }

    private void Unbind()
    {
      if (_session == null) { return; }
      _session.OnCameraConnected -= OnCameraConnected;
      _session.OnCameraNotConnected -= OnCameraNotConnected;
      _session.OnCameraShutdown -= OnCameraShutdown;
      _session.OnLiveViewStarted -= OnLiveViewStarted;
      _session.OnLiveViewStopped -= OnLiveViewStopped;
      _session.OnPhotoSaved -= OnPhotoSaved;
      _session.OnError -= OnError;
      _session = null;
    }

    private void Push(string line)
    {
      _logs.Enqueue("[" + System.DateTime.Now.ToString("HH:mm:ss") + "] " + line);
      while (_logs.Count > maxLogs) { _logs.Dequeue(); }
    }

    private void OnGUI()
    {
      GUILayout.BeginArea(new Rect(8, 8, 300, Screen.height - 16));

      GUILayout.BeginVertical("box");
      GUILayout.Label("<b>Canon EDSDK 测试</b>", RichText());

      Session s = Session.Instance;
      GUILayout.Label("相机: " + (s != null ? Fmt(s.IsCameraConnected) : "无会话"));
      GUILayout.Label("LiveView: " + (s != null ? Fmt(s.IsLiveViewActive) : "-"));
      GUILayout.Label("型号: " + (s != null ? s.DeviceDescription : "-"));

      GUILayout.Space(4);
      if (GUILayout.Button("连接相机")) { s?.Open(); }
      GUILayout.BeginHorizontal();
      if (GUILayout.Button(s != null && s.IsLiveViewActive ? "停止 LiveView" : "开启 LiveView"))
      {
        if (s == null) { return; }
        if (s.IsLiveViewActive) { s.StopLiveView(); } else { s.StartLiveView(); }
      }
      if (GUILayout.Button("拍照")) { s?.TakePhoto(); }
      GUILayout.EndHorizontal();
      if (GUILayout.Button("断开")) { s?.Close(); }
      GUILayout.EndVertical();

      GUILayout.Space(6);
      GUILayout.Label("<b>回调日志</b>", RichText());
      _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(200));
      foreach (string log in _logs) { GUILayout.Label(log); }
      GUILayout.EndScrollView();

      GUILayout.EndArea();
    }

    private static GUIStyle _rich;
    private static GUIStyle RichText()
    {
      if (_rich == null) { _rich = new GUIStyle(GUI.skin.label) { richText = true }; }
      return _rich;
    }

    private static string Fmt(bool b) { return b ? "<color=#00ff00>on</color>" : "<color=#ff5555>off</color>"; }

    private void OnCameraConnected(string desc) { Push("相机已连接: " + desc); }
    private void OnCameraNotConnected() { Push("未检测到相机"); }
    private void OnCameraShutdown() { Push("相机断开/关机"); }
    private void OnLiveViewStarted() { Push("LiveView 已开启"); }
    private void OnLiveViewStopped() { Push("LiveView 已停止"); }
    private void OnPhotoSaved(string path) { Push("拍照保存: " + path); }
    private void OnError(string msg) { Push("<color=#ff5555>错误: " + msg + "</color>"); }
  }
}
