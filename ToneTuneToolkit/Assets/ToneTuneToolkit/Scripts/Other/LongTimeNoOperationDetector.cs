/// <summary>
/// Copyright (c) 2024 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.01
/// </summary>

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ToneTuneToolkit.Other
{
  public class LongTimeNoOperationDetector : MonoBehaviour
  {
    public static LongTimeNoOperationDetector Instance;
    public UnityAction OnLongTimeNoOperation;

    private float timeInterval = 10f; // 检测时间间隔
    private float lastOperationTime; // 上次操作的时间
    private float nowTime; // 当前时间
    private float noOperationTime; // 无操作时间

    private bool isAllowCheck = false; // 是否允许无操作检测
    private bool beginCheck = false; // 是否无操作检测


    // ==================================================

    private void Awake()
    {
      Instance = this;
    }

    private void Start()
    {
      StartCoroutine("AutoCheck");
    }

    public void Update()
    {
      CheckOperate();
    }

    // ==================================================

    public void SwitchCheck(bool value)
    {
      isAllowCheck = value;
      if (!value)
      {
        lastOperationTime = 0;
        nowTime = 0;
        noOperationTime = 0;
      }
      else
      {
        lastOperationTime = Time.time;
        nowTime = Time.time;
        noOperationTime = 0;
      }
      return;
    }

    public void SetTimeInterval(float value)
    {
      if (value > 0)
      {
        timeInterval = value;
      }
      return;
    }

    /// <summary>
    /// 执行无操作监测
    /// </summary>
    private IEnumerator AutoCheck()
    {
      yield return new WaitUntil(() => isAllowCheck); // true之前一直等待
      lastOperationTime = Time.time; // 设置初始时间为当前
      while (true)
      {
        yield return new WaitUntil(() => !beginCheck && isAllowCheck); // 等到允许无操作监测时，且之前的无操作监测结束时
        // 开启新一轮无操作监测
        beginCheck = true; // 开启无操作监测
      }
    }

    /// <summary>
    /// 进行无操作监测
    /// </summary>
    private void CheckOperate()
    {
      if (!beginCheck)
      {
        return;
      }

      isAllowCheck = false; // 正在该轮操作监测中，停止开启新一轮操作监测
      nowTime = Time.time; // 当前时间

      // 如果有操作则更新上次操作时间为此时

#if UNITY_EDITOR
      if (Input.GetMouseButtonDown(0)) // 若点击鼠标左键/有操作，则更新触摸时间
      {
        lastOperationTime = nowTime;
      }
#elif UNITY_IOS || UNITY_ANDROID
      // 非编辑器环境下，触屏操作
      // Input.touchCount在pc端没用，只在移动端生效
      // Application.isMobilePlatform在pc和移动端都生效

      if (Input.touchCount > 0) // 有屏幕手指接触
      {
        lastOperationTime = nowTime; // 更新触摸时间
      }
#endif

      // 判断无操作时间是否达到指定时长，若达到指定时长无操作，则执行TakeOperate
      noOperationTime = Mathf.Abs(nowTime - lastOperationTime);
      if (noOperationTime > timeInterval)
      {
        lastOperationTime = nowTime; // 更新时间
        NoticeAll();
      }

      // 该轮操作监测结束，开启新一轮操作监测
      isAllowCheck = true;
      beginCheck = false;
      return;
    }

    private void NoticeAll()
    {
      if (OnLongTimeNoOperation != null)
      {
        OnLongTimeNoOperation();
      }
      return;
    }
  }
}