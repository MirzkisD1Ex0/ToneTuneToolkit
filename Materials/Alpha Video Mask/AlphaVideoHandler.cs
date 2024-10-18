using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AlphaVideoHandler : MonoBehaviour
{
  public static AlphaVideoHandler Instance;

  private VideoPlayer videoPlayer;

  // ==================================================

  private void Awake() => Instance = this;
  private void Start() => Init();
  private void Update() => ShortcutKey();

  // ==================================================

  private void Init()
  {
    videoPlayer = GetComponent<VideoPlayer>();
    StartCoroutine(nameof(InitAction));
    return;
  }

  private IEnumerator InitAction()
  {
    yield return new WaitForSeconds(1f); // 为了视频加载完毕而延迟
    videoPlayer.Play();
    yield return new WaitForSeconds(0.2f); // 显示擦除的短暂空白
    videoPlayer.Pause();
    yield break;
  }

  // ==================================================

  public void SwitchAlphaVideo(bool value)
  {
    if (value)
    {
      videoPlayer.Play();
    }
    else
    {
      videoPlayer.Pause();
    }
    return;
  }

  // ==================================================

  private void ShortcutKey()
  {
    if (Input.GetKeyDown(KeyCode.Q))
    {
      SwitchAlphaVideo(true);
    }
    if (Input.GetKeyDown(KeyCode.W))
    {
      SwitchAlphaVideo(false);
    }
    return;
  }
}