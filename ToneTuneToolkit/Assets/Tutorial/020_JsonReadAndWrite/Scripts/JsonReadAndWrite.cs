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
      NewtonsoftJsonManager.SetJson(path, "delay", "100");
      Debug.Log(NewtonsoftJsonManager.GetJson(path, "delay"));
    }
  }
}