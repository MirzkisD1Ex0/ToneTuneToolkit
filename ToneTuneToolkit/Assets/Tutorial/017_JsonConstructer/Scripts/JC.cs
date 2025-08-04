using UnityEngine;
using System.Collections.Generic;
using ToneTuneToolkit.Data;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class JC : MonoBehaviour
  {
    private void Start()
    {
      Dictionary<string, string> testDic = new Dictionary<string, string>();
      testDic["KeyA"] = "ValueA";
      testDic["KeyB"] = "ValueB";
      string testSting = DataConverter.Dic2Json(testDic);
      Debug.Log(testSting);

      Dictionary<string, string> dic = new Dictionary<string, string>();
      dic = DataConverter.Json2Dic(testSting);
      Debug.Log(dic["KeyA"]);
    }
  }
}