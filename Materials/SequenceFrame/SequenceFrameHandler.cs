using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceFrameHandler : MonoBehaviour
{
  [SerializeField] private List<Sprite> frames;
  private float fps = 12f;
  private Image image;
  private bool isAnimationPlaying = false;

  [SerializeField] private bool allowPlayOnStart = false;
  private bool allowLoop = true;

  // ==================================================

  private void Start() => Init();

  // ==================================================

  private void Init()
  {
    image = GetComponent<Image>();
    if (allowPlayOnStart)
    {
      SwitchAnimation(true);
    }
    return;
  }

  // ==================================================

  public void SwitchAnimation(bool value)
  {
    if (value)
    {
      if (isAnimationPlaying)
      {
        return;
      }
      isAnimationPlaying = true;
      StartCoroutine(nameof(AnimationAction));
    }
    else
    {
      if (!isAnimationPlaying)
      {
        return;
      }
      isAnimationPlaying = false;
      image.sprite = frames[0];
      StopCoroutine(nameof(AnimationAction));
    }
    return;
  }

  private IEnumerator AnimationAction()
  {
    while (allowLoop) // 注释则不循环
    {
      for (int i = 0; i < frames.Count; i++)
      {
        image.sprite = frames[i];
        yield return new WaitForSeconds(1f / fps);
      }
    }
  }
}