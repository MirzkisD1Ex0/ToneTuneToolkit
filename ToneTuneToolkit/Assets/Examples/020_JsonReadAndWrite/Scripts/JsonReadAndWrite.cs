using UnityEngine;
using ToneTuneToolkit.Common;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class JsonReadAndWrite : MonoBehaviour
  {
    private void Start()
    {
      string path = Application.streamingAssetsPath + "/ToneTuneToolkit/configs/somejson.json";
      TextLoader.SetJson(path, "delay", "100");
      Debug.Log(TextLoader.GetJson(path, "delay"));
    }
  }
}