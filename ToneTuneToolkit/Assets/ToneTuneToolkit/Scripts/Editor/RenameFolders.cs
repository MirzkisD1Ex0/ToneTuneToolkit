/// <summary>
/// Copyright (c) 2024 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.18
/// </summary>

using System.IO;
using UnityEditor;
using UnityEngine;

namespace ToneTuneToolkit.Editor
{
  public class RenameFolders : EditorWindow
  {
    private string oldFolderName = "Original Folder Name";
    private string newFolderName = "New Folder Name";

    // ==================================================

    [MenuItem("ToneTuneToolkit/Rename Folders")]

    private static void Init()
    {
      RenameFolders window = (RenameFolders)GetWindow(typeof(RenameFolders));
      window.Show();
      return;
    }

    private void OnGUI() => OnGUIRenameFolders();

    // ==================================================c

    private void OnGUIRenameFolders()
    {
      GUILayout.Label("Original & New Folder Name(s):");
      oldFolderName = EditorGUILayout.TextField("Original Folder Name:", oldFolderName);
      newFolderName = EditorGUILayout.TextField("New Folder Name:", newFolderName);

      GUILayout.Space(EditorStorage.GUI.Space);

      GUILayout.BeginHorizontal();
      GUILayout.Label($"Rename Selected Folders to [{newFolderName}]");
      if (GUILayout.Button("Rename", GUILayout.Width(EditorStorage.GUI.NarrowWidth)))
      {
        RenameSelectedFolders();
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label($"Rename All [{oldFolderName}] to [{newFolderName}]");
      if (GUILayout.Button("Rename", GUILayout.Width(EditorStorage.GUI.NarrowWidth)))
      {
        RenameAllMatchedFolders();
      }
      GUILayout.EndHorizontal();
      return;
    }

    private void RenameSelectedFolders()
    {
      foreach (string guid in Selection.assetGUIDs)
      {
        string tempOld‌Directory‌Path = AssetDatabase.GUIDToAssetPath(guid);

        // 如果不是文件夹，则跳过
        if (!AssetDatabase.IsValidFolder(tempOld‌Directory‌Path))
        {
          continue;
        }

        string tempOldFolderName = Path.GetFileName(tempOld‌Directory‌Path);
        string tempNew‌Directory‌Path = tempOld‌Directory‌Path.Replace(tempOldFolderName, newFolderName);

        // Debug.Log(tempOldFolderPath);
        // Debug.Log(tempNewFolderPath);

        // 重命名文件夹
        Directory.Move(tempOld‌Directory‌Path, tempNew‌Directory‌Path);
        Directory.Move(tempOld‌Directory‌Path + ".meta", tempNew‌Directory‌Path + ".meta");
      }
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      return;
    }

    private void RenameAllMatchedFolders()
    {
      RenameAllMatchedFoldersAction(new DirectoryInfo(Application.dataPath));
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      return;
    }
    private void RenameAllMatchedFoldersAction(DirectoryInfo root)
    {
      // 遍历所有子文件夹
      foreach (DirectoryInfo directoryInfo in root.GetDirectories())
      {
        // 递归调用，遍历子文件夹中的文件夹
        RenameAllMatchedFoldersAction(directoryInfo);
      }

      string tempDirectoryPath = root.FullName.Replace(@"\", @"/");
      string tempFolderName = Path.GetFileName(tempDirectoryPath);

      if (tempFolderName == oldFolderName)
      {
        string tempNew‌Directory‌Path = tempDirectoryPath.Replace(tempFolderName, newFolderName);
        Directory.Move(tempDirectoryPath, tempNew‌Directory‌Path);
        Directory.Move(tempDirectoryPath + ".meta", tempNew‌Directory‌Path + ".meta");
      }
      return;
    }
  }
}