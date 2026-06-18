/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>
using System;
using System.Runtime.InteropServices;

namespace SeaorySDK
{
  public class ScardApi
  {
    [DllImport("SeaorySDK.dll")]
    public static extern IntPtr SOY_SC_Init(short port);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Exit(IntPtr hDev);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_GetVer(IntPtr hDev, [Out] char[] sver);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ResetDevice(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Beep(IntPtr hDev, short _Msec);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Load_Key(IntPtr hDev, byte _Mode, byte _SecNr, byte[] _NKey);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Load_Key_hex(IntPtr hDev, byte _Mode, byte _SecNr, char[] _NKey);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Hex_A(byte[] hex, [Out] char[] a, short len);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_A_Hex(char[] a, [Out] byte[] hex, short len);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Des(byte[] key, byte[] src, [Out] byte[] dest, short m);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Des_hex(char[] key, char[] sour, [Out] char[] dest, short m);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_TripleDes(byte[] key, byte[] src, [Out] byte[] dest, short m);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_TripleDes_hex(char[] key, char[] src, [Out] char[] dest, short m);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Set_CPU(IntPtr hDev, byte _Byte);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuReset(IntPtr hDev, ref byte rlen, [Out] byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CpuReset_hex(IntPtr hDev, ref byte rlen, [Out] char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuHotReset(IntPtr hDev, ref byte rlen, [Out] byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CpuHotReset_hex(IntPtr hDev, ref byte rlen, [Out] char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuApdu(IntPtr hDev, byte slen, byte[] sendbuffer, ref byte rlen, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CpuApdu_hex(IntPtr hDev, byte slen, char[] sendbuffer, ref byte rlen, [Out] char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuApduInt(IntPtr hDev, uint slen, byte[] sendbuffer, ref UInt32 rlen, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CpuApduInt_hex(IntPtr hDev, uint slen, char[] sendbuffer, ref UInt32 rlen, [Out] char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuApduEXT(IntPtr hDev, short slen, byte[] sendbuffer, ref short rlen, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CpuApduEXT_hex(IntPtr hDev, short slen, char[] sendbuffer, ref short rlen, [Out] char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuApduSource(IntPtr hDev, byte slen, byte[] sendbuffer, ref byte rlen, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CpuApduSource_hex(IntPtr hDev, byte slen, char[] sendbuffer, ref byte rlen, [Out] char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuApduSourceEXT(IntPtr hDev, short slen, byte[] sendbuffer, ref short rlen, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CpuApduSourceEXT_hex(IntPtr hDev, short slen, char[] sendbuffer, ref short rlen, [Out] char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuApduRespon(IntPtr hDev, byte slen, byte[] sendbuffer, ref byte rlen, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CpuApduRespon_hex(IntPtr hDev, byte slen, char[] sendbuffer, ref byte rlen, [Out] char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuApduRespon_Int(IntPtr hDev, uint slen, byte[] sendbuffer, ref UInt32 rlen, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CpuApduRespon_Int_hex(IntPtr hDev, uint slen, char[] sendbuffer, ref UInt32 rlen, [Out] char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CpuDown(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Reset(IntPtr hDev, short _Msec);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ConfigCard(IntPtr hDev, byte cardtype);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Card_Exist(IntPtr hDev, ref short flag);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Card_Status(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_TypeAB_Card_Status(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Request(IntPtr hDev, byte _Mode, ref short TagType);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Anticoll(IntPtr hDev, byte _Bcnt, ref UInt32 _Snr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Select(IntPtr hDev, uint _Snr, [Out] byte[] _Size);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Anticoll2(IntPtr hDev, byte _Bcnt, ref UInt32 _Snr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Select2(IntPtr hDev, uint _Snr, [Out] byte[] _Size);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Halt(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Card(IntPtr hDev, byte _Mode, ref UInt32 _Snr);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Card_hex(IntPtr hDev, byte _Mode, [Out] char[] snrstr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Card_n(IntPtr hDev, byte _Mode, ref UInt32 SnrLen, [Out] byte[] _Snr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Card_n_hex(IntPtr hDev, byte _Mode, ref UInt32 SnrLen, [Out] char[] _Snr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Card_double(IntPtr hDev, byte _Mode, [Out] byte[] _Snr);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Card_double_hex(IntPtr hDev, byte _Mode, [Out] char[] _Snr);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CardStr(IntPtr hDev, byte _Mode, [Out] char[] Strsnr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Card_AB(IntPtr hDev, ref byte rlen, [Out] byte[] rbuf, ref byte type);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Card_B(IntPtr hDev, [Out] byte[] rbuf);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Card_B_hex(IntPtr hDev, [Out] char[] rbuf);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Request_B(IntPtr hDev, byte _Mode, byte AFI, byte N, [Out] byte[] ATQB);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_SlotMarker(IntPtr hDev, byte N, [Out] byte[] ATQB);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Attri_B(IntPtr hDev, byte[] PUPI, byte CID);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Authentication(IntPtr hDev, byte _Mode, byte _SecNr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Authentication_2(IntPtr hDev, byte _Mode, byte KeyNr, byte Adr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_AuthenticationPassAddr(IntPtr hDev, byte _Mode, byte _Addr, byte[] passbuff);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_AuthenticationPassAddr_hex(IntPtr hDev, byte _Mode, byte _Addr, byte[] passbuff);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Authentication_pass(IntPtr hDev, byte _Mode, byte _Addr, byte[] passbuff);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Authentication_pass_hex(IntPtr hDev, byte _Mode, byte _Addr, byte[] passbuff);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_HL_Authentication(IntPtr hDev, byte reqmode, uint snr, byte authmode, byte secnr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read(IntPtr hDev, byte _Adr, [Out] byte[] _Data);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Read_hex(IntPtr hDev, byte _Adr, [Out] char[] _Data);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write(IntPtr hDev, byte _Adr, byte[] _Data);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Write_hex(IntPtr hDev, byte _Adr, char[] _Data);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ChangeB3(IntPtr hDev, byte _SecNr, byte[] _KeyA, byte _B0, byte _B1, byte _B2, byte _B3, byte _Bk, byte[] _KeyB);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ChangeB3_hex(IntPtr hDev, byte _SecNr, char[] _KeyA, byte _B0, byte _B1, byte _B2, byte _B3, byte _Bk, char[] _KeyB);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Restore(IntPtr hDev, byte _Adr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Transfer(IntPtr hDev, byte _Adr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_InitVal(IntPtr hDev, byte _Adr, uint _Value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadVal(IntPtr hDev, byte _Adr, ref UInt32 _Value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Increment(IntPtr hDev, byte _Adr, uint _Value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Decrement(IntPtr hDev, byte _Adr, uint _Value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_InitVal_ml(IntPtr hDev, short _Value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadVal_ml(IntPtr hDev, ref short _Value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Decrement_ml(IntPtr hDev, short _Value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_Write(IntPtr hDev, uint Snr, byte authmode, byte Adr, byte[] _data);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Check_Write_hex(IntPtr hDev, uint Snr, byte authmode, byte Adr, [Out] char[] _data);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Mfdes_Auth(IntPtr hDev, byte keyno, byte keylen, byte[] authkey, byte[] randAdata, [Out] byte[] randBdata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Mfdes_Auth_hex(IntPtr hDev, byte keyno, byte keylen, byte[] authkey, byte[] randAdata, [Out] char[] randBdata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_Reset(IntPtr hDev, ref byte rlen, [Out] byte[] receive_data);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Pro_Reset_hex(IntPtr hDev, ref byte rlen, [Out] char[] receive_data);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_ResetInt(IntPtr hDev, ref byte rlen, [Out] byte[] receive_data);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Pro_ResetInt_hex(IntPtr hDev, ref byte rlen, [Out] char[] receive_data);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_Command(IntPtr hDev, byte slen, byte[] sendbuffer, ref byte rlen, [Out] byte[] databuffer, byte timeout);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Pro_Command_hex(IntPtr hDev, byte slen, char[] sendbuffer, ref byte rlen, [Out] char[] databuffer, byte timeout);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_Command_Int(IntPtr hDev, uint slen, byte[] sendbuffer, ref UInt32 rlen, [Out] byte[] databuffer, byte timeout);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Pro_Command_Int_hex(IntPtr hDev, uint slen, char[] sendbuffer, ref UInt32 rlen, [Out] char[] databuffer, byte timeout);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_CommandSource(IntPtr hDev, byte slen, byte[] sendbuffer, ref byte rlen, [Out] byte[] databuffer, byte timeout);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Pro_CommandSource_hex(IntPtr hDev, byte slen, char[] sendbuffer, ref byte rlen, [Out] char[] databuffer, byte timeout);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_CommandSource_int(IntPtr hDev, uint slen, byte[] sendbuffer, ref UInt32 rlen, [Out] byte[] databuffer, byte timeout);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_CommandSource_CRC(IntPtr hDev, byte slen, byte[] sendbuffer, ref byte rlen, [Out] byte[] databuffer, byte timeout, byte CRCSTU);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Pro_CommandSource_CRC_hex(IntPtr hDev, byte slen, char[] sendbuffer, ref byte rlen, [Out] char[] databuffer, byte timeout, byte CRCSTU);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_CommandLink(IntPtr hDev, byte slen, byte[] sendbuffer, ref byte rlen, [Out] byte[] databuffer, byte timeout, byte FG);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Pro_CommandLink_hex(IntPtr hDev, byte slen, char[] sendbuffer, ref byte rlen, [Out] char[] databuffer, byte timeout, byte FG);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_CommandLinkInt(IntPtr hDev, uint slen, byte[] sendbuffer, ref UInt32 rlen, [Out] byte[] databuffer, byte timeout);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Pro_CommandLinkInt_hex(IntPtr hDev, uint slen, char[] sendbuffer, ref UInt32 rlen, [Out] char[] databuffer, byte timeout);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_CommandLinkEXT(IntPtr hDev, uint slen, byte[] sendbuffer, ref UInt32 rlen, [Out] byte[] databuffer, byte timeout, byte FG);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Pro_CommandLinkEXT_hex(IntPtr hDev, uint slen, char[] sendbuffer, ref UInt32 rlen, [Out] char[] databuffer, byte timeout, byte FG);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Pro_Halt(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Request_Shc1102(IntPtr hDev, byte _Mode, ref short TagType);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Auth_Shc1102(IntPtr hDev, byte[] _Data);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_Shc1102(IntPtr hDev, byte _Adr, [Out] byte[] _Data);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_Shc1102(IntPtr hDev, byte _Adr, byte[] _Data);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Halt_Shc1102(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L0_WritePerson(IntPtr hDev, uint BNr, byte[] dataperso);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_MF_PLUS_L0_WritePerson_hex(IntPtr hDev, uint BNr, char[] dataperso);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L0_CommitPerson(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L1_Auth_L1key(IntPtr hDev, byte[] authkey);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_MF_PLUS_L1_Auth_L1key_hex(IntPtr hDev, char[] authkey);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L1_Switch_toL2(IntPtr hDev, byte[] authkey);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L1_Switch_toL3(IntPtr hDev, byte[] authkey);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L2_Switch_toL3(IntPtr hDev, byte[] authkey);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L3_Auth_L3key(IntPtr hDev, uint keyBNr, byte[] authkey);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_MF_PLUS_L3_Auth_L3_hex(IntPtr hDev, uint keyBNr, char[] authkey);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L3_Auth_L3_SectorKey(IntPtr hDev, byte mode, uint sectorBNr, byte[] authkey);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_MF_PLUS_L3_Auth_L3_SectorKey_hex(IntPtr hDev, byte mode, uint sectorBNr, char[] authkey);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L3_ReadInPlain(IntPtr hDev, uint BNr, byte Numblock, [Out] byte[] readdata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_MF_PLUS_L3_ReadInPlain_hex(IntPtr hDev, uint BNr, byte Numblock, [Out] char[] readdata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L3_ReadEncrypted(IntPtr hDev, uint BNr, byte Numblock, byte[] readdata, byte flag);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_MF_PLUS_L3_ReadEncrypted_hex(IntPtr hDev, uint BNr, byte Numblock, [Out] char[] readdata, byte flag);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L3_WriteInPlain(IntPtr hDev, uint BNr, byte Numblock, byte[] writedata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_MF_PLUS_L3_WriteInPlain_hex(IntPtr hDev, uint BNr, byte Numblock, char[] writedata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_MF_PLUS_L3_WriteEncrypted(IntPtr hDev, uint BNr, byte Numblock, byte[] writedata, byte flag);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_MF_PLUS_L3_WriteEncrypted_hex(IntPtr hDev, uint BNr, byte Numblock, char[] writedata, byte flag);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Auth_Ulc(IntPtr hDev, byte[] key);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Auth_Ulc_hex(IntPtr hDev, char[] key);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Changekey_Ulc(IntPtr hDev, byte[] newkey);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Changekey_Ulc_hex(IntPtr hDev, char[] newkey);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_4442(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Down_4442(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_VerifyPin_4442(IntPtr hDev, byte[] passwd);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_VerifyPin_4442_hex(IntPtr hDev, char[] passwd);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadPin_4442(IntPtr hDev, byte[] passwd);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ReadPin_4442_hex(IntPtr hDev, char[] passwd);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadPinCount_4442(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ChangePin_4442(IntPtr hDev, byte[] passwd);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ChangePin_4442_hex(IntPtr hDev, char[] passwd);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadProtect_4442(IntPtr hDev, short offset, short length, [Out] byte[] data_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ReadProtect_4442_hex(IntPtr hDev, short offset, short length, [Out] char[] data_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_WriteProtect_4442(IntPtr hDev, short offset, short length, byte[] data_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_WriteProtect_4442_hex(IntPtr hDev, short offset, short length, char[] data_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_4442(IntPtr hDev, short offset, short length, [Out] byte[] data_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Read_4442_hex(IntPtr hDev, short offset, short length, [Out] char[] data_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_4442(IntPtr hDev, short offset, short length, byte[] data_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Write_4442_hex(IntPtr hDev, short offset, short length, char[] data_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_4428(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Down_4428(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_VerifyPin_4428(IntPtr hDev, byte[] passwd);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_VerifyPin_4428_hex(IntPtr hDev, char[] passwd);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadPin_4428(IntPtr hDev, [Out] byte[] passwd);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ReadPin_4428_hex(IntPtr hDev, [Out] char[] passwd);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadPinCount_4428(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ChangePin_4428(IntPtr hDev, byte[] passwd);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ChangePin_4428_hex(IntPtr hDev, char[] passwd);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadProtect_4428(IntPtr hDev, short offset, short length, [Out] byte[] data_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ReadProtect_4428_hex(IntPtr hDev, short offset, short length, [Out] char[] data_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_WriteProtect_4428(IntPtr hDev, short offset, short length, byte[] data_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_WriteProtect_4428_hex(IntPtr hDev, short offset, short length, char[] data_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Write_4428_hex(IntPtr hDev, short offset, short length, char[] data_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_4428(IntPtr hDev, short offset, short length, [Out] byte[] data_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_4428(IntPtr hDev, short offset, short length, byte[] data_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_4428_hex(IntPtr hDev, short offset, short length, [Out] char[] data_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_24c01(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_24c02(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_24c04(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_24c08(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_24c16(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_24c64(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_CPU(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_Card(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_24c(IntPtr hDev, short offset, short length, [Out] byte[] receive_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Read_24c_hex(IntPtr hDev, short offset, short length, [Out] char[] receive_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_24c(IntPtr hDev, short offset, short length, byte[] snd_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Write_24c_hex(IntPtr hDev, short offset, short length, byte[] snd_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_24c64(IntPtr hDev, short offset, short length, [Out] byte[] receive_buffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Read_24c64_hex(IntPtr hDev, short offset, short length, [Out] char[] receive_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_24c64(IntPtr hDev, short offset, short length, byte[] snd_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_24c64_hex(IntPtr hDev, short offset, short length, char[] snd_buffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_102(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Down_102(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_102(IntPtr hDev, byte offset, byte length, [Out] byte[] readdata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Read_102_hex(IntPtr hDev, byte offset, byte length, [Out] char[] readdata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_102(IntPtr hDev, byte offset, byte length, byte[] writedata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Write_102_hex(IntPtr hDev, byte offset, byte length, char[] writedata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CheckPass_102(IntPtr hDev, short zone, byte[] password);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CheckPass_102_hex(IntPtr hDev, short zone, char[] password);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ChangePass_102(IntPtr hDev, short zone, byte[] password);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ChangePass_102_hex(IntPtr hDev, short zone, char[] password);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadCount_102(IntPtr hDev, short zone);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Fuse_102(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_1604(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Down_1604(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_1604(IntPtr hDev, uint offset, uint length, [Out] byte[] readdata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Read_1604_hex(IntPtr hDev, uint offset, uint length, [Out] char[] readdata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_1604(IntPtr hDev, uint offset, uint length, byte[] writedata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Write_1604_hex(IntPtr hDev, uint offset, uint length, char[] writedata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CheckPass_1604(IntPtr hDev, short zone, byte[] password);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CheckPass_1604_hex(IntPtr hDev, short zone, char[] password);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ChangePass_1604(IntPtr hDev, short zone, byte[] password);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_ChangePass_160_hex(IntPtr hDev, short zone, char[] password);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_ReadCount_1604(IntPtr hDev, short zone);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Fuse_1604(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_1608(IntPtr hDev, byte zone, uint offset, uint length, [Out] byte[] readdata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Read_1608_hex(IntPtr hDev, byte zone, uint offset, uint length, [Out] char[] readdata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_1608(IntPtr hDev, byte zone, uint offset, uint length, byte[] writedata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Write_1608_hex(IntPtr hDev, byte zone, uint offset, uint length, char[] writedata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CheckPass_1608(IntPtr hDev, byte zone, byte type, byte[] password);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CheckPass_1608_hex(IntPtr hDev, byte zone, byte type, char[] password);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Init_Auth_1608(IntPtr hDev, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Init_Auth_1608_hex(IntPtr hDev, char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_Auth_1608(IntPtr hDev, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Check_Auth_1608_hex(IntPtr hDev, char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_Fuse_1608(IntPtr hDev, byte value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_Fuse_1608(IntPtr hDev, [Out] byte[] value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_153(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Down_153(IntPtr hDev);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_153(IntPtr hDev, byte zone, uint offset, uint length, [Out] byte[] readdata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_153_hex(IntPtr hDev, byte zone, uint offset, uint length, byte[] readdata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_153(IntPtr hDev, byte zone, uint offset, uint length, byte[] writedata);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Write_153_hex(IntPtr hDev, byte zone, uint offset, uint length, char[] writedata);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_CheckPass_153(IntPtr hDev, byte zone, byte type, byte[] password);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_CheckPass_153_hex(IntPtr hDev, byte zone, byte type, char[] password);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Init_Auth_153(IntPtr hDev, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Init_Auth_153_hex(IntPtr hDev, char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Check_Auth_153(IntPtr hDev, byte[] databuffer);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Check_Auth_153_hex(IntPtr hDev, char[] databuffer);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_Fuse_153(IntPtr hDev, byte value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_Fuse_153(IntPtr hDev, [Out] byte[] value);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Card_fm11rf005(IntPtr hDev, byte _Mode, ref UInt32 _Snr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_GetSnr_fm11rf005(IntPtr hDev, ref UInt32 _Snr);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_GetSnr_fm11rf005_hex(IntPtr hDev, [Out] char[] snrstr);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Read_fm11rf005(IntPtr hDev, byte _Adr, byte[] _Data);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Read_fm11rf005_hex(IntPtr hDev, byte _Adr, [Out] char[] _Data);

    [DllImport("SeaorySDK.dll")]
    public static extern short SOY_SC_Write_fm11rf005(IntPtr hDev, byte _Adr, byte[] _Data);

    [DllImport("SeaorySDK.dll", CharSet = CharSet.Ansi)]
    public static extern short SOY_SC_Write_fm11rf005_hex(IntPtr hDev, byte _Adr, [Out] char[] _Data);
  }
}
