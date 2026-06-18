using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
  [SerializeField] private CanvasGroup cgMainTitle;

  // ==================================================

  private void Start() => Init();
  private void OnDestroy() => UnInit();

  // ==================================================

  private void Init()
  {
    cgMainTitle = GetComponent<CanvasGroup>();
  }

  private void UnInit()
  {

  }

  // ==================================================
  #region Function

  public void SwitchCanvas(bool value)
  {
    if (!cgMainTitle) { return; }

    if (value) { cgMainTitle.alpha = 1; }
    else { cgMainTitle.alpha = 0; }
  }

  #endregion
}