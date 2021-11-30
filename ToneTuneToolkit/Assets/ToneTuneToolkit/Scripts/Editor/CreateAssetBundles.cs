using UnityEditor;
using System.IO;

namespace ToneTuneToolkit.Editor
{
  public class CreateAssetBundles
  {
    [MenuItem("ToneTuneToolkit/Build AssetBundles")]
    static void BuildAllAssetBundles()
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