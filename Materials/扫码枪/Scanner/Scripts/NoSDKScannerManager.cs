using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class NoSDKScannerManager : MonoBehaviour
{
  public static NoSDKScannerManager Instance { get; private set; }
  public static UnityAction<string> OnInputFinished;

  private const float SCANTIME = 2f;

  private static StringBuilder inputBuffer = new StringBuilder();
  private static bool allowInput = false;
  private static bool hasPublish = false;

  // ==================================================

  private void Awake() => Instance = this;
  private void Start() => Init();
  private void Update() => InputAction();
  private void OnDestroy() => UnInit();

  // ==================================================

  private void Init()
  {
    return;
  }

  private void UnInit()
  {
    return;
  }

  private static void Reset()
  {
    inputBuffer.Clear();
    allowInput = false;
    hasPublish = false;
    return;
  }

  // ==================================================

  [ContextMenu(nameof(AllowScanner), true)]
  public static void AllowScanner(bool value)
  {
    if (value)
    {
      allowInput = value;
    }
    else
    {
      Reset();
    }
    return;
  }

  public string GetInput()
  {
    if (inputBuffer.Length > 0)
    {
      string input = inputBuffer.ToString();

      Debug.Log(@$"[NoSDKSM] {input}");
      if (OnInputFinished != null)
      {
        OnInputFinished(input);
      }

      Reset();
      return input;
    }
    return null;
  }



  private void InputAction()
  {
    if (!allowInput)
    {
      return;
    }

    if (inputBuffer.Length > 0 && !hasPublish)
    {
      hasPublish = true;
      StartCoroutine(nameof(DelayGetInput));
    }

    // 处理常规字符输入
    foreach (char c in Input.inputString)
    {
      // 只接受可打印ASCII字符
      if (c >= ' ' && c <= '~')
      {
        inputBuffer.Append(c);
        // Debug.Log("当前输入: " + inputBuffer.ToString());
      }
    }

    // // 处理退格键
    // if (Input.GetKeyDown(KeyCode.Backspace) && inputBuffer.Length > 0)
    // {
    //   inputBuffer.Length--;
    //   Debug.Log("当前输入: " + inputBuffer.ToString());
    // }
    return;
  }

  private IEnumerator DelayGetInput()
  {
    yield return new WaitForSeconds(SCANTIME);
    GetInput();
  }

  // ==================================================
  #region Tool

  public static string AnalyzeURL(string value)
  {
    string pattern = @"(https://project\.studiocapsule\.cn/nike_adt_2025/index\.html#/\?user_code=[A-Z]\d+)";
    Match match = Regex.Match(value, pattern);
    if (match.Success)
    {
      return match.Groups[1].Value;
    }
    else
    {
      return null;
    }

  }

  #endregion
}