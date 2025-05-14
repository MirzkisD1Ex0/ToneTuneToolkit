using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.Networking;
using ToneTuneToolkit.Common;

public class RemoveBGManager : SingletonMaster<RemoveBGManager>
{
  public static UnityAction<Texture2D, int> OnRemoveBGFinished;

  private const string REMOVEBGAPIURL = "https://api.remove.bg/v1.0/removebg";
  private const string APIKEY = "76YHaSA8WZYmbZXfqfBeYbqy";

  // ==================================================

  [SerializeField] private Texture2D t2dOrigin;
  [SerializeField] private Texture2D t2dResult;
  [SerializeField] private int flag;

  public void UpdatePackage(Texture2D value, int flagValue)
  {
    t2dOrigin = value;
    flag = flagValue;
    return;
  }

  // ==================================================

  /// <summary>
  /// 上传照片至RemoveBG
  /// </summary>
  public void StartRemoveBG() => StartCoroutine(nameof(RemoveBGAction));
  private IEnumerator RemoveBGAction()
  {
    WWWForm wwwForm = new WWWForm();
    wwwForm.AddBinaryData("image_file", t2dOrigin.EncodeToPNG());
    wwwForm.AddField("size", "full");
    wwwForm.AddField("type", "person");
    wwwForm.AddField("format", "png");

    using (UnityWebRequest uwr = UnityWebRequest.Post(REMOVEBGAPIURL, wwwForm))
    {
      uwr.SetRequestHeader("X-Api-Key", APIKEY);

      yield return uwr.SendWebRequest();

      if (uwr.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError(@$"[RBGM] {uwr.error}");
        Debug.LogError(@$"[RBGM] {uwr.downloadHandler.text}");
        yield break;
      }

      try
      {
        Debug.Log(uwr.downloadHandler.data);
        Debug.Log(uwr.downloadHandler.data.Length);

        byte[] bytes = uwr.downloadHandler.data;
        Texture2D t2d = new Texture2D(2, 2);
        t2d.LoadImage(bytes);

        t2dResult = t2d;
        if (OnRemoveBGFinished != null)
        {
          OnRemoveBGFinished(t2d, flag);
        }
      }
      catch { }
    }
    yield break;
  }

  // ==================================================
  // 数据类

  private ResultJson resultJson;

  [Serializable]
  public class ResultJson
  {
    public ResultDataJson data;
  }
  [Serializable]
  public class ResultDataJson
  {
    public string result_b64;
    public int foreground_top;
    public int foreground_left;
    public int foreground_width;
    public int foreground_height;
  }
}