/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.1
/// </summary>

using System;
using System.IO;
using System.Text;
using LitJson;

namespace ToneTuneToolkit.Data
{
  /// <summary>
  /// JSON 文件读写工具（LitJson 封装）
  /// Get 返回 JsonData（可继续索引或配合类反序列化）；Set 覆盖写顶层字段并落盘
  /// 注意：LitJson 的 ToJson 输出为紧凑格式（无缩进）；文件内容不含 // 注释（jsonc 不支持）
  /// </summary>
  public static class LitJsonEditor
  {
    /// <summary>
    /// 读取整个 JSON 文件为 JsonData
    /// </summary>
    public static JsonData Load(string path)
    {
      string jsonString = File.ReadAllText(path, Encoding.UTF8);
      return JsonMapper.ToObject(jsonString);
    }

    /// <summary>
    /// 读取顶层字段，返回 JsonData（可继续索引嵌套层或判型转换）
    /// </summary>
    public static JsonData Get(string path, string keyName)
    {
      return Load(path)[keyName];
    }

    /// <summary>
    /// 覆盖写顶层字段并保存到文件
    /// </summary>
    public static void Set(string path, string keyName, object value)
    {
      JsonData jd = Load(path);
      jd[keyName] = ToJsonData(value);
      File.WriteAllText(path, JsonMapper.ToJson(jd), Encoding.UTF8);
    }

    /// <summary>
    /// 保存 JsonData 到文件（整体覆写）
    /// </summary>
    public static void Save(string path, JsonData data)
    {
      File.WriteAllText(path, JsonMapper.ToJson(data), Encoding.UTF8);
    }

    /// <summary>
    /// object → JsonData（LitJson 无通用 object 构造，按类型分发）
    /// </summary>
    private static JsonData ToJsonData(object value)
    {
      if (value == null) return new JsonData();
      if (value is JsonData jd) return jd;
      if (value is string s) return new JsonData(s);
      if (value is bool b) return new JsonData(b);
      if (value is int i) return new JsonData(i);
      if (value is long l) return new JsonData(l);
      if (value is double d) return new JsonData(d);
      if (value is float f) return new JsonData((double)f);
      throw new ArgumentException("LitJsonEditor 不支持的值类型: " + value.GetType().Name);
    }
  }
}