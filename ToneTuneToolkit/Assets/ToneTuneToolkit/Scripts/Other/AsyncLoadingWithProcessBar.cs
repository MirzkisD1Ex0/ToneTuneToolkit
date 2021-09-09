/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Other
{
  /// <summary>
  /// 带进度条的异步场景加载
  ///
  /// 需要Slider和Text对象
  /// SceneLoading.Instance.LoadingScene(01);
  /// </summary>
  public class AsyncLoadingWithProcessBar : MonoBehaviour
  {
    public static AsyncLoadingWithProcessBar Instance;

    public Slider LoadingSlider;
    public Text LoadingText;

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      if (!LoadingSlider || !LoadingText)
      {
        TipTools.Error("[AsyncLoadingWithProcessBar] Cant find nessary component.");
        enabled = false;
        return;
      }
    }

    /// <summary>
    /// 对外接口
    /// </summary>
    /// <param name="sceneIndex">场景编号</param>
    public void LoadingScene(int sceneIndex)
    {
      StartCoroutine(LoadingProcess(sceneIndex));
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <returns></returns>
    private IEnumerator LoadingProcess(int sceneIndex)
    {
      float index = 0;
      AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
      asyncOperation.allowSceneActivation = false;
      while (index <= 100)
      {
        index++;
        LoadingSlider.value = index / 100;
        yield return new WaitForEndOfFrame();
        LoadingText.text = index.ToString() + "%";
      }
      asyncOperation.allowSceneActivation = true; // 若为false会卡住最后10%的进度
    }
  }
}