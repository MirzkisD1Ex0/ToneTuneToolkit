/// <summary>
/// Copyright (c) 2024 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.18
/// </summary>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToneTuneToolkit.Common;
using UnityEditor;
using UnityEngine;

namespace ToneTuneToolkit.Editor
{
  public class UpdateCopyrights : EditorWindow
  {
    private string author = "MirzkisD1Ex0";
    private string year = "2025";
    private string codeVersion = "1.4.20";

    private string displayString = "";

    // ==================================================

    [MenuItem(@"ToneTuneToolkit/Update Copyrights")]

    private static void Init()
    {
      UpdateCopyrights window = (UpdateCopyrights)GetWindow(typeof(UpdateCopyrights));
      window.Show();
      return;
    }

    private void OnGUI() => OnGUIUpdateCopyright();

    // ==================================================

    private void OnGUIUpdateCopyright()
    {
      GUILayout.Label("Copyright Info");
      author = EditorGUILayout.TextField("Author:", author);
      year = EditorGUILayout.TextField("Year:", year);
      codeVersion = EditorGUILayout.TextField("Code Version", codeVersion);

      GUILayout.Space(EditorStorage.GUI.Space);

      if (GUILayout.Button("Load All Selected File Info(s)"))
      {
        DsiplaySelectedFileInfos();
      }
      if (GUILayout.Button("Clear Info(s)", GUILayout.Width(EditorStorage.GUI.NarrowWidth)))
      {
        ClearFileInfos();
      }
      GUILayout.Label(displayString);

      GUILayout.Space(EditorStorage.GUI.Space);

      if (GUILayout.Button("Add Copyright Info to Above File(s)"))
      {
        ChangeContent();
      }
      return;
    }

    private List<string> scriptFilePaths;
    private List<string> scriptFileNames;

    private void ClearFileInfos()
    {
      scriptFilePaths.Clear();
      scriptFileNames.Clear();
      displayString = null;
      return;
    }

    // ==================================================

    private void DsiplaySelectedFileInfos()
    {
      ClearFileInfos();

      foreach (string guid in Selection.assetGUIDs)
      {
        string scriptFilePath = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(scriptFilePath) || !scriptFilePath.EndsWith(".cs"))
        {
          continue;
        }
        scriptFilePaths.Add(scriptFilePath);
        scriptFileNames.Add(Path.GetFileName(scriptFilePath));
      }

      if (scriptFilePaths.Count <= 0)
      {
        TTTDebug.LogWarning($"{nameof(UpdateCopyrights)} Can't find any file and its path.");
        return;
      }

      displayString = null;
      for (int i = 0; i < scriptFilePaths.Count; i++)
      {
        displayString += scriptFileNames[i] + "\n" + scriptFilePaths[i];

        if (i != scriptFilePaths.Count - 1) // 不是最后一个
        {
          displayString += "\n\n";
          continue;
        }
      }
      return;
    }

    /// <summary>
    /// 改变内容
    /// </summary>
    private void ChangeContent()
    {
      if (scriptFilePaths.Count <= 0)
      {
        return;
      }

      Debug.Log(scriptFilePaths.Count);

      string filePath = scriptFilePaths[0];
      List<string> fileContents = File.ReadAllLines(filePath).ToList();

      // 定位并所有Copyright
      int startIndex = -1;
      int endIndex = -1;

      for (int i = 0; i < 4; i++) // fileContents.Count
      {
        if (fileContents[i].Contains("<summary>"))
        {
          startIndex = i;
          Debug.Log("Find" + startIndex.ToString());
          break;
        }
      }

      for (int i = 0; i < 4; i++)
      {
        if (fileContents[i].Contains("</summary>"))
        {
          endIndex = i;
          Debug.Log("Find" + endIndex.ToString());
          break;
        }
      }

      // 删除已有的版权信息
      if (startIndex != -1 && endIndex != -1)
      {
        // 删除从开始位置到结束位置的所有行
        Debug.Log(endIndex - startIndex + 1);
        fileContents.RemoveRange(startIndex, endIndex - startIndex + 1);
        Debug.Log("Deleted");
      }

      #region 添加新的Copeyright
      fileContents.Insert(0, $"/// <summary>");
      fileContents.Insert(1, $"/// Copyright (c) {year} {author} All rights reserved.");
      fileContents.Insert(2, $"/// Code Version {codeVersion}");
      fileContents.Insert(3, $"/// </summary>");
      #endregion

      File.WriteAllLines(filePath, fileContents);
      ClearFileInfos();
      return;
    }
  }
}