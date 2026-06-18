using UnityEngine;
using ToneTuneToolkit.Data;

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
      NewtonsoftJsonEditor.SetJson(path, "delay", "100");
      Debug.Log(NewtonsoftJsonEditor.GetJson(path, "delay"));
    }
  }
}