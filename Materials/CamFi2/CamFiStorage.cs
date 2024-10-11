using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.Data;

namespace MartellGroupPhoto
{
  public class CamFiStorage : MonoBehaviour
  {
    private static string configPath = "/camficonfig.json";

    public static string CamFiIP = "192.168.50.101";
    private static string CamFiPort = "80";
    private static string CamFiEventPort = "8080";
    public static string CamFiLiveViewPort = "890";

    public static string CamFiRESTAddress { get { return $"http://{CamFiIP}:{CamFiPort}"; } }
    public static string CamFiEventAddress { get { return $"http://{CamFiIP}:{CamFiEventPort}/socket.io/"; } }
    public static string CamFiLiveViewAddress { get { return $"http://{CamFiIP}:{CamFiLiveViewPort}"; } }


    public static string Live { get { return $"{CamFiRESTAddress}/live"; } }
    public static string TakePic { get { return $"{CamFiRESTAddress}/takepic/true"; } }
    public static string GetFileList { get { return $"{CamFiRESTAddress}/files/0/4"; } }
    public static string GetRawFile { get { return $"{CamFiRESTAddress}/raw/"; } }
    public static string StartLiveView { get { return $"{CamFiRESTAddress}/capturemovie"; } }
    public static string StopLiveView { get { return $"{CamFiRESTAddress}/stopcapturemovie"; } }

    // ==================================================

    private void Awake()
    {
      Init();
    }

    // ==================================================

    private void Init()
    {
      configPath = Application.streamingAssetsPath + configPath;
      CamFiIP = JsonManager.GetJson(configPath, "CamFi IP");
      return;
    }
  }
}