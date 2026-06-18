/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using ToneTuneToolkit.Common;

public class ARAnchorSetter : SingletonMaster<ARAnchorSetter>
{
  [SerializeField] private GameObject AnchorSetterGO;
  private ARAnchorManager arAnchorManager;

  private Dictionary<TrackableId, ARAnchor> m_ARAnchorLoad = new Dictionary<TrackableId, ARAnchor>();
  private bool isSessionReady = false;

  public bool isSaveARAnchorData = false;

  // ==================================================

  private void Awake()
  {
    arAnchorManager = GetComponent<ARAnchorManager>();
    AnchorSetterGO.gameObject.SetActive(false);
  }

  private void Start() => Init();

  private void OnEnable()
  {
    ARSession.stateChanged += WhenARSessionStateChanged;
  }

  private void OnDisable()
  {
    ARSession.stateChanged -= WhenARSessionStateChanged;
  }

  // ==================================================

  private void Init()
  {
    AnchorSetterGO.gameObject.SetActive(false); // 初始化时先隐藏生成点
    arAnchorManager.trackablesChanged.AddListener(AnchorTrackablesChanged);
  }

  // ==================================================

  private void WhenARSessionStateChanged(ARSessionStateChangedEventArgs args)
  {
    if (args.state == ARSessionState.SessionTracking) // AR系统准备就绪时加载锚点
    {
      Debug.Log("[PSAM] 准备就绪，加载锚点");
      isSessionReady = true;
      LoadSavedAnchors();
    }
  }

  /// <summary>
  /// 加载锚点信息
  /// </summary>
  private void LoadSavedAnchors()
  {
    if (PlayerPrefs.HasKey("AnchorPosition"))
    {
      // 有数据时加载
      AnchorSetterGO.gameObject.SetActive(false);
      isSaveARAnchorData = true;
    }
    else
    {
      // 没有数据时显示放置UI
      AnchorSetterGO.gameObject.SetActive(true);
      AnchorSetterGO.GetComponent<XRGrabInteractable>().selectExited.AddListener(OnGrabObj);
      isSaveARAnchorData = false;
    }
  }

  /// <summary>
  /// 重置AR锚点
  /// </summary>
  public void ResetPolySpatialAnchors()
  {
    Debug.Log("[PSAM] 重置AR锚点");
    DeleteAllAnchors();
    AnchorSetterGO.gameObject.SetActive(true);
    AnchorSetterGO.transform.position = new Vector3(0f, 0.85f, 5f);
    AnchorSetterGO.GetComponent<XRGrabInteractable>().selectExited.AddListener(OnGrabObj);
  }



  private void AnchorTrackablesChanged(ARTrackablesChangedEventArgs<ARAnchor> Trackable)
  {
    foreach (ARAnchor anchor in Trackable.added)
    {
      var id = anchor.trackableId;
      if (!m_ARAnchorLoad.ContainsKey(id))
      {
        m_ARAnchorLoad.Add(id, anchor);
      }
    }

    foreach (var anchor in Trackable.removed)
    {
      var id = anchor.Key;
      if (m_ARAnchorLoad.ContainsKey(id))
      {
        m_ARAnchorLoad.Remove(id);
      }
    }
  }

  private void OnGrabObj(SelectExitEventArgs args)
  {
    isSaveARAnchorData = false;
    DeleteAllAnchors();
    CreateARSpatalAnchor();
  }

  private async void CreateAnchorAtPose(Pose pose)
  {
    var result = await arAnchorManager.TryAddAnchorAsync(pose);
    if (result.status.IsSuccess())
    {
      var anchor = result.value;
      // 保存位姿数据
      PlayerPrefs.SetString("AnchorPosition", JsonUtility.ToJson(pose));
      PlayerPrefs.Save();
    }
    else
    {
      Debug.LogError($"[PSAM] 创建锚点失败: {result.status}");
    }
  }

  // ==================================================

  private void CreateARSpatalAnchor()
  {
    // 使用当前位置创建新锚点
    Pose newPose = new Pose(AnchorSetterGO.transform.position, AnchorSetterGO.transform.rotation);
    CreateAnchorAtPose(newPose);
  }

  /// <summary>
  /// 删除所有锚点
  /// </summary>
  private void DeleteAllAnchors()
  {
    if (m_ARAnchorLoad.Count > 0)
    {
      // 清理所有锚点
      foreach (var pair in m_ARAnchorLoad)
      {
        arAnchorManager.TryRemoveAnchor(pair.Value);
        Destroy(pair.Value.gameObject);
      }
      m_ARAnchorLoad.Clear();

      // 删除保存数据
      PlayerPrefs.DeleteKey("AnchorPosition");
      PlayerPrefs.Save();
    }
  }
}
