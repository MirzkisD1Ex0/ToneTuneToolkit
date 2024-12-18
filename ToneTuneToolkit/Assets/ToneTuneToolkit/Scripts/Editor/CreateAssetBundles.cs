/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEditor;
using System.IO;

namespace ToneTuneToolkit.Editor
{
  public class CreateAssetBundles
  {
    [MenuItem("ToneTuneToolkit/Build AssetBundles")]
    private static void BuildAllAssetBundles()
    {
      string directory = "Assets/StreamingAssets/AssetBundles";
      if (Directory.Exists(directory) == false)
      {
        Directory.CreateDirectory(directory);
      }
      //BuildTarget 选择build出来的AB包要使用的平台
      BuildPipeline.BuildAssetBundles(directory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
  }
}