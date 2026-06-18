/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System;
using System.Runtime.InteropServices;

namespace SeaorySDK
{
  class UhfApi
  {
    #region UHF tag data section
    //0表示标签RESERVED存储区，1表示EPC数据区，2表示TID数据区，3表示USER数据区
    public const int UHF_DATA_SECTION_RESERVED = 0;
    public const int UHF_DATA_SECTION_EPC = 1;
    public const int UHF_DATA_SECTION_TID = 2;
    public const int UHF_DATA_SECTION_USER = 3;
    #endregion

    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfReaderConnect(ref IntPtr hCom, string cPort, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfReaderDisconnect(ref IntPtr hCom, byte flagCrc);
    //[DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    //public static extern int UhfGetVersion(IntPtr hCom, [Out] char[] uVersion, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfGetPower(IntPtr hCom, [Out] char[] uPower, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfSetPower(IntPtr hCom, char uOption, int uPower, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfGetFrequency(IntPtr hCom, [Out] byte[] uFreMode, [Out] byte[] uFreBase, [Out] byte[] uBaseFre, [Out] byte[] uChannNum, [Out] byte[] uChannSpc, [Out] byte[] uFreHop, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfSetFrequency(IntPtr hCom, byte uFreMode, byte uFreBase, [Out] byte[] uBaseFre, byte uChannNum, byte uChannSpc, byte uFreHop, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfGetReaderUID(IntPtr hCom, [Out] byte[] uUid, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfStartInventory(IntPtr hCom, byte flagAnti, byte initQ, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfReadInventory(IntPtr hCom, [Out] byte[] uLenUii, [Out] byte[] uUii);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfRecvData(IntPtr hCom, [Out] byte[] uUii);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfStopOperation(IntPtr hCom, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfReadDataByEPC(IntPtr hCom, string accessPwd, int memBank, int sa, int dl, [Out] byte[] uDataReturn, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfReadDataByTID(IntPtr hCom, int sa, int dl, [Out] byte[] uDataReturn, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfReadDataByXZEPC(IntPtr hCom, char[] epcLabelStr, char[] accessPwd, int memBank, int sa, int dl, [Out] byte[] uDataReturn, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfInventorySingleTag(IntPtr hCom, ref byte uLenUii, [Out] byte[] uUii, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfAddFilter(IntPtr hCom, int intSelTarget, int intAction, int intSelMemBank, int intSelPointer, int intMaskLen, int intTruncate, string txtSelMask, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfDeleteFilterByIndex(IntPtr hCom, byte SINDEX, [Out] byte[] STATUS, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfStartGetFilterByIndex(IntPtr hCom, byte SINDEX, byte SNUM, [Out] byte[] STATUS, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfWriteDataByEPC(IntPtr hCom, string uAccessPwd, byte uBank, string uPtr, byte uCnt, [Out] byte[] uWriteData, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfWriteDataByEPCEx(IntPtr hCom, char[] uAccessPwd, byte uBank, char[] uPtr, byte uCnt, [Out] byte[] uWriteData, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfWriteDataByXZEPC(IntPtr hCom, char[] EPCXZ, char[] uAccessPwd, byte uBank, char[] uPtr, byte uCnt, [Out] byte[] uWriteData, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfWriteDataByUSER(IntPtr hCom, char[] uAccessPwd, char[] uPtr, byte uCnt, [Out] byte[] uWriteData, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfEraseDataByEPC(IntPtr hCom, char[] uAccessPwd, byte uBank, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfChangeConfig(IntPtr hCom, char[] uAccessPwd, int Config, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfSetReadProtect(IntPtr hCom, char[] uAccessPwd, int Config, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfChangeEAS(IntPtr hCom, char[] uAccessPwd, int ConfigPSF, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfEASAlarm(IntPtr hCom, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfChangeQTControl(IntPtr hCom, char[] uAccessPwd, int ReadWrite, int Persistence, char[] Payload, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfLockMemByEPC(IntPtr hCom, char[] EPCXZ, char[] uAccessPwd, char[] uLockData, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfunLockMemByEPC(IntPtr hCom, char[] EPCXZ, char[] uAccessPwd, char[] uLockData, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfKillTagByEPC(IntPtr hCom, char[] uKillPwd, ref byte uErrorCode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfSetMode(IntPtr hCom, byte mode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfSaveConfig(IntPtr hCom, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfSleep(IntPtr hCom, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfAutoFrequeC(IntPtr hCom, int mode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfSelectQ(IntPtr hCom, int mode, byte flagCrc);
    [DllImport("UhfReader_API.dll", CharSet = CharSet.Ansi)]
    public static extern int UhfControIO(IntPtr hCom, int ioCtrlType, int param1, int param2, byte flagCrc);
  }
}
