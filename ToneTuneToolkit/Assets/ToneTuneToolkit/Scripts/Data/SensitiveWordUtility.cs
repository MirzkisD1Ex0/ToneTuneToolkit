/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.4.20
/// </summary>

using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace ToneTuneToolkit.Data
{
  public class SensitiveWordUtility : MonoBehaviour
  {
    private const string END_FLAG = "IsEnd";
    private static Hashtable hashtable = new Hashtable();

    private List<string> lexicon = new List<string>();

    private string sensitiveWordsConfigPath = Application.streamingAssetsPath + "/configs/sensitivewords.json";

    // ==================================================

    private void Awake() => Init();

    // ==================================================

    private void Init()
    {
      lexicon = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(sensitiveWordsConfigPath, Encoding.UTF8));
      InitLexicon(lexicon);
      return;
    }

    // ==================================================

    /// <summary>
    /// 初始化词库
    /// </summary>
    public static void InitLexicon(List<string> lexicon)
    {
      hashtable = new Hashtable(lexicon.Count);
      foreach (string word in lexicon)
      {
        Hashtable tempHashtable = hashtable;
        for (int i = 0; i < word.Length; i++)
        {
          char c = word[i];
          if (IsSymbol(c)) { continue; }
          if (tempHashtable.ContainsKey(c))
          {
            tempHashtable = (Hashtable)tempHashtable[c];
          }
          else
          {
            var newHashtable = new Hashtable
          {
            { END_FLAG, 0 }
          };
            tempHashtable.Add(c, newHashtable);
            tempHashtable = newHashtable;
          }
          if (i == word.Length - 1)
          {
            if (tempHashtable.ContainsKey(END_FLAG))
            {
              tempHashtable[END_FLAG] = 1;
            }
            else
            {
              tempHashtable.Add(END_FLAG, 1);
            }
          }
        }
      }
      return;
    }



    public static bool CheckSensitiveWords(string text)
    {
      List<string> result = new List<string>();
      for (int i = 0; i < text.Length; i++)
      {
        int length = SearchSensitiveWord(text, i);
        if (length > 0)
        {
          result.Add(text.Substring(i, length));
          i = i + length - 1;

          return true;
        }
      }
      return false;
    }



    /// <summary>
    /// 查找所有敏感词，找到则返回敏感词长度
    /// </summary>
    /// <param name="text">需要过滤的字符串</param>
    /// <param name="startIndex">查找的起始位置</param>
    /// <returns></returns>
    public static int SearchSensitiveWord(string text, int startIndex)
    {
      Hashtable newMap = hashtable;
      bool flag = false;
      int len = 0;
      for (int i = startIndex; i < text.Length; i++)
      {
        char word = text[i];
        if (IsSymbol(word))
        {
          len++;
          continue;
        }
        Hashtable temp = (Hashtable)newMap[word];
        if (temp != null)
        {
          if ((int)temp[END_FLAG] == 1) flag = true;
          else newMap = temp;
          len++;
        }
        else break;
      }
      if (!flag) len = 0;
      return len;
    }

    /// <summary>
    /// 找到内容字符串内所有敏感词
    /// </summary>
    /// <param name="text">需要处理的文本</param>
    /// <returns></returns>
    public static List<string> GetAllSensitiveWords(string text)
    {
      List<string> result = new List<string>();
      for (int i = 0; i < text.Length; i++)
      {
        int length = SearchSensitiveWord(text, i);
        if (length > 0)
        {
          result.Add(text.Substring(i, length));
          i = i + length - 1;
        }
      }
      return result;
    }

    /// <summary>
    /// 替换 需要剔除的 敏感字
    /// </summary>
    /// <param name="text">需要处理的文本</param>
    /// <returns></returns>
    public static string ReplaceSensitiveWords(string text)
    {
      int i = 0;
      StringBuilder builder = new StringBuilder(text);
      while (i < text.Length)
      {
        int len = SearchSensitiveWord(text, i);
        if (len > 0)
        {
          for (int j = 0; j < len; j++)
          {
            builder[i + j] = '*';
          }
          i += len;
        }
        else ++i;
      }
      return builder.ToString();
    }

    /// <summary>
    /// 判断是否是一个符号
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private static bool IsSymbol(char c)
    {
      int ic = c;
      // 0x2E80-0x9FFF 东亚文字范围
      return !((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) && (ic < 0x2E80 || ic > 0x9FFF);
    }
  }
}