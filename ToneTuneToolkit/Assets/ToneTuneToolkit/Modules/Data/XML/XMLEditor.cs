/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System;

namespace ToneTuneToolkit.Data.XML
{
  /// <summary>
  /// 先用工具在根目录下创建与XML等效的Class
  /// 
  /// 用法
  /// string path = Path.Combine(Application.streamingAssetsPath, "ToneTuneToolkit/Configs/GameSetting.xml");
  /// XMLSetting gs = XMLEditor.LoadXML<XMLSetting>(path);
  /// Debug.Log(gs.IPv4);
  /// gs.Port = 4215;
  /// XMLEditor.SaveXML(path, gs);
  /// </summary>
  public static class XMLEditor
  {
    /// <summary>
    /// 同步读取并反序列化 XML 文件。
    /// </summary>
    /// <param name="path">XML文件的完整物理路径</param>
    public static T LoadXML<T>(string path) where T : class
    {
      if (!File.Exists(path))
      {
        Debug.LogError($"[XML Editor] 未找到 XML 文件，路径: {path}");
        return null;
      }
      return Deserialize<T>(File.ReadAllText(path));
    }

    /// <summary>
    /// 将对象序列化后写回 XML 文件。若父目录不存在会自动创建。
    /// </summary>
    /// <param name="path">XML文件的完整物理路径</param>
    /// <param name="data">待写入的对象</param>
    /// <returns>写入是否成功</returns>
    public static bool SaveXML<T>(string path, T data) where T : class
    {
      if (data == null)
      {
        Debug.LogError("[XML Editor] 待写入的数据为 null");
        return false;
      }

      try
      {
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
          Directory.CreateDirectory(dir);
        }

        using (StringWriter writer = new StringWriter())
        {
          XmlSerializer serializer = new XmlSerializer(typeof(T));
          serializer.Serialize(writer, data);
          File.WriteAllText(path, writer.ToString());
        }
        return true;
      }
      catch (Exception ex)
      {
        Debug.LogError($"[XML Editor] 写入 XML 失败: {ex.Message} | 路径: {path}");
        return false;
      }
    }

    private static T Deserialize<T>(string xmlContent) where T : class
    {
      try
      {
        using (StringReader reader = new StringReader(xmlContent))
        {
          XmlSerializer serializer = new XmlSerializer(typeof(T));
          return serializer.Deserialize(reader) as T;
        }
      }
      catch (Exception ex)
      {
        Debug.LogError($"[XML Editor] XML 反序列化失败，请检查 XML 格式或实体类字段是否匹配。错误信息: {ex.Message}");
        return null;
      }
    }
  }
}
