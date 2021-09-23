using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.WOL;

public class NewBehaviourScript : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    WakeOnLan.ShutdownOnLan();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
