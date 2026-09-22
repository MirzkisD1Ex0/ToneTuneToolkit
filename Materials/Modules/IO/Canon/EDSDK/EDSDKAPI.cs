/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.1
/// </summary>

using System;
using System.Runtime.InteropServices;

namespace ToneTuneToolkit.IO.Canon.EDSDK
{
  public static class EDSDKAPI
  {
    // ================= Windows COM (EDSDK 要求相机线程做 STA 初始化) =================
    [DllImport("ole32.dll")]
    private extern static int CoInitializeEx(IntPtr pvReserved, int dwCoInit);

    [DllImport("ole32.dll")]
    private extern static void CoUninitialize();

    public const int COINIT_APARTMENTTHREADED = 0x2;

    public static void EdsComInit()
    {
      // 返回 S_OK(0) / S_FALSE(1) 都视为成功；其余为失败。
      try { CoInitializeEx(IntPtr.Zero, COINIT_APARTMENTTHREADED); }
      catch { /* 已初始化或非 Windows 环境时忽略 */ }
    }

    public static void EdsComUninit() { try { CoUninitialize(); } catch { } }

    // ================= 基础 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsInitializeSDK();

    [DllImport("EDSDK.dll")]
    public extern static uint EdsTerminateSDK();

    [DllImport("EDSDK.dll")]
    public extern static uint EdsRetain(IntPtr inRef);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsRelease(IntPtr inRef);

    // ================= 对象树 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetChildCount(IntPtr inRef, out int outCount);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetChildAtIndex(IntPtr inRef, int inIndex, out IntPtr outRef);

    // ================= 属性 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetPropertySize(IntPtr inRef, uint inPropertyID, int inParam, out EdsDataType outDataType, out int outSize);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, int inPropertySize, IntPtr outPropertyData);

    [DllImport("EDSDK.dll", EntryPoint = "EdsSetPropertyData")]
    private extern static uint EdsSetPropertyDataRaw(IntPtr inRef, uint inPropertyID, int inParam, int inPropertySize, IntPtr inPropertyData);

    public static uint EdsSetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, uint value)
    {
      IntPtr ptr = Marshal.AllocHGlobal(sizeof(uint));
      Marshal.WriteInt32(ptr, (int)value);
      uint err = EdsSetPropertyDataRaw(inRef, inPropertyID, inParam, sizeof(uint), ptr);
      Marshal.FreeHGlobal(ptr);
      return err;
    }

    public static uint EdsSetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, byte[] value)
    {
      IntPtr ptr = Marshal.AllocHGlobal(value.Length);
      Marshal.Copy(value, 0, ptr, value.Length);
      uint err = EdsSetPropertyDataRaw(inRef, inPropertyID, inParam, value.Length, ptr);
      Marshal.FreeHGlobal(ptr);
      return err;
    }

    // 结构体值（EdsRect/EdsPoint/EdsSize/…）统一走泛型；非结构体的 uint/byte[] 走上面专用重载。
    public static uint EdsSetPropertyData<T>(IntPtr inRef, uint inPropertyID, int inParam, T value) where T : struct
    {
      int size = Marshal.SizeOf(typeof(T));
      IntPtr ptr = Marshal.AllocHGlobal(size);
      Marshal.StructureToPtr(value, ptr, false);
      uint err = EdsSetPropertyDataRaw(inRef, inPropertyID, inParam, size, ptr);
      Marshal.FreeHGlobal(ptr);
      return err;
    }

    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetPropertyDesc(IntPtr inRef, uint inPropertyID, out EdsPropertyDesc outPropertyDesc);

    // ---- GetPropertyData 便捷重载（内部调用上面那个 P/Invoke 重载）----
    public static uint EdsGetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, out uint outPropertyData)
    {
      int size = Marshal.SizeOf(typeof(uint));
      IntPtr ptr = Marshal.AllocHGlobal(size);
      uint err = EdsGetPropertyData(inRef, inPropertyID, inParam, size, ptr);
      outPropertyData = (uint)Marshal.PtrToStructure(ptr, typeof(uint));
      Marshal.FreeHGlobal(ptr);
      return err;
    }

    public static uint EdsGetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, out string outPropertyData)
    {
      IntPtr ptr = Marshal.AllocHGlobal(EDSDKConsts.EDS_MAX_NAME);
      uint err = EdsGetPropertyData(inRef, inPropertyID, inParam, EDSDKConsts.EDS_MAX_NAME, ptr);
      outPropertyData = Marshal.PtrToStringAnsi(ptr);
      Marshal.FreeHGlobal(ptr);
      return err;
    }

    public static uint EdsGetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, out int[] outPropertyData)
    {
      EdsDataType type; int size;
      EdsGetPropertySize(inRef, inPropertyID, 0, out type, out size);
      IntPtr ptr = Marshal.AllocHGlobal(size);
      uint err = EdsGetPropertyData(inRef, inPropertyID, inParam, size, ptr);
      int len = size / 4;
      outPropertyData = new int[len];
      Marshal.Copy(ptr, outPropertyData, 0, len);
      Marshal.FreeHGlobal(ptr);
      return err;
    }

    // 结构体输出（EdsRect/EdsPoint/EdsSize/EdsCameraPos/…）统一走泛型
    public static uint EdsGetPropertyData<T>(IntPtr inRef, uint inPropertyID, int inParam, out T outPropertyData) where T : struct
    {
      int size = Marshal.SizeOf(typeof(T));
      IntPtr ptr = Marshal.AllocHGlobal(size);
      uint err = EdsGetPropertyData(inRef, inPropertyID, inParam, size, ptr);
      outPropertyData = (T)Marshal.PtrToStructure(ptr, typeof(T));
      Marshal.FreeHGlobal(ptr);
      return err;
    }

    public static uint EdsGetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, out byte[] outPropertyData)
    {
      EdsDataType type; int size;
      EdsGetPropertySize(inRef, inPropertyID, 0, out type, out size);
      IntPtr ptr = Marshal.AllocHGlobal(size);
      uint err = EdsGetPropertyData(inRef, inPropertyID, inParam, size, ptr);
      outPropertyData = new byte[size];
      Marshal.Copy(ptr, outPropertyData, 0, size);
      Marshal.FreeHGlobal(ptr);
      return err;
    }

    // ================= 事件泵送 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetEvent();

    // ================= 相机 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetCameraList(out IntPtr outCameraListRef);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetDeviceInfo(IntPtr inCameraRef, out EdsDeviceInfo outDeviceInfo);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsOpenSession(IntPtr inCameraRef);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsCloseSession(IntPtr inCameraRef);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsSendCommand(IntPtr inCameraRef, uint inCommand, int inParam);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsSendStatusCommand(IntPtr inCameraRef, uint inStatusCommand, int inParam);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsSetCapacity(IntPtr inCameraRef, EdsCapacity inCapacity);

    // ================= 卷 / 目录项 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetVolumeInfo(IntPtr inVolumeRef, out EdsVolumeInfo outVolumeInfo);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetDirectoryItemInfo(IntPtr inDirItemRef, out EdsDirectoryItemInfo outDirItemInfo);

    // ================= 下载 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsDownload(IntPtr inDirItemRef, UInt64 inReadSize, IntPtr outStream);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsDownloadCancel(IntPtr inDirItemRef);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsDownloadComplete(IntPtr inDirItemRef);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsDownloadThumbnail(IntPtr inDirItemRef, IntPtr outStream);

    // ================= 流 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsCreateFileStream(string inFileName, EdsFileCreateDisposition inCreateDisposition, EdsAccess inDesiredAccess, out IntPtr outStream);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsCreateMemoryStream(UInt64 inBufferSize, out IntPtr outStream);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetLength(IntPtr inStreamRef, out UInt64 outLength);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetPointer(IntPtr inStreamRef, out IntPtr outPointer);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsRead(IntPtr inStreamRef, UInt64 inReadSize, IntPtr outBuffer, out UInt64 outReadSize);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsSetProgressCallback(IntPtr inRef, EdsProgressCallback inProgressFunc, EdsProgressOption inProgressOption, IntPtr inContext);

    // ================= 图像 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsCreateImageRef(IntPtr inStreamRef, out IntPtr outImageRef);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetImageInfo(IntPtr inImageRef, EdsImageSource inImageSource, out EdsImageInfo outImageInfo);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsGetImage(IntPtr inImageRef, EdsImageSource inImageSource, EdsTargetImageType inImageType, EdsRect inSrcRect, EdsSize inDstSize, IntPtr outStreamRef);

    // ================= 事件注册 =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsSetPropertyEventHandler(IntPtr inCameraRef, uint inEvent, EdsPropertyEventHandler inPropertyEventHandler, IntPtr inContext);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsSetObjectEventHandler(IntPtr inCameraRef, uint inEvent, EdsObjectEventHandler inObjectEventHandler, IntPtr inContext);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsSetCameraStateEventHandler(IntPtr inCameraRef, uint inEvent, EdsStateEventHandler inStateEventHandler, IntPtr inContext);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsSetCameraAddedHandler(EdsCameraAddedHandler inCameraAddedHandler, IntPtr inContext);

    // ================= LiveView (EVF) =================
    [DllImport("EDSDK.dll")]
    public extern static uint EdsCreateEvfImageRef(IntPtr inStreamRef, out IntPtr outEvfImageRef);

    [DllImport("EDSDK.dll")]
    public extern static uint EdsDownloadEvfImage(IntPtr inCameraRef, IntPtr evfImageRef);
  }
}
