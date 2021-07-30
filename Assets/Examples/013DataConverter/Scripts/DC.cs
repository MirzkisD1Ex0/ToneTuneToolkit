using UnityEngine;
using ToneTuneToolkit.Common;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class DC : MonoBehaviour
  {
    private void Start()
    {
      string text = "The Word.";

      string bin = DataConverter.String2Binary(text);
      Debug.Log(bin);

      string newText = DataConverter.Binary2String(bin);
      Debug.Log(newText);

    }
  }
}