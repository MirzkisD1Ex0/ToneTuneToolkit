/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

namespace ToneTuneToolkit.LED
{
  /// <summary>
  /// LED基础指令集
  /// PornHub
  /// </summary>
  public static class LEDCommandHub
  {
    /// <summary>
    /// 基础构成
    /// </summary>
    public static class Basic
    {
      public const string BaseCommand = "[Dev]REQ=";
      public const string Port = "&Port=";
      public const string Begin = "&Begin=";
      public const string End = "&End=";
      public const string Color = "&Color=";
    }

    /// <summary>
    /// 基础命令
    /// </summary>
    public static class MethodType
    {
      public const string GetInfo = "GetInfo"; // 获取信息
      public const string SemanticLighting = "SemanticLighting&"; // 语义灯控
      public const string SetBrightnessBrightness = "SetBrightness&Brightness="; // 设置亮度
    }

    /// <summary>
    /// 语义灯控
    /// </summary>
    public static class SemanticLighting
    {
      public const string Action = "Action=";
      public const string DimColor = "DimColor";
      public const string DimEffect = "DimEffect";

      public const string DimEffectName = "&EffectName=";
      public const string DimEffectType = "&EffectType=";
      public const string DimEffectSpeed = "&Speed=";
    }
  }
}
