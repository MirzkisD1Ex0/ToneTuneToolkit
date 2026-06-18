/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.6.0
/// </summary>

using UnityEngine;
using System.IO;
using ToneTuneToolkit.Data;

namespace ToneTuneToolkit.IO.CamFi
{
  public class Configer
  {
    private static readonly string configPath = Path.Combine(Application.streamingAssetsPath, "ToneTuneToolkit", "Configs", "CamFi", "camficonfig.json");

    public static string CamFiIP { get; private set; } = "192.168.50.101";
    public static string CamFiPort { get; private set; } = "80";
    public static string CamFiEventPort { get; private set; } = "8080";
    public static string CamFiLiveViewPort { get; private set; } = "890";

    public static string CamFiRESTAddress { get { return $"http://{CamFiIP}:{CamFiPort}"; } }
    public static string CamFiEventAddress { get { return $"http://{CamFiIP}:{CamFiEventPort}/socket.io/"; } }
    public static string CamFiLiveViewAddress { get { return $"http://{CamFiIP}:{CamFiLiveViewPort}"; } }

    public static string Live { get { return $"{CamFiRESTAddress}/live"; } }
    public static string TakePic { get { return $"{CamFiRESTAddress}/takepic/true"; } }
    public static string GetFileList { get { return $"{CamFiRESTAddress}/files/0/4"; } } // 读取第0张到第4张
    public static string GetRawFile { get { return $"{CamFiRESTAddress}/raw/"; } } // 末尾要加路径，从GetFileList获得
    public static string StartLiveView { get { return $"{CamFiRESTAddress}/capturemovie"; } }
    public static string StopLiveView { get { return $"{CamFiRESTAddress}/stopcapturemovie"; } }

    // ==================================================

    static Configer()
    {
      Init();
    }

    // ==================================================

    private static void Init()
    {
      CamFiIP = LitJsonManager.GetJson(configPath, "camfi_ip").ToString();
      CamFiPort = LitJsonManager.GetJson(configPath, "camfi_port").ToString();
    }
  }
}
