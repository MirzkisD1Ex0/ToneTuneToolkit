using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ToneTuneToolkit.Data;

public class Test01 : MonoBehaviour
{
  private void Start() => Init();
  private void Init()
  {
    string newMessage = DataProcessor.DoRichTextHighlight("A quick fox jumps over a lazy dog.", "quj", Color.red);
    UpdateText(newMessage);
    return;
  }

  [SerializeField] private Text textInfo;
  private void UpdateText(string value)
  {
    textInfo.text = value;
    return;
  }
}