/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Z CAM 接入模块：公共类型定义（状态、枚举、DTO、异常、参数键）
/// 本文件同时充当模块内的协议备忘录：凡实测有坑的键/值，把坑写进注释。
/// </summary>

using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace ToneTuneToolkit.IO.ZCAM
{
  /// <summary>
  /// 取景通道选择
  /// </summary>
  public enum ZCAMLiveViewChannel
  {
    /// <summary>MJPEG over HTTP：相机内固定 720p 编码器，零依赖，调试用</summary>
    MJPEG = 0,
    /// <summary>SSP + libssp 原生插件 + MF 解码：1080p RGBA 直灌纹理，延迟地板 + 逐帧墙钟 PTS（主通道）</summary>
    SSP = 1,
  }

  /// <summary>
  /// 取景源统一接口：MJPEG / SSP 源共用，Manager 按通道选择实现。
  /// JPEG 源帧数据自描述（LoadImage 解码）；SSP 源为 RGBA 裸数据（LoadRawTextureData 直灌）。
  /// </summary>
  public interface IZCAMViewSource
  {
    /// <summary>发起取景（必须主线程调用）</summary>
    void Start();
    /// <summary>停止并释放资源，可重复调用</summary>
    void Stop();
    /// <summary>首帧到达即完成；被停止/失败则 Faulted</summary>
    Task FirstFrameTask { get; }
    /// <summary>流是否已结束（异常终止或正常停止）</summary>
    bool IsCompleted { get; }
    /// <summary>异常终止时的错误信息；正常停止或流中返回 null</summary>
    string Error { get; }
    /// <summary>取走最新一帧（主线程），无新帧返回 null；旧帧被丢弃</summary>
    byte[] TryConsumeLatestFrame();
    /// <summary>帧是否为 RGBA 裸数据；false = JPEG</summary>
    bool IsRawRgba { get; }
    /// <summary>最近一帧宽度（仅 IsRawRgba 时有效，JPEG 帧自描述返回 0）</summary>
    int FrameWidth { get; }
    /// <summary>最近一帧高度（仅 IsRawRgba 时有效）</summary>
    int FrameHeight { get; }
  }

  /// <summary>
  /// 与相机的连接状态
  /// </summary>
  public enum ZCAMConnectionState
  {
    Disconnected = 0,
    Connecting = 1,
    Connected = 2,
  }

  /// <summary>
  /// 拍照画质（对应相机设置键 photo_q）。
  /// 注意：实机（E2-M4/0.98.2）仅验证过 JPEG；RAW/HEIF 的参数字符串待真机确认。
  /// </summary>
  public enum ZCAMPhotoQuality
  {
    JPEG = 0,
    RAW = 1,
    HEIF = 2,
  }

  /// <summary>
  /// 相机工作模式（/ctrl/mode?action=query 的 msg 值）。
  /// E2-M4 实测无独立拍照模式（to_cap 返回 -1），cap_* 值按协议文档预留。
  /// </summary>
  public enum ZCAMCameraMode
  {
    Unknown = 0,
    Rec,        // rec        录像模式（待机）
    RecIng,     // rec_ing    录像中
    RecPaused,  // rec_paused 录像暂停
    RecTlIdle,  // rec_tl_idle 延时录像待机
    RecTlIng,   // rec_tl_ing  延时录像中
    Pb,         // pb         回放模式
    PbIng,      // pb_ing     回放中
    PbPaused,   // pb_paused  回放暂停
    CapTlIdle,  // cap_tl_idle 延时拍照待机（协议预留）
    CapTlIng,   // cap_tl_ing  延时拍照中（协议预留）
    CapBurst,   // cap_burst   连拍（协议预留）
    Standby,    // standby    待机（任何 /ctrl/* 会自动唤醒）
  }

  /// <summary>
  /// 拍照目标分辨率（实现方式：拍照档 movfmt 映射——照片分辨率跟录制格式走）。
  /// 注意：跨档拍照需要临时切 movfmt 并在拍后还原，期间取景流会被重置（自动恢复）；
  /// 与当前 movfmt 同档时直拍，无任何扰动。
  /// </summary>
  public enum ZCAMPhotoResolution
  {
    /// <summary>1920×1080（movfmt=1080P29.97）</summary>
    FHD = 0,
    /// <summary>3840×2160（movfmt=4KP29.97）</summary>
    UHD = 1,
  }

  /// <summary>拍照分辨率 ↔ movfmt 档位映射（⚠️ 档位值精确拼写：4KP29.97，不存在 4KP30）</summary>
  public static class ZCAMPhotoResolutionExtensions
  {
    public static string ToMovfmt(this ZCAMPhotoResolution r)
    {
      return r == ZCAMPhotoResolution.UHD ? "4KP29.97" : "1080P29.97";
    }

    public static string ToDisplay(this ZCAMPhotoResolution r)
    {
      return r == ZCAMPhotoResolution.UHD ? "3840×2160" : "1920×1080";
    }
  }

  /// <summary>
  /// 工作模式协议字符串与枚举的互转及判断辅助
  /// </summary>
  public static class ZCAMCameraModeExtensions
  {
    /// <summary>相机返回字符串 → 枚举；无法识别（null/新固件新值）返回 Unknown，不抛异常</summary>
    public static ZCAMCameraMode FromCameraString(string mode)
    {
      switch (mode)
      {
        case "rec": return ZCAMCameraMode.Rec;
        case "rec_ing": return ZCAMCameraMode.RecIng;
        case "rec_paused": return ZCAMCameraMode.RecPaused;
        case "rec_tl_idle": return ZCAMCameraMode.RecTlIdle;
        case "rec_tl_ing": return ZCAMCameraMode.RecTlIng;
        case "pb": return ZCAMCameraMode.Pb;
        case "pb_ing": return ZCAMCameraMode.PbIng;
        case "pb_paused": return ZCAMCameraMode.PbPaused;
        case "cap_tl_idle": return ZCAMCameraMode.CapTlIdle;
        case "cap_tl_ing": return ZCAMCameraMode.CapTlIng;
        case "cap_burst": return ZCAMCameraMode.CapBurst;
        case "standby": return ZCAMCameraMode.Standby;
        default: return ZCAMCameraMode.Unknown;
      }
    }

    /// <summary>正在录像（含延时录像进行中）</summary>
    public static bool IsRecording(this ZCAMCameraMode m)
    {
      return m == ZCAMCameraMode.RecIng || m == ZCAMCameraMode.RecTlIng;
    }

    /// <summary>处于录像模式家族（可随时开录/抓帧拍照）</summary>
    public static bool IsRecordMode(this ZCAMCameraMode m)
    {
      return m == ZCAMCameraMode.Rec || m == ZCAMCameraMode.RecIng || m == ZCAMCameraMode.RecPaused
          || m == ZCAMCameraMode.RecTlIdle || m == ZCAMCameraMode.RecTlIng;
    }

    /// <summary>处于回放模式家族（拍照/录像前需 to_rec）</summary>
    public static bool IsPlayback(this ZCAMCameraMode m)
    {
      return m == ZCAMCameraMode.Pb || m == ZCAMCameraMode.PbIng || m == ZCAMCameraMode.PbPaused;
    }
  }

  /// <summary>
  /// 画质枚举与相机参数值的显式映射——不要用 ToString()，枚举改名不应影响协议
  /// </summary>
  public static class ZCAMPhotoQualityExtensions
  {
    public static string ToParamValue(this ZCAMPhotoQuality q)
    {
      switch (q)
      {
        case ZCAMPhotoQuality.RAW: return "RAW";
        case ZCAMPhotoQuality.HEIF: return "HEIF";
        default: return "JPEG";
      }
    }
  }

  /// <summary>
  /// HTTP 响应通用结构 {"code":0,"desc":"","msg":""}，code=0 为成功。
  /// code 默认 -1：解析不到该字段时不会被误判为成功。
  /// </summary>
  [Serializable]
  public class ZCAMResponse
  {
    public int code = -1;
    public string desc = string.Empty;
    public string msg = string.Empty;
  }

  /// <summary>
  /// /DCIM/ 与 /DCIM/100ZCAME 目录列表响应：files 位于顶层
  /// {"code":0,"desc":"","files":["100ZCAME","101ZCAME"]}
  /// </summary>
  [Serializable]
  public class ZCAMFileListResponse
  {
    public int code = -1;
    public string desc;
    public string[] files;
  }

  /// <summary>
  /// /info 响应载荷
  /// </summary>
  [Serializable]
  public class ZCAMInfoPayload
  {
    public string model;
    public string sw;
    public string mac;
    public string eth_ip;
    public string sn;
  }

  /// <summary>
  /// 相机信息（OpenCamera 成功后由 OnCameraOpened 事件与 CameraInfo 属性提供）
  /// </summary>
  public class ZCAMCameraInfo
  {
    public string Model;
    public string Firmware;
    public string Mac;
    public string EthIp;
    public string Sn;

    public override string ToString()
    {
      return $"[{Model ?? "?"}] fw={Firmware ?? "?"} ip={EthIp ?? "?"} sn={Sn ?? "?"}";
    }
  }

  /// <summary>
  /// 拍照结果：远端路径（相机 DCIM 内）、本地落盘路径、解码后的纹理
  /// </summary>
  public class ZCAMPhotoResult
  {
    /// <summary>相机上的文件路径，如 /DCIM/A001/A001C1411_20260922150306.DNG</summary>
    public string RemotePath;
    /// <summary>本地保存路径（Application.persistentDataPath/ZCAM/ 下），下载失败为 null</summary>
    public string LocalPath;
    /// <summary>
    /// 解码后的图像；RAW/HEIF 时为缩略图（?act=thm，JPEG 格式），解码失败为 null。
    /// 所有权归调用方：不再使用时请自行 Destroy 释放显存，模块不代管生命周期。
    /// </summary>
    public Texture2D Texture;
    /// <summary>画质档位</summary>
    public ZCAMPhotoQuality Quality;

    /// <summary>远端文件名（含扩展名）；RemotePath 为空时返回 null</summary>
    public string FileName
    {
      get { return string.IsNullOrEmpty(RemotePath) ? null : Path.GetFileName(RemotePath); }
    }

    public override string ToString()
    {
      return $"{RemotePath ?? "(无远端路径)"} -> {LocalPath ?? "(未下载)"} [{Quality}]";
    }
  }

  /// <summary>
  /// 相机状态快照（ZCAMManager.GetCameraStatus 返回）。
  /// 调用本身兼作保活心跳——任何 /ctrl/* 查询都会重置相机的待机计时器。
  /// </summary>
  public class ZCAMCameraStatus
  {
    public string Mode;        // 工作模式（原始协议字符串）：rec / rec_ing / pb / standby 等
    public string Battery;     // 电量百分比（字符串数值，如 "100"）
    public string AutoStandby; // 自动待机：Off / 1 min / 5 min / 15 min
    public string AutoOff;     // 自动关机：Off / 30s / 1 min / 2 min / 4 min / 8 min / 15 min
  }

  /// <summary>
  /// Z CAM 操作异常：携带相机返回的 code/desc/msg，或网络错误信息
  /// </summary>
  public class ZCAMException : Exception
  {
    /// <summary>相机返回码；-1 表示请求未到达相机、无响应或响应无法解析</summary>
    public int Code;
    public string Desc;

    /// <summary>是否网络层失败（未到达相机/无响应/解析失败），此时相机状态未知而非出错</summary>
    public bool IsNetworkError
    {
      get { return Code == -1; }
    }

    public ZCAMException(string message, int code = -1, string desc = null) : base(message)
    {
      Code = code;
      Desc = desc ?? string.Empty;
    }
  }

  /// <summary>
  /// 意外错误事件载荷（如取景流中断）；同步 API 的失败直接抛 ZCAMException，不走此事件
  /// </summary>
  public class ZCAMError
  {
    /// <summary>出错环节：LiveView / Session 等</summary>
    public string Context;
    public ZCAMException Exception;

    public override string ToString()
    {
      var msg = Exception != null ? Exception.Message : "未知错误";
      return $"[{Context ?? "?"}] {msg}";
    }
  }

  /// <summary>
  /// 常用相机设置键（完整清单见相机内置 /www/js/controller.js，约 150 键）
  /// </summary>
  public static class ZCAMParamKey
  {
    public const string PhotoQuality = "photo_q";       // JPEG / RAW / HEIF
    public const string RecordFormat = "movfmt";        // ⚠️ 值写作 4KP29.97（不存在 4KP30，写错返回 -1）；变更会重置 Stream 1 推流配置
    public const string RecordVFR = "movvfr";           // 可变帧率（升格）1~240
    public const string ISO = "iso";                    // E2-M4 实测 500~102400
    public const string ShutterTime = "shutter_time";   // E2-M4 实测 1/60~1/8000
    public const string WhiteBalance = "wb";            // Manual 档下 mwb/tint 才可写
    public const string EV = "ev";                      // E2-M4 实测 ±9.6
    public const string Iris = "iris";                  // E2-M4 实测 f/2.8~22
  }
}
