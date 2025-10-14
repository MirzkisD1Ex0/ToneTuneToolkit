/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using UnityEngine;
using System.Net;
using System.IO;

namespace ToneTuneToolkit.IO
{
  /// <summary>
  /// FTP下载(上传暂无)工具
  /// 需要服务器假设FTP服务并创建用户
  /// FileZilla_Server-cn-0_9_60_2.exe
  /// </summary>
  public class FTPMaster : MonoBehaviour
  {
    private string ftpFilePath = @"ftp://192.168.50.152/" + "SandTableAB/terrain 0x0";
    private string localFileSavePath = Application.streamingAssetsPath + "/SandTableAB/" + "terrain 0x0";
    public string ftpUserName = "tt";
    private string ftpPassword = "tt";

    // ==================================================

    private void Start()
    {
      // DownloadFile(ftpFilePath, ftpUserName, ftpPassword, localFileSavePath);
    }

    // ==================================================

    public static void DownloadFile(string ftpFilePath, string ftpUserName, string ftpPassword, string localFileSavePath)
    {
      try
      {
        // 创建连接
        FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(ftpFilePath);
        ftpWebRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
        ftpWebRequest.KeepAlive = true;
        ftpWebRequest.UseBinary = true;
        ftpWebRequest.Proxy = null;
        ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

        // 创建响应
        FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

        Stream stream = ftpWebResponse.GetResponseStream();

        using (FileStream fileStream = File.Create(localFileSavePath))
        {
          byte[] bytes = new byte[2048];
          int contentLength = stream.Read(bytes, 0, bytes.Length);

          while (contentLength != 0)
          {
            fileStream.Write(bytes, 0, contentLength);
            contentLength = stream.Read(bytes, 0, bytes.Length);
          }
          fileStream.Close();
          stream.Close();
        }
        Debug.Log("[FTPMaster] <color=green>File downloaded</color>...[Done]");
      }
      catch
      {
      }
      return;
    }
  }
}
