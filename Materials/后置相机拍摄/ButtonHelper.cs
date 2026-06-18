/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHelper : MonoBehaviour
{
  private Button button;

  private void Start()
  {
    button = GetComponent<Button>();
  }

  public void Hide()
  {
    button.interactable = false;
    StartCoroutine(HideAction());
    return;
  }

  private IEnumerator HideAction()
  {
    yield return new WaitForSeconds(3f);
    button.interactable = true;
    yield break;
  }
}
