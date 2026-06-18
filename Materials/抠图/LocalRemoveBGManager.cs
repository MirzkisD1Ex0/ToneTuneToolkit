/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System;
using System.Collections;
using System.Globalization;
using ToneTuneToolkit.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// 本地抠图
/// </summary>
public class LocalRemoveBGManager : SingletonMaster<LocalRemoveBGManager>
{
  [Header("Server")]
  [Tooltip("Background-removal service base URL.\n" + "Local dev (Editor / Standalone on same machine): http://localhost:8000\n" + "Android / iOS device on same LAN:                http://192.168.x.x:8000")]
  public string serverUrl = "http://localhost:8000";
  public BGRemovalModel modelName = BGRemovalModel.BirefNetPortrait;
  [Min(1)] public int timeoutSeconds = 60;

  [Header("Diagnostics (read-only)")]
  [SerializeField] private string lastStatus = "Idle";
  [SerializeField] private float lastProcessingTimeSec = -1f;
  [SerializeField] private string lastSelectedModel = "";
  [SerializeField] private string lastError = "";

  // ==================================================

  public Texture2D t2dSource;
  public Texture2D t2dResult;
  public static UnityAction<Texture2D> OnRemoveBGFinished;
  public static UnityAction<string> OnRemoveBGError;

  // ==================================================

  private void OnDestroy() => UnInit();

  // ==================================================

  private void UnInit()
  {
    if (t2dSource != null)
    {
      Destroy(t2dSource);
      t2dSource = null;
    }
    if (t2dResult != null)
    {
      Destroy(t2dResult);
      t2dResult = null;
    }
  }

  // ==================================================

  public void RemoveBackground(Texture2D t2d)
  {
    t2dSource = t2d;
    RemoveBackground();
  }
  public void RemoveBackground() => StartCoroutine(RemoveBackgroundRoutine());
  private IEnumerator RemoveBackgroundRoutine()
  {
    if (t2dSource == null)
    {
      Fail("t2dUpload is null. Please assign a source texture first.");
      yield break;
    }
    if (string.IsNullOrWhiteSpace(serverUrl))
    {
      Fail("serverUrl is empty.");
      yield break;
    }

    byte[] uploadBytes;
    try
    {
      uploadBytes = t2dSource.EncodeToPNG();
    }
    catch (Exception e)
    {
      Fail("EncodeToPNG failed: " + e.Message + "\nMake sure the texture has Read/Write Enabled in its import settings.");
      yield break;
    }
    if (uploadBytes == null || uploadBytes.Length == 0)
    {
      Fail("EncodeToPNG produced empty bytes. Is the texture readable?");
      yield break;
    }

    WWWForm form = new WWWForm();
    form.AddBinaryData("file", uploadBytes, "upload.png", "image/png");

    string modelStr = GetModelString(modelName);
    string url = serverUrl.TrimEnd('/') + "/remove-bg" + "?model=" + UnityWebRequest.EscapeURL(modelStr);

    lastStatus = "Uploading and processing...";
    lastError = "";

    UnityWebRequest req = UnityWebRequest.Post(url, form);
    req.downloadHandler = new DownloadHandlerBuffer();
    req.timeout = timeoutSeconds;

    yield return req.SendWebRequest();

    if (req.result != UnityWebRequest.Result.Success)
    {
      string body = req.downloadHandler != null ? req.downloadHandler.text : "";
      Fail($"HTTP {req.responseCode} {req.error}\n{body}");
      req.Dispose();
      yield break;
    }

    byte[] resultPng = req.downloadHandler.data;
    if (resultPng == null || resultPng.Length == 0)
    {
      Fail("Server returned an empty body.");
      req.Dispose();
      yield break;
    }

    Texture2D newTex = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false)
    {
      name = "result",
      filterMode = FilterMode.Bilinear,
      wrapMode = TextureWrapMode.Clamp
    };
    bool loaded = newTex.LoadImage(resultPng, markNonReadable: false);
    if (!loaded)
    {
      Destroy(newTex);
      Fail("LoadImage failed: response is not a valid PNG.");
      req.Dispose();
      yield break;
    }

    if (t2dResult != null)
    {
      Destroy(t2dResult);
    }
    t2dResult = newTex;

    string procTime = req.GetResponseHeader("X-Processing-Time-Seconds");
    string usedModel = req.GetResponseHeader("X-Selected-Model");
    if (!string.IsNullOrEmpty(procTime) &&
        float.TryParse(procTime, NumberStyles.Float, CultureInfo.InvariantCulture, out float t))
    {
      lastProcessingTimeSec = t;
    }
    else
    {
      lastProcessingTimeSec = -1f;
    }
    lastSelectedModel = string.IsNullOrEmpty(usedModel) ? modelStr : usedModel;
    lastStatus = $"OK | {resultPng.Length / 1024} KB | {lastProcessingTimeSec:F3}s | model={lastSelectedModel}";

    Debug.Log("[LocalBGRemovalManager] " + lastStatus);

    req.Dispose();

    try { OnRemoveBGFinished?.Invoke(t2dResult); }
    catch (Exception ex) { Debug.LogException(ex); }
  }

  /// <summary>
  /// 获取模型名字符串
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  private static string GetModelString(BGRemovalModel model)
  {
    switch (model)
    {
      default: return "birefnet-portrait";
      case BGRemovalModel.BirefNetPortrait: return "birefnet-portrait";
      case BGRemovalModel.BirefNetGeneral: return "birefnet-general";
      case BGRemovalModel.U2NetHumanSeg: return "u2net_human_seg";
      case BGRemovalModel.U2Net: return "u2net";
      case BGRemovalModel.IsNetGeneralUse: return "isnet-general-use";
      case BGRemovalModel.Silueta: return "silueta";
    }
  }

  // ==================================================

  private void Fail(string message)
  {
    lastStatus = "ERROR";
    lastError = message;
    Debug.LogError("[LocalBGRemovalManager] " + message);
    try { OnRemoveBGError?.Invoke(message); }
    catch (Exception ex) { Debug.LogException(ex); }
  }

  // ==================================================

  public enum BGRemovalModel
  {
    BirefNetPortrait,
    BirefNetGeneral,
    U2NetHumanSeg,
    U2Net,
    IsNetGeneralUse,
    Silueta
  }
}
