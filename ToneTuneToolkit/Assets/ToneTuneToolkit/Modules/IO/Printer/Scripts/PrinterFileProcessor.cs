using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace ToneTuneToolkit.IO.Printer
{
  public class PrinterFileProcessor : MonoBehaviour
  {
    private const string PrinterPath = "C:/Printer";
    private string CachePath = Application.streamingAssetsPath + "/PrinterCache";

    // ==================================================

    private void Start() => Init();
    private void OnDestroy() => UnInit();

    // ==================================================

    private void Init()
    {
      FolderCheck();
      StartCoroutine(nameof(PrintLoop));
    }

    private void UnInit()
    {
      StopCoroutine(nameof(PrintLoop));
    }

    // ==================================================

    private void FolderCheck()
    {
      if (!Directory.Exists(PrinterPath)) { Directory.CreateDirectory(PrinterPath); }
      if (!Directory.Exists(CachePath)) { Directory.CreateDirectory(CachePath); }
    }

    // ==================================================

    private IEnumerator PrintLoop()
    {
      yield return new WaitForSeconds(3f);
      while (true)
      {
        CheckFile();
        yield return new WaitForSeconds(3f);
      }
    }

    /// <summary>
    /// 检测是否存在文件
    /// </summary>
    private void CheckFile()
    {
      string[] files = Directory.GetFiles(PrinterPath); // 获取源文件夹中的所有文件
      if (files.Length == 0) { return; }
      foreach (string sourceFilePath in files)
      {
        ProcessFile(sourceFilePath);
      }
    }

    /// <summary>
    /// 处理文件
    /// </summary>
    /// <param name="sourceFilePath"></param>
    private void ProcessFile(string sourceFilePath)
    {
      try
      {
        if (!IsFileReady(sourceFilePath)) // 检查文件是否就绪（未被其他进程占用）
        {
          Debug.Log($"[FP] 文件被占用，跳过: {Path.GetFileName(sourceFilePath)}");
          return;
        }

        string fileName = @$"{Path.GetFileNameWithoutExtension(sourceFilePath)}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{Path.GetExtension(sourceFilePath)}";
        string destFilePath = Path.Combine(CachePath, fileName);

        // Debug.Log(fileName + "\n" + destFilePath);

        File.Move(sourceFilePath, destFilePath);
        Debug.Log($"[FP] 文件已移动: {fileName}");

        byte[] fileData = File.ReadAllBytes(destFilePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        texture.Apply();
        PrinterManager.Instance.Print(texture);
        Debug.Log($"[FP] 图片准备打印");

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh(); // 在编辑器中刷新AssetDatabase
#endif
      }
      catch (Exception e)
      {
        Debug.LogError($"[FP] 处理文件 {Path.GetFileName(sourceFilePath)} 时出错: {e.Message}");
      }
    }



    private bool IsFileReady(string filePath)
    {
      try
      {
        // 尝试打开文件，如果成功则文件就绪
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
          return true;
        }
      }
      catch (IOException)
      {
        return false; // 文件被占用或不可访问
      }
    }
  }
}