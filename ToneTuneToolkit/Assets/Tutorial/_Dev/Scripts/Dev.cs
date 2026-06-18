using UnityEngine;
using System.Collections;
using ToneTuneToolkit;
using UnityEngine.SceneManagement;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class Dev : MonoBehaviour
  {
    public GameObject DD;

    private void Start()
    {
      Debug.Log(DD.GetComponent<CanvasGroup>());
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Q))
      {
        SceneManager.LoadScene(0);
      }

    }

  }
}