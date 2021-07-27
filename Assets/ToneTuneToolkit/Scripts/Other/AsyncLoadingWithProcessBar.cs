using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Other
{
  /// <summary>
  /// OK
  /// 带进度条的异步场景加载
  /// 需要Slider和Text对象
  /// SceneLoading.Instance.LoadingScene(01);
  /// </summary>
  public class AsyncLoadingWithProcessBar : MonoBehaviour
  {
    public static AsyncLoadingWithProcessBar Instance; // 懒人单例


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
        TipTools.Notice(this.name + "组件缺失");
        this.enabled = false;
        return;
      }
    }

    /// <summary>
    /// 对外接口
    /// </summary>
    /// <param name="sceneIndex"></param>
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
      asyncOperation.allowSceneActivation = true; // false会卡住最后10%的进度
    }
  }
}