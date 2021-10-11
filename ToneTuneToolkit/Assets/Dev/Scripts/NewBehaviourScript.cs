using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.Common;

public class NewBehaviourScript : MonoBehaviour
{
  private void Start()
  {
    TextLoader.SetJson(Application.streamingAssetsPath + "/ToneTuneToolkit/configs/somejson.json", "set", "dasfgaghasdg");
    Debug.Log("dasd");
  }
}