using UnityEngine;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class Tips : MonoBehaviour
  {

    [SerializeField] private float ValueA = 0;

    [Range(0, 10)] public float ValueB = 1.5f;

    [Header("GG")] public GameObject GO;

  }
}