using UnityEngine;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class Dev : MonoBehaviour
  {
    private GameObject NodeImages;

    private void Start()
    {
      NodeImages = GameObject.Find("Node - Images").gameObject;


    }

    private void Update()
    {
      Debug.DrawLine(Vector3.zero, new Vector3(5, 0, 5), Color.red);
    }
  }
}