/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ToneTuneToolkit.Common;
using ToneTuneToolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace ToneTuneToolkit.Data.XML
{
  /// <summary>
  /// 根据 Project 窗口中选中的 .xml 文件，在其同级目录下生成对应的 C# 实体类。
  /// </summary>
  public class XML2ClassGenerator : EditorWindow
  {
    private string selectedXmlAssetPath = "";
    private string targetClassName = "GameSettingConfig";

    // ==================================================

    [MenuItem("ToneTuneToolkit/Data/XML/XML 2 Class Generator")]

    private static void Init()
    {
      XML2ClassGenerator window = (XML2ClassGenerator)GetWindow(typeof(XML2ClassGenerator));
      window.titleContent = new GUIContent("XML 2 Class Generator");
      window.Show();
      return;
    }

    private void OnGUI() => OnGUIXML2ClassGenerator();

    // ==================================================

    private void OnGUIXML2ClassGenerator()
    {
      GUILayout.Label("XML 2 Class Generator", EditorStyles.boldLabel);
      EditorGUILayout.Space();

      bool hasValidSelection = TryGetSelectedXmlAssetPath(out string xmlAssetPath, out _);
      if (hasValidSelection)
      {
        selectedXmlAssetPath = xmlAssetPath;
        targetClassName = Path.GetFileNameWithoutExtension(xmlAssetPath);
      }
      else
      {
        selectedXmlAssetPath = string.Empty;
        targetClassName = string.Empty;
      }

      EditorGUILayout.LabelField("Selected XML:", string.IsNullOrEmpty(selectedXmlAssetPath) ? "(none — please select a .xml in the Project window)" : selectedXmlAssetPath);
      EditorGUILayout.LabelField("Target Class Name:", string.IsNullOrEmpty(targetClassName) ? "(derived from XML file name)" : targetClassName);

      GUILayout.Space(EditorStorage.GUI.Space);

      GUI.enabled = hasValidSelection;
      if (GUILayout.Button(hasValidSelection ? "Generate C# Class Next to Selected XML" : "Select an XML in Project First", GUILayout.Height(40)))
      {
        if (TryGenerateCSharpClass(xmlAssetPath, targetClassName, out string generatedPath, out string genError))
        {
          EditorUtility.DisplayDialog("Success", $"C# class generated to:\n{generatedPath}", "OK");
        }
        else
        {
          EditorUtility.DisplayDialog("Error", genError, "OK");
        }
      }
      GUI.enabled = true;
      return;
    }

    // ==================================================

    /// <summary>
    /// 从 Project 窗口当前选中项中找出第一个 .xml，并转换为 Unity 资源路径（"Assets/..."）。
    /// </summary>
    private static bool TryGetSelectedXmlAssetPath(out string assetPath, out string errorMessage)
    {
      assetPath = string.Empty;
      errorMessage = string.Empty;

      string[] guids = Selection.assetGUIDs;
      if (guids == null || guids.Length <= 0)
      {
        errorMessage = "No asset selected in the Project window.";
        return false;
      }

      foreach (string guid in guids)
      {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path))
        {
          continue;
        }
        if (!path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }
        assetPath = path;
        return true;
      }

      errorMessage = "Selected asset is not a .xml file.";
      return false;
    }

    /// <summary>
    /// 根据 Unity 资源路径（"Assets/..."）计算其在文件系统中的物理绝对路径。
    /// </summary>
    private static string GetPhysicalPath(string assetPath)
    {
      // Application.dataPath = "<工程根>/Assets"
      string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
      return Path.GetFullPath(Path.Combine(projectRoot, assetPath));
    }
    /// <summary>
    /// 由 .xml 的 Unity 资源路径推算"与 .xml 同级目录下的 .cs 保存路径"。
    /// </summary>
    private static string GetSavePathForSelectedXml(string xmlAssetPath, string className)
    {
      string xmlFullPath = GetPhysicalPath(xmlAssetPath);
      string xmlDirectory = Path.GetDirectoryName(xmlFullPath);
      return Path.Combine(xmlDirectory ?? Application.dataPath, $"{className}.cs");
    }

    /// <summary>
    /// 主体：解析 .xml → 生成 .cs → 写入"选中 XML 的同级目录"。
    /// </summary>
    private static bool TryGenerateCSharpClass(string xmlAssetPath, string className, out string generatedPath, out string errorMessage)
    {
      generatedPath = string.Empty;
      errorMessage = string.Empty;

      if (string.IsNullOrWhiteSpace(className))
      {
        errorMessage = "Target class name is empty.";
        return false;
      }

      string xmlFullPath = GetPhysicalPath(xmlAssetPath);
      if (!File.Exists(xmlFullPath))
      {
        errorMessage = $"XML file not found: {xmlFullPath}";
        Debug.LogError($"[{nameof(XML2ClassGenerator)}] {errorMessage}");
        return false;
      }

      string savePath = GetSavePathForSelectedXml(xmlAssetPath, className);
      if (File.Exists(savePath))
      {
        bool overwrite = EditorUtility.DisplayDialog(
          "File Exists",
          $"A .cs file already exists at:\n{savePath}\n\nOverwrite it?",
          "Overwrite",
          "Cancel");
        if (!overwrite)
        {
          errorMessage = "Generation cancelled by user.";
          TTTDebug.LogWarning($"[{nameof(XML2ClassGenerator)}] {errorMessage}");
          return false;
        }
      }

      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlFullPath);
        XmlElement root = doc.DocumentElement;
        if (root == null)
        {
          errorMessage = "XML has no root element.";
          Debug.LogError($"[{nameof(XML2ClassGenerator)}] {errorMessage}");
          return false;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Xml.Serialization;");
        sb.AppendLine();
        sb.AppendLine($"[XmlRoot(\"{root.Name}\")]");
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");

        // <类名, 节点>：需要在外层类之外再生成一个独立 class 的子对象
        Dictionary<string, XmlNode> subClassMap = new();

        foreach (XmlNode node in root.ChildNodes)
        {
          if (node.NodeType != XmlNodeType.Element) continue;
          ParseNode(node, sb, subClassMap, "    ");
        }

        sb.Append("}");

        foreach (KeyValuePair<string, XmlNode> entry in subClassMap)
        {
          sb.AppendLine();
          sb.AppendLine();
          sb.AppendLine($"public class {entry.Key}");
          sb.AppendLine("{");
          foreach (XmlNode subNode in entry.Value.ChildNodes)
          {
            if (subNode.NodeType != XmlNodeType.Element) continue;
            ParseNode(subNode, sb, null, "    ");
          }
          sb.Append("}");
        }

        File.WriteAllText(savePath, sb.ToString(), Encoding.UTF8);
        generatedPath = savePath;

        // 让 Unity 感知新文件
        string importedAssetPath = "Assets" + savePath[Application.dataPath.Length..].Replace('\\', '/');
        AssetDatabase.ImportAsset(importedAssetPath, ImportAssetOptions.ForceUpdate);

        TTTDebug.Log($"[{nameof(XML2ClassGenerator)}] Generated C# class: {savePath}");
        return true;
      }
      catch (Exception ex)
      {
        errorMessage = $"Failed to parse or write: {ex.Message}";
        Debug.LogError($"[{nameof(XML2ClassGenerator)}] {errorMessage}");
        return false;
      }
    }

    // ==================================================

    /// <summary>
    /// 核心解析逻辑：判断节点类型并转为 C# 属性字符串。
    /// </summary>
    private static void ParseNode(XmlNode node, StringBuilder sb, Dictionary<string, XmlNode> subClassMap, string indent)
    {
      string nodeName = node.Name;
      bool hasChildElements = false;

      foreach (XmlNode child in node.ChildNodes)
      {
        if (child.NodeType == XmlNodeType.Element)
        {
          hasChildElements = true;
          break;
        }
      }

      if (!hasChildElements)
      {
        // 情况 C：普通叶子字段
        string leafType = InferType(node.InnerText);
        sb.AppendLine($"{indent}[XmlElement(\"{nodeName}\")]");
        sb.AppendLine($"{indent}public {leafType} {nodeName} {{ get; set; }}");
        return;
      }

      // 情况 A：<ItemIds><Id> 这种数组/列表结构
      if (node.ChildNodes.Count > 1 && node.FirstChild.Name == node.LastChild.Name)
      {
        string childName = node.FirstChild.Name;
        string itemType = InferType(node.FirstChild.InnerText);
        sb.AppendLine($"{indent}[XmlArray(\"{nodeName}\")]");
        sb.AppendLine($"{indent}[XmlArrayItem(\"{childName}\")]");
        sb.Append($"{indent}public List<{itemType}> {nodeName} {{ get; set; }}");
        return;
      }

      // 情况 B：<PlayerSettings> 这种子对象
      string subClassName = nodeName;
      if (subClassMap != null && !subClassMap.ContainsKey(subClassName))
      {
        subClassMap.Add(subClassName, node);
      }
      sb.AppendLine($"{indent}[XmlElement(\"{nodeName}\")]");
      sb.Append($"{indent}public {subClassName} {nodeName} {{ get; set; }}");
      return;
    }

    /// <summary>
    /// 自动猜测数值类型：bool → int → float → string。
    /// </summary>
    private static string InferType(string value)
    {
      if (bool.TryParse(value, out _)) return "bool";
      if (int.TryParse(value, out _)) return "int";
      if (float.TryParse(value, out _)) return "float";
      return "string";
    }
  }
}
