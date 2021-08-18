/// <summary>
/// Copyright (c) 2021 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.0
/// </summary>

using UnityEngine;
using ToneTuneToolkit.Common;

namespace ToneTuneToolkit.Object
{
  /// <summary>
  /// 对象寻找器
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
        TipTools.Error("[ObjectSearcher] Cant find any [" + tag + "].");
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
      TipTools.Notice("[ObjectSearcher] Nearest " + tag + " is [" + nearestObject.name + "/" + lowestDistance + "].");
      return nearestObject;
    }
  }
}