using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
  public static View Instance;

  public Text TextCOM;
  public InputField InputField;

  // ==================================================

  private void Awake()
  {
    Instance = this;
  }

  // ==================================================

  public void UpdateText(string value)
  {
    TextCOM.text = "Recived massage:" + value;
    return;
  }
}