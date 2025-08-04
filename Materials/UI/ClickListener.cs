using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClickListener : MonoBehaviour
{
  public static ClickListener Instance;

  // ==================================================

  private void Awake() => Instance = this;
  private void Update() => DetectOperation();

  // ==================================================
  #region 检测点击

  private void DetectOperation()
  {
    if (Input.GetMouseButtonDown(0))
    {
      if (EventSystem.current.IsPointerOverGameObject())
      {
        SwitchResetSequence(false);
        SwitchResetSequence(true);
      }
    }
    return;
  }

  #endregion
  // ==================================================
  #region 开始暂停流程

  public void SwitchResetSequence(bool value)
  {
    if (value)
    {
      StartCoroutine(nameof(ResetStateAction));
    }
    else
    {
      StopCoroutine(nameof(ResetStateAction));
    }
    return;
  }

  #endregion
  // ==================================================
  #region 自动重置流程

  private IEnumerator ResetStateAction()
  {
    yield return new WaitForSeconds(60f);
    SceneManager.LoadScene(0);
    yield break;
  }

  #endregion
}