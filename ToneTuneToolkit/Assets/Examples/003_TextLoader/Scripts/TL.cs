using UnityEngine;
using ToneTuneToolkit.Common;
using ToneTuneToolkit.Data;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class TL : MonoBehaviour
  {
    private void Start()
    {
      string txt = TextLoader.GetText(ToolkitManager.ConfigsPath + "sometext.txt", 1);
      TTTDebug.Log(txt);

      string json = JsonManager.GetJson(ToolkitManager.ConfigsPath + "udpconfig.json", "Local IP");
      TTTDebug.Log(json);
    }
  }
}