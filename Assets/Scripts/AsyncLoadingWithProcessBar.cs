using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ToneTuneToolkit
{
    /// <summary>
    /// 带进度条的异步场景加载
    /// 需要Slider和Text对象
    /// 方法 SceneLoading.Instance.LoadingScene(01);
    /// </summary>
    public class AsyncLoadingWithProcessBar : MonoBehaviour
    {
        public Slider LoadingSlider;
        public Text LoadingText;

        public static AsyncLoadingWithProcessBar Instance; // 懒人单例

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (LoadingSlider == null || LoadingText == null)
            {
                TTTDebug.Warning(this.name + "组件缺失");
                this.enabled = false;
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