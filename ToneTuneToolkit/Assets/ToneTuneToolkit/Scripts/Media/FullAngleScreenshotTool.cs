/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using System.Collections;
using UnityEngine;
using System.IO;

namespace ToneTuneToolkit.Media
{
  /// <summary>
  /// 全角度截图工具
  /// 等待修复
  /// </summary>
  [RequireComponent(typeof(ScreenshotMaster))]
  public class FullAngleScreenshotTool : MonoBehaviour
  {
    public GameObject TargetGO; // 目标
    public int ShotTime = 4; // 拍摄次数

    private Camera ScreenshotCamera;
    private string fileSavePath = Application.streamingAssetsPath + "/";

    // ==================================================

    private void Start()
    {
      // ScreenshotCamera = ScreenshotMaster.Instance.ScreenshotCamera;
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Q))
      {
        StartCoroutine("ScreenshotAction");
      }
    }

    private void OnApplicationQuit()
    {
      StopAllCoroutines();
    }

    // ==================================================

    private IEnumerator ScreenshotAction()
    {
      // 判断有多少文件夹在目录中
      DirectoryInfo di = new DirectoryInfo(fileSavePath);
      FileInfo[] files = di.GetFiles("*", SearchOption.TopDirectoryOnly);
      int folderIndex = files.Length;
      string currentPath = fileSavePath + string.Format("{0:d4}", folderIndex) + "/";

      // ScreenshotCamera.transform.LookAt(TargetGO.transform); // 好像不太需要盯着看

      for (int i = 0; i < ShotTime; i++)
      {
        TargetGO.transform.rotation = Quaternion.Euler(0, 360 / ShotTime * i, 0);
        ScreenshotCamera.clearFlags = CameraClearFlags.SolidColor;
        ScreenshotCamera.clearFlags = CameraClearFlags.Nothing;
        yield return new WaitForEndOfFrame();
        // ScreenshotMaster.Instance.SaveRenderTexture(currentPath, string.Format("{0:d4}", i) + ".png");
      }
      Debug.Log($"[FullAngleScreenshotTool] {ShotTime} shots complete...[Done]");
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#endif
      Application.Quit();
      yield break;
    }
  }
}
