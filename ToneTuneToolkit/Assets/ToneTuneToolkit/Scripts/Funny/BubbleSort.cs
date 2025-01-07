/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

namespace ToneTuneToolkit.Funny
{
  /// <summary>
  /// 冒泡排序
  /// 整型版和浮点重载
  /// </summary>
  public static class BubbleSort
  {
    public static int[] Sort(int[] tempArray)
    {
      for (int j = 0; j < tempArray.Length; j++)
      {
        for (int i = tempArray.Length - 1; i > j; i--)
        {
          if (tempArray[i - 1] > tempArray[i])
          {
            int storage = tempArray[i];
            tempArray[i] = tempArray[i - 1];
            tempArray[i - 1] = storage;
          }
        }
      }
      return tempArray;
    }

    public static float[] Sort(float[] tempArray)
    {
      for (int j = 0; j < tempArray.Length; j++)
      {
        for (int i = tempArray.Length - 1; i > j; i--)
        {
          if (tempArray[i - 1] > tempArray[i])
          {
            float storage = tempArray[i];
            tempArray[i] = tempArray[i - 1];
            tempArray[i - 1] = storage;
          }
        }
      }
      return tempArray;
    }
  }
}
