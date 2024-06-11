using UnityEngine;
using System.Collections;
using ToneTuneToolkit;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class Dev : MonoBehaviour
  {

    private void Start()
    {

    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Q))
      {
        StartCoroutine("Count");
      }
      if (Input.GetKeyDown(KeyCode.W))
      {
        StopCoroutine("Count");
      }
    }

    private IEnumerator Count()
    {
      Debug.Log("启动");
      yield return new WaitForSeconds(3f);
      Debug.Log("3s");
      StartCoroutine("Count");
    }
  }
}