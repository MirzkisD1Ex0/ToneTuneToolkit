using UnityEngine;
using ToneTuneToolkit.Common;

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
      TipTools.Notice(txt);

      string json = TextLoader.GetJson(ToolkitManager.ConfigsPath + "udpconfig.json", "Local IP");
      TipTools.Notice(json);
    }
  }
}