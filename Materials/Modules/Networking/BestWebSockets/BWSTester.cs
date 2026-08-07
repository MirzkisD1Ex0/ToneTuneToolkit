using System.Collections;
using System.Collections.Generic;
using ToneTuneToolkit.Networking.WebSocket;
using UnityEngine;

namespace ToneTuneToolkit.Networking.WebSocket
{
  public class BWSTester : MonoBehaviour
  {
    private void Start()
    {
      WSClient.Instance.SetServerInfo("192.168.1.17", "1000");
      WSClient.Instance.SwitchClient(true);

      WSClient.OnStringReceive += WhenStringReceive;
      WSClient.OnBinaryReceive += WhenBinaryReceive;
    }

    private void OnDestroy()
    {
      WSClient.OnStringReceive -= WhenStringReceive;
      WSClient.OnBinaryReceive -= WhenBinaryReceive;
    }

    // ==================================================


    [ContextMenu(nameof(SendTestString))]
    public void SendTestString() => WSClient.Instance.SendWSMessage("TESTMESSAGE");



    // 测试带编号的图片：和上面那个只发裸字节流的版本互不干扰
    [SerializeField] private int testImageCode = 1;
    [SerializeField] private Texture2D t2dTest;
    [ContextMenu(nameof(SendTestT2D))]
    public void SendTestT2D()
    {
      byte[] bytes = t2dTest.EncodeToPNG();
      byte[] packet = WSCustomBinaryPacket.Pack(testImageCode, bytes);
      WSClient.Instance.SendWSMessage(packet);
      Debug.Log($"[Tester] Sent image with code={testImageCode}, size={bytes.Length} bytes.");
    }

    // ==================================================

    private void WhenStringReceive(string value)
    {
      Debug.Log(value);
    }

    // 收到带编号的图片包：自动解包成 (code, imageBytes)
    private void WhenBinaryReceive(byte[] rawBytes)
    {
      var (code, imageBytes) = WSCustomBinaryPacket.Unpack(rawBytes);
      if (imageBytes == null)
      {
        // 不是 WSImagePacket 格式（可能是旧版裸字节流图），按你的原始逻辑兜底
        Debug.LogWarning($"[Tester] Received binary frame ({rawBytes.Length} bytes) is NOT a WSImagePacket. Treating as raw image.");
        return;
      }

      Debug.Log($"[Tester] Received WSImagePacket: code={code}, image size={imageBytes.Length} bytes.");

      // 示例：还原成 Texture2D 并显示出来（实际业务按需处理）
      var tex = new Texture2D(2, 2);
      if (tex.LoadImage(imageBytes))
      {
        Debug.Log($"[Tester] Image decoded OK: {tex.width}x{tex.height}");
        // Destroy(tex) 由业务方决定何时释放
      }
      else
      {
        Debug.LogError($"[Tester] Image decode FAILED for code={code}.");
        Destroy(tex);
      }
    }
  }
}