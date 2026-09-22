/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.5
/// </summary>

using System;
using System.Runtime.InteropServices;

namespace ToneTuneToolkit.IO.Canon.EDSDK
{
  public static class EDSDKConsts
  {
    public const int EDS_MAX_NAME = 256;
    public const int EDS_TRANSFER_BLOCK_SIZE = 512;

    // ---- 属性 ID（PropID_*）----
    public const uint PropID_ProductName = 0x00000002;
    public const uint PropID_SaveTo = 0x0000000b;
    public const uint PropID_ImageQuality = 0x00000100;
    public const uint PropID_AvailableShots = 0x0000040a;

    public const uint PropID_Evf_OutputDevice = 0x00000500;
    public const uint PropID_Evf_Mode = 0x00000501;
    public const uint PropID_Evf_Zoom = 0x00000507;
    public const uint PropID_Evf_ZoomPosition = 0x00000508;
    public const uint PropID_Evf_ImagePosition = 0x0000050B;
    public const uint PropID_Evf_AFMode = 0x0000050E;
    public const uint PropID_Evf_CoordinateSystem = 0x00000540;
    public const uint PropID_Evf_ZoomRect = 0x00000541;
    public const uint PropID_Evf_RollingPitching = 0x01000544;
    public const uint PropID_Evf_VisibleRect = 0x01000546;

    // ---- 相机指令（CameraCommand_*）----
    public const uint CameraCommand_TakePicture = 0x00000000;
    public const uint CameraCommand_ExtendShutDownTimer = 0x00000001; // 延长自动关机计时（Keep Device On）
    public const uint CameraCommand_PressShutterButton = 0x00000004;
    public const uint CameraCommand_DoEvfAf = 0x00000102;
    public const uint CameraCommand_DriveLensEvf = 0x00000103;
    public const uint CameraCommand_MovieSelectSwON = 0x00000107;
    public const uint CameraCommand_MovieSelectSwOFF = 0x00000108;

    // ---- 相机状态指令（CameraState_*，配合 EdsSendStatusCommand）----
    public const uint CameraState_UILock = 0x00000000;
    public const uint CameraState_UIUnLock = 0x00000001;
    public const uint CameraState_EnterDirectTransfer = 0x00000002;
    public const uint CameraState_ExitDirectTransfer = 0x00000003;

    // ---- LiveView 输出设备位（EvfOutputDevice_*）----
    public const uint EvfOutputDevice_TFT = 0x01;
    public const uint EvfOutputDevice_PC = 0x02;
    public const uint EvfOutputDevice_PC_Small = 0x08;

    // ---- 属性回调事件（PropertyEvent_*）----
    public const uint PropertyEvent_All = 0x00000100;
    public const uint PropertyEvent_PropertyChanged = 0x00000101;
    public const uint PropertyEvent_PropertyDescChanged = 0x00000102;

    // ---- 对象回调事件（ObjectEvent_*）----
    public const uint ObjectEvent_All = 0x00000200;
    public const uint ObjectEvent_VolumeInfoChanged = 0x00000201;
    public const uint ObjectEvent_FolderUpdateItems = 0x00000203;
    public const uint ObjectEvent_DirItemCreated = 0x00000204;
    public const uint ObjectEvent_DirItemRemoved = 0x00000205;
    public const uint ObjectEvent_DirItemInfoChanged = 0x00000206;
    public const uint ObjectEvent_DirItemContentChanged = 0x00000207; // 仅 Type2 协议机型
    public const uint ObjectEvent_DirItemRequestTransfer = 0x00000208;
    public const uint ObjectEvent_DirItemRequestTransferDT = 0x00000209;
    public const uint ObjectEvent_DirItemCancelTransferDT = 0x0000020a;
    public const uint ObjectEvent_VolumeAdded = 0x0000020c;
    public const uint ObjectEvent_VolumeRemoved = 0x0000020d;

    // ---- 状态回调事件（StateEvent_*）----
    public const uint StateEvent_All = 0x00000300;
    public const uint StateEvent_Shutdown = 0x00000301;
    public const uint StateEvent_JobStatusChanged = 0x00000302; // 有对象等待传输（type1 协议机型不发）
    public const uint StateEvent_WillSoonShutDown = 0x00000303; // 即将自动关机预警（inParameter=剩余秒数）
    public const uint StateEvent_ShutDownTimerUpdate = 0x00000304;
    public const uint StateEvent_CaptureError = 0x00000305; // 远程释放失败（inParameter=错误码 0x01~0x08）
    public const uint StateEvent_InternalError = 0x00000306; // SDK 内部错误（文档要求收到即取消远程连接）
    public const uint StateEvent_AfResult = 0x00000309;

    // ---- 错误码 ----
    public const uint EDS_ERR_OK = 0x00000000;
    public const uint EDS_ERR_INTERNAL_ERROR = 0x00000002;
    public const uint EDS_ERR_DEVICE_BUSY = 0x00000081;
    public const uint EDS_ERR_OBJECT_NOTREADY = 0x0000A102;
  }

  // ---- 委托（EDSdk 回调）----
  public delegate uint EdsProgressCallback(uint inPercent, IntPtr inContext, ref bool outCancel);
  public delegate uint EdsCameraAddedHandler(IntPtr inContext);
  public delegate uint EdsPropertyEventHandler(uint inEvent, uint inPropertyID, uint inParam, IntPtr inContext);
  public delegate uint EdsObjectEventHandler(uint inEvent, IntPtr inRef, IntPtr inContext);
  public delegate uint EdsStateEventHandler(uint inEvent, uint inParameter, IntPtr inContext);

  // ---- 枚举 ----
  public enum EdsDataType : uint
  {
    Unknown = 0, Bool = 1, String = 2, Int8 = 3, UInt8 = 6, Int16 = 4, UInt16 = 7,
    Int32 = 8, UInt32 = 9, Int64 = 10, UInt64 = 11, Float = 12, Double = 13,
    ByteBlock = 14, Rational = 20, Point = 21, Rect = 22, Time = 23,
  }

  public enum EdsAccess : uint { Read = 0, Write = 1, ReadWrite = 2, Error = 0xFFFFFFFF }

  public enum EdsFileCreateDisposition : uint
  {
    CreateNew = 0, CreateAlways = 1, OpenExisting = 2, OpenAlways = 3, TruncateExsisting = 4,
  }

  public enum EdsTargetImageType : uint
  {
    Unknown = 0x00000000, Jpeg = 0x00000001, TIFF = 0x00000007,
    TIFF16 = 0x00000008, RGB = 0x00000009, RGB16 = 0x0000000A,
  }

  public enum EdsImageSource : uint { FullView = 0, Thumbnail = 1, Preview = 2 }

  public enum EdsProgressOption : uint { NoReport = 0, Done = 1, Periodically = 2 }

  public enum EdsFileAttribute : uint
  {
    Normal = 0x00000000, ReadOnly = 0x00000001, Hidden = 0x00000002,
    System = 0x00000004, Archive = 0x00000020,
  }

  public enum EdsSaveTo : uint { Camera = 1, Host = 2, Both = 3 }

  public enum EdsShutterButton : uint
  {
    CameraCommand_ShutterButton_OFF = 0x00000000,
    CameraCommand_ShutterButton_Halfway = 0x00000001,
    CameraCommand_ShutterButton_Completely = 0x00000003,
    CameraCommand_ShutterButton_Halfway_NonAF = 0x00010001,
    CameraCommand_ShutterButton_Completely_NonAF = 0x00010003,
  }

  public enum EdsEvfAf : uint { CameraCommand_EvfAf_OFF = 0, CameraCommand_EvfAf_ON = 1 }

  // ---- 结构体 ----
  [StructLayout(LayoutKind.Sequential)]
  public struct EdsPoint { public int x; public int y; }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsRect { public int x; public int y; public int width; public int height; }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsSize { public int width; public int height; }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsRational { public int Numerator; public uint Denominator; }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsTime
  {
    public int Year; public int Month; public int Day;
    public int Hour; public int Minute; public int Second; public int Milliseconds;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsCameraPos { public int status; public int position; public int rolling; public int pitching; }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsDeviceInfo
  {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EDSDKConsts.EDS_MAX_NAME)] public string szPortName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EDSDKConsts.EDS_MAX_NAME)] public string szDeviceDescription;
    public uint DeviceSubType;
    public uint reserved;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsVolumeInfo
  {
    public uint StorageType;
    public uint Access;
    public ulong MaxCapacity;
    public ulong FreeSpaceInBytes;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EDSDKConsts.EDS_MAX_NAME)] public string szVolumeLabel;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsDirectoryItemInfo
  {
    public UInt64 Size;
    public int isFolder;
    public uint GroupID;
    public uint Option;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = EDSDKConsts.EDS_MAX_NAME)] public string szFileName;
    public uint format;
    public uint dateTime;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsImageInfo
  {
    public uint Width;
    public uint Height;
    public uint NumOfComponents;
    public uint ComponentDepth;
    public EdsRect EffectiveRect;
    public uint reserved1;
    public uint reserved2;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct EdsPropertyDesc
  {
    public int Form;
    public uint Access;
    public int NumElements;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)] public int[] PropDesc;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 2)]
  public struct EdsCapacity
  {
    public int NumberOfFreeClusters;
    public int BytesPerSector;
    public int Reset;
  }
}
