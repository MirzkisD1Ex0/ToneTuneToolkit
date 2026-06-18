using System;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Examples
{
  /// <summary>
  /// 
  /// </summary>
  public class SortTest : MonoBehaviour
  {
    public List<PlayerData> playerDatas;


    private void Start()
    {
      playerDatas = new List<PlayerData>()
      {
        new PlayerData {name = "Toto", score = 3, stringScore = "3"},
        new PlayerData {name = "Gar", score = 4, stringScore = "4"},
        new PlayerData {name = "Earth", score = 2, stringScore = "2"},
        new PlayerData {name = "Po", score = 1, stringScore = "1"}
      };

      playerDatas = playerDatas.OrderBy(x => float.Parse(x.stringScore)).ToList();
      // playerDatas = playerDatas.OrderBy(x => x.stringScore).ToList();
    }

  }

  [Serializable]
  public class PlayerData
  {
    public string name;
    public int score;
    public string stringScore;
  }
}