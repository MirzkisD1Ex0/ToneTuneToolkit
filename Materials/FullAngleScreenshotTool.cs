using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using ToneTuneToolkit.Common;
using ToneTuneToolkit.Media;

namespace FordBeyond
{
  /// <summary>
  /// 全角度截图工具
  /// </summary>
  public class FullAngleScreenshotTool : MonoBehaviour
  {
    public GameObject GO;
    public GameObject ShotCamera;
    public int ShotTime = 4;

    private string _basepath = Application.streamingAssetsPath + "/Cars/";


    private void Update()
    {


      if (Input.GetKeyDown(KeyCode.Q))
      {
        StartCoroutine("Begin");
      }
    }



    private IEnumerator Begin()
    {
      // 判断有多少文件夹在目录中
      DirectoryInfo di = new DirectoryInfo(_basepath);
      FileInfo[] files = di.GetFiles("*", SearchOption.TopDirectoryOnly);
      int folderIndex = files.Length + 1;
      string currentPath = _basepath + string.Format("{0:d4}", folderIndex) + "/";

      ShotCamera.transform.LookAt(GO.transform);

      for (int i = 0; i < ShotTime; i++)
      {
        GO.transform.rotation = Quaternion.Euler(0, 360 / ShotTime * i, 0);
        yield return new WaitForEndOfFrame();
        ScreenshotMaster.Instance.SaveRenderTexture(currentPath, string.Format("{0:d4}", i) + ".png");
      }
      yield break;
    }

    private void OnApplicationQuit()
    {
      StopAllCoroutines();
    }
  }
}