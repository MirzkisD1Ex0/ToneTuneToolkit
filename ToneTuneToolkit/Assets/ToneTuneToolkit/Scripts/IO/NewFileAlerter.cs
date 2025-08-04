/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;

/// <summary>
/// 文件轮询查找功能
/// 你是不是在xxx文件夹下又塞了一个模型?
/// ↓参考组率先获取所有文件名
/// ↓对照组循环获取文件名
/// ↓对比数量
/// ↓将对照组多出来的末位文件名传出
/// </summary>
namespace ToneTuneToolkit.IO
{
  public class NewFileAlerter : MonoBehaviour
  {
    public static UnityAction<string> OnNewFileDetected;

    private const float detectSpaceTime = 10f; // 间隔多久轮询一次文件数量
    private const string folderPath = @"C:/SandTableMesh/"; // 路径
    private const string fileSuffix = ".obj"; // 文件后缀名

    [SerializeField] private List<string> LastFileList; // 参考组
    [SerializeField] private List<string> NewFileList; // 对照组

    // ==============================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==============================

    private void Init()
    {
      LastFileList = FileCapturer.GetFileNames2List(folderPath, fileSuffix);
      StartCoroutine(nameof(FileDetectCircle));
    }

    private void UnInit()
    {
      StopCoroutine(nameof(FileDetectCircle));
    }

    // ==============================

    /// <summary>
    /// 文件数量检测循环
    /// </summary>
    /// <returns></returns>
    private IEnumerator FileDetectCircle()
    {
      while (true)
      {
        NewFileList = FileCapturer.GetFileNames2List(folderPath, fileSuffix);

        if (NewFileList.Count > LastFileList.Count) // 对比数量判断是否有新文件传入
        {
          string meshPath = Path.Combine(folderPath, NewFileList[NewFileList.Count - 1]);
          Debug.Log("<color=green>[STEP01]</color> New .obj detected : <" + meshPath + ">.");
          OnNewFileDetected?.Invoke(meshPath);
          LastFileList = new List<string>(NewFileList);
        }

        yield return new WaitForSeconds(detectSpaceTime);
      }
    }
  }
}
