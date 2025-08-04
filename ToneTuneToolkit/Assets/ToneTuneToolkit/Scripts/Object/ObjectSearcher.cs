/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>



using UnityEngine;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 对象寻找器
  /// 找不到对象
  /// </summary>
  public class ObjectSearcher : MonoBehaviour
  {
    /// <summary>
    /// 寻找距离最近的特定类型的对象
    /// </summary>
    /// <param name="rootGO">桩对象</param>
    /// <param name="tag">搜寻对象的标签儿</param>
    /// <returns></returns>
    public static GameObject FindNearestObject(GameObject rootGO, string tag)
    {
      GameObject[] tempObject = GameObject.FindGameObjectsWithTag(tag);
      if (tempObject.Length <= 0)
      {
        Debug.Log("[ObjectSearcher] Cant find any [<color=red>" + tag + "</color>]...[Er]");
        return null;
      }

      GameObject nearestObject = tempObject[0];
      float lowestDistance = Vector3.Distance(rootGO.transform.position, tempObject[0].transform.position);
      for (int i = 0; i < tempObject.Length; i++)
      {
        float tempDistance = Vector3.Distance(rootGO.transform.position, tempObject[i].transform.position);
        if (tempDistance < lowestDistance)
        {
          lowestDistance = tempDistance;
          nearestObject = tempObject[i];
        }
      }
      Debug.Log($"[ObjectSearcher] Nearest [{tag}] is <color=green>{nearestObject.name}/{lowestDistance}</color>...[OK]");
      return nearestObject;
    }
  }
}
