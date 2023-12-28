using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniOSC;

namespace MartellController
{
  public class UniOSCReceiver : UniOSCEventTarget
  {
    public override void OnOSCMessageReceived(UniOSCEventArgs args)
    {
      AnalyseMessage(args);
      return;
    }

    private void AnalyseMessage(UniOSCEventArgs args)
    {
      switch (args.Address)
      {
        default: break;

        case "/callback/resetscene": // 重加载场景
          SceneManager.LoadScene("Scene");
          break;

      }
    }
  }
}