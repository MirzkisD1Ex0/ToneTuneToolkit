using System;
using System.IO;
using UnityEngine;
using ToneTuneToolkit.Media;
using ToneTuneToolkit.Networking.WebSocket;

public class Tester : MonoBehaviour
{
  private void Start() => Init();
  private void OnDestroy() => UnInit();

  // ==================================================

  private void Init()
  {
    WSServer.Instance.OnTextReceived += WhenTextReceived;
    WSServer.Instance.OnCustomDataReceived += WhenCustomDataReceived;
    WSServer.Instance.OnImageReceived += HandleImage;
  }

  private void UnInit()
  {
    WSServer.Instance.OnTextReceived -= WhenTextReceived;
    WSServer.Instance.OnCustomDataReceived -= WhenCustomDataReceived;
    WSServer.Instance.OnImageReceived -= HandleImage;
  }

  // ==================================================

  private void WhenTextReceived(string sessionID, string value)
  {
    Debug.Log(@$"Message: {value}, from {sessionID}");
    WSServer.Instance.SendTextToSession(sessionID, "I got your message.");
  }



  private void WhenCustomDataReceived(string sessionID, byte type, int code, byte[] payload)
  {
    Debug.Log($"[Test] CustomData session={sessionID} type=0x{type:X2} code={code} payloadLen={payload?.Length ?? 0}");
  }

  private void HandleImage(string sessionID, int code, byte[] imageBytes)
  {
    Debug.Log($"[Test] Image session={sessionID} code={code} bytes={imageBytes?.Length ?? 0}");

    if (imageBytes == null || imageBytes.Length == 0) return;

    Texture2D tex = new Texture2D(2, 2);
    if (!tex.LoadImage(imageBytes))
    {
      Debug.LogError($"[Test] 图片还原失败 (code={code})");
      Destroy(tex);
      return;
    }

    string dir = Path.Combine(Application.streamingAssetsPath, "Images");
    Directory.CreateDirectory(dir);
    string path = Path.Combine(dir, $"{DateTime.Now:yyyyMMdd_HHmmss_fff}_{code}.png");

    ScreenshotMaster.SaveTexture2File(tex, path);
    Destroy(tex);

    WSServer.Instance.SendTextToSession(sessionID, "I got your image.");
  }
}