/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ToneTuneToolkit.Common;
using UnityEditor;
using UnityEngine;

namespace ToneTuneToolkit.Editor
{
  public class UpdateCopyrights : EditorWindow
  {
    private string author = "MirzkisD1Ex0";
    private string year = "2025";
    private string codeVersion = "1.5.2";

    private string displayString = "";
    private Vector2 displayStringScrollPosition = Vector2.zero;

    private List<string> scriptFilePaths = new List<string>();
    private List<string> scriptFileNames = new List<string>();
    private List<string> scriptFileGroupNames = new List<string>(); // 与上面两个列表一一对应：Materials / Modules / Scripts

    // ==================================================
    // 固定扫描路径（主工作目录与 Unity 工程目录的三个目标位置）

    private const string MaterialsGroup = "Materials";
    private const string ModulesGroup = "Modules";
    private const string ScriptsGroup = "Scripts";

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

      if (GUILayout.Button("Scan All .cs in Fixed Paths"))
      {
        ScanFixedPaths();
      }
      if (GUILayout.Button("Load All Selected File Info(s)"))
      {
        DisplaySelectedFileInfos();
      }
      if (GUILayout.Button("Clear Info(s)", GUILayout.Width(EditorStorage.GUI.NarrowWidth)))
      {
        ClearFileInfos();
      }

      GUILayout.BeginVertical(GUILayout.Height(EditorStorage.GUI.Height));
      displayStringScrollPosition = GUILayout.BeginScrollView(displayStringScrollPosition);
      GUILayout.TextArea(displayString);
      GUILayout.EndScrollView();
      GUILayout.EndVertical();

      GUILayout.Space(EditorStorage.GUI.Space);

      if (GUILayout.Button("Add or Update Copyright Info to Above File(s)"))
      {
        ChangeContent();
      }
      return;
    }

    // ==================================================

    private void ClearFileInfos()
    {
      scriptFilePaths.Clear();
      scriptFileNames.Clear();
      scriptFileGroupNames.Clear();
      displayString = null;
      return;
    }

    /// <summary>
    /// 读取 Project 窗口中当前选中的 .cs 文件
    /// </summary>
    private void DisplaySelectedFileInfos()
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
        scriptFileGroupNames.Add("Selection");
      }

      if (scriptFilePaths.Count <= 0)
      {
        TTTDebug.LogWarning($"{nameof(UpdateCopyrights)} Can't find any file and its path.");
        return;
      }

      displayString = "[" + "Selection" + "]\n" + string.Join("\n", scriptFileNames);
      return;
    }

    /// <summary>
    /// 扫描三个固定目录下的全部 .cs 文件
    /// </summary>
    private void ScanFixedPaths()
    {
      ClearFileInfos();

      // 三个固定目录：(分组名, 绝对路径)
      List<KeyValuePair<string, string>> targets = ResolveTargetDirectories();
      HashSet<string> seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      StringWriter groupedText = new StringWriter();

      foreach (KeyValuePair<string, string> target in targets)
      {
        string groupName = target.Key;
        string dirPath = target.Value;

        groupedText.WriteLine($"[{groupName}]");

        if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))
        {
          groupedText.WriteLine($"  (skipped: directory not found -> {dirPath})");
          groupedText.WriteLine();
          continue;
        }

        string[] foundFiles = Directory.GetFiles(dirPath, "*.cs", SearchOption.AllDirectories);
        Array.Sort(foundFiles, StringComparer.OrdinalIgnoreCase);

        int groupCount = 0;
        foreach (string fullPath in foundFiles)
        {
          // 跳过 Unity 自动生成的 .meta 等
          if (!fullPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
          {
            continue;
          }

          string normalized = fullPath.Replace('\\', '/');
          if (!seenPaths.Add(normalized))
          {
            continue; // 防止不同路径间重复
          }

          scriptFilePaths.Add(fullPath);
          scriptFileNames.Add(Path.GetFileName(fullPath));
          scriptFileGroupNames.Add(groupName);

          string relative = normalized.Substring(dirPath.Replace('\\', '/').Length).TrimStart('/');
          groupedText.WriteLine($"  {relative}");
          groupCount++;
        }

        groupedText.WriteLine($"  ({groupCount} file(s))");
        groupedText.WriteLine();
      }

      if (scriptFilePaths.Count <= 0)
      {
        TTTDebug.LogWarning($"{nameof(UpdateCopyrights)} Can't find any .cs file under fixed paths.");
        displayString = groupedText.ToString();
        return;
      }

      displayString = groupedText.ToString();
      TTTDebug.Log($"{nameof(UpdateCopyrights)} Scanned {scriptFilePaths.Count} .cs file(s) across {targets.Count} path(s).");
      return;
    }

    /// <summary>
    /// 解析三个固定扫描目录。优先使用运行时推导的绝对路径，
    /// 以避免硬编码主工作目录带来的迁移成本。
    /// </summary>
    private static List<KeyValuePair<string, string>> ResolveTargetDirectories()
    {
      List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

      // 工程内路径：基于 Application.dataPath 推导（运行在 Unity Editor 中）。
      // dataPath 形如 "<主工作目录>\ToneTuneToolkit\ToneTuneToolkit\Assets"
      string projectAssets = Application.dataPath.Replace('\\', '/'); // .../ToneTuneToolkit/Assets
      string projectRoot = Path.GetFullPath(Path.Combine(projectAssets, "..")); // .../ToneTuneToolkit/ToneTuneToolkit
      string mainWorkRoot = Path.GetFullPath(Path.Combine(projectRoot, "..")); // .../ToneTuneToolkit

      result.Add(new KeyValuePair<string, string>(ModulesGroup,
        Path.Combine(projectRoot, "Assets/ToneTuneToolkit/Modules")));
      result.Add(new KeyValuePair<string, string>(ScriptsGroup,
        Path.Combine(projectRoot, "Assets/ToneTuneToolkit/Scripts")));
      result.Add(new KeyValuePair<string, string>(MaterialsGroup,
        Path.Combine(mainWorkRoot, "Materials")));

      return result;
    }

    // ==================================================

    /// <summary>
    /// 改变内容
    /// </summary>
    private void ChangeContent()
    {
      if (scriptFilePaths.Count <= 0) { return; }

      int successCount = 0;
      int skippedCount = 0;
      List<string> failureMessages = new List<string>();

      foreach (string filePath in scriptFilePaths)
      {
        try
        {
          if (!File.Exists(filePath))
          {
            skippedCount++;
            failureMessages.Add($"[skip] not found: {filePath}");
            continue;
          }

          List<string> fileContents = File.ReadAllLines(filePath).ToList();

          // 1) 定位并删除文件头中已有的 <summary>...</summary> 块
          int headerEnd = FindHeaderScanEnd(fileContents); // 文件头扫描范围 [0, headerEnd)
          int summaryStart = -1;
          int summaryEnd = -1;
          FindSummaryBlock(fileContents, 0, headerEnd, out summaryStart, out summaryEnd);

          if (summaryStart != -1 && summaryEnd != -1)
          {
            // 删除整段 <summary>...</summary>（含两个标记行）
            fileContents.RemoveRange(summaryStart, summaryEnd - summaryStart + 1);
          }

          // 2) 在文件最顶端插入新的版权块
          fileContents.Insert(0, $"/// </summary>");
          fileContents.Insert(0, $"/// Code Version {codeVersion}");
          fileContents.Insert(0, $"/// Copyright (c) {year} {author} All rights reserved.");
          fileContents.Insert(0, $"/// <summary>");

          File.WriteAllLines(filePath, fileContents);
          successCount++;
        }
        catch (Exception ex)
        {
          skippedCount++;
          failureMessages.Add($"[error] {filePath}: {ex.Message}");
        }
      }

      ClearFileInfos();

      TTTDebug.Log($"{nameof(UpdateCopyrights)} Done. {successCount} updated, {skippedCount} skipped.");
      if (failureMessages.Count > 0)
      {
        foreach (string msg in failureMessages)
        {
          TTTDebug.LogWarning(msg);
        }
      }

      AssetDatabase.Refresh();
      return;
    }

    /// <summary>
    /// 找到"文件头扫描范围"的右开边界：从第 0 行开始，
    /// 遇到第一个既不是 /// 注释、又不是空行的行就停止。
    /// 该范围内的内容被视为"可以安全替换的版权头区"。
    /// </summary>
    private static int FindHeaderScanEnd(List<string> lines)
    {
      int end = 0;
      while (end < lines.Count)
      {
        string line = lines[end] ?? string.Empty;
        string trimmed = line.Trim();
        if (trimmed.Length == 0)
        {
          end++;
          continue;
        }
        if (trimmed.StartsWith("///", StringComparison.Ordinal))
        {
          end++;
          continue;
        }
        break; // 遇到 using / namespace / 属性 / class 等真正代码
      }
      return end;
    }

    /// <summary>
    /// 在 [scanStart, scanEnd) 范围内查找第一个匹配的 <summary>...</summary> 块。
    /// 输出 summaryStart / summaryEnd（含两端），未找到时输出 -1。
    /// </summary>
    private static void FindSummaryBlock(List<string> lines, int scanStart, int scanEnd, out int summaryStart, out int summaryEnd)
    {
      summaryStart = -1;
      summaryEnd = -1;

      int upper = Mathf.Min(scanEnd, lines.Count);
      int openIdx = -1;
      for (int i = scanStart; i < upper; i++)
      {
        if (ContainsSummaryOpen(lines[i]))
        {
          openIdx = i;
          break;
        }
      }
      if (openIdx == -1)
      {
        return;
      }

      for (int i = openIdx + 1; i < upper; i++)
      {
        if (ContainsSummaryClose(lines[i]))
        {
          summaryStart = openIdx;
          summaryEnd = i;
          return;
        }
      }
    }

    private static readonly Regex SummaryOpenRegex = new Regex(@"<\s*summary\s*>", RegexOptions.Compiled);
    private static readonly Regex SummaryCloseRegex = new Regex(@"<\s*/\s*summary\s*>", RegexOptions.Compiled);

    private static bool ContainsSummaryOpen(string line)
    {
      return !string.IsNullOrEmpty(line) && SummaryOpenRegex.IsMatch(line);
    }

    private static bool ContainsSummaryClose(string line)
    {
      return !string.IsNullOrEmpty(line) && SummaryCloseRegex.IsMatch(line);
    }
  }
}
