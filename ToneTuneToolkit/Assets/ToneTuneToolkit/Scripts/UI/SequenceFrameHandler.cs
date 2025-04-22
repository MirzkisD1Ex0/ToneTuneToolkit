using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceFrameHandler : MonoBehaviour
{
  [SerializeField] private List<Sprite> frames;
  private float fps = 15f;
  private Image image;
  private bool isAnimationPlaying = false;

  [SerializeField] private bool playOnStart = false;
  public bool reverse = false;

  private bool allowLoop = true;

  // ==================================================

  private void Start() => Init();

  // ==================================================

  private void Init()
  {
    image = GetComponent<Image>();
    if (playOnStart)
    {
      Play();
    }
    return;
  }

  // ==================================================

  public void Reset()
  {
    if (!reverse)
    {
      image.sprite = frames[0];
    }
    else
    {
      image.sprite = frames[frames.Count - 1];
    }
    return;
  }

  public void Play()
  {
    if (isAnimationPlaying)
    {
      return;
    }
    isAnimationPlaying = true;
    StartCoroutine(nameof(AnimationAction));
    return;
  }

  public void Stop()
  {
    if (!isAnimationPlaying)
    {
      return;
    }
    isAnimationPlaying = false;
    StopCoroutine(nameof(AnimationAction));
    return;
  }

  private IEnumerator AnimationAction()
  {
    // while (allowLoop) // 注释则不循环
    // {
    if (!reverse)
    {
      for (int i = 0; i < frames.Count; i++)
      {
        yield return new WaitForSeconds(1f / fps);
        image.sprite = frames[i];
      }
    }
    else
    {
      for (int i = frames.Count - 1; i > 0; i--)
      {
        yield return new WaitForSeconds(1f / fps);
        image.sprite = frames[i];
      }
    }
    // }
  }
}