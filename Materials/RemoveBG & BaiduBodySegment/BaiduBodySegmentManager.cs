using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Events;
using ToneTuneToolkit.Common;

public class BaiduBodySegmentManager : SingletonMaster<BaiduBodySegmentManager>
{
  public static UnityAction<Texture2D, int> OnSegmentFinished;

  #region Info
  private const string CLIENTID = @"2fClRTA6uqf8WMMs3oYetrtN";
  private const string CLIENTSECRET = @"9K1HQItadDrPdFizDJkRh5bzWwi1O1tJ";
  private const string TOKENURL = @"https://aip.baidubce.com/oauth/2.0/token";
  private const string BODYSEGURL = @"https://aip.baidubce.com/rest/2.0/image-classify/v1/body_seg?access_token=";
  private string token = @"25.0acc4e48d0f7450dd320126240dbaa7c.315360000.2037861152.282335-101570444"; // 后续会Get // 可以用一个月
  #endregion

  private TokenJson tokenJson;
  private ResultJson resultJson;

  // ==================================================
  #region 上传包

  [SerializeField] private Texture2D t2dOrigin;
  [SerializeField] private Texture2D t2dResult;
  [SerializeField] private int flag;

  public void UpdatePackage(Texture2D value, int flagValue)
  {
    t2dOrigin = value;
    flag = flagValue;
    return;
  }

  #endregion
  // ==================================================

  /// <summary>
  /// 人像分割
  /// </summary>
  public void StartBodySegment() => StartCoroutine(nameof(BodySegmentAction));
  private IEnumerator BodySegmentAction()
  {
    #region GetToken // 获取Token
    string url = $"{TOKENURL}?client_id={CLIENTID}&client_secret={CLIENTSECRET}&grant_type=client_credentials";
    using (UnityWebRequest uwr = UnityWebRequest.PostWwwForm(url, ""))
    {
      uwr.SetRequestHeader("Content-Type", "application/json");
      uwr.SetRequestHeader("Accept", "application/json");
      yield return uwr.SendWebRequest();

      if (uwr.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError(@$"[BBSM] {uwr.error}");
        yield break;
      }

      try
      {
        tokenJson = JsonConvert.DeserializeObject<TokenJson>(uwr.downloadHandler.text);
        token = tokenJson.access_token;
      }
      catch
      {
        Debug.Log("[BBSM] Token Analyze Failed.");
        RetryBodySegment();
      }
    }
    #endregion


    #region BodySegment // 人像分割
    WWWForm form = new WWWForm();
    form.AddField("image", Texture2Base64(t2dOrigin));

    using (UnityWebRequest uwr = UnityWebRequest.Post(BODYSEGURL + token, form))
    {
      uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
      yield return uwr.SendWebRequest();

      if (uwr.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError(@$"[BBSM] {uwr.error}");
        yield break;
      }

      try
      {
        resultJson = JsonConvert.DeserializeObject<ResultJson>(uwr.downloadHandler.text);
        string foregroundBase64 = resultJson.foreground;

        if (!string.IsNullOrEmpty(foregroundBase64)) // 判断是否有图
        {
          Texture2D result = Base642Texture(foregroundBase64);
          t2dResult = result;
          // 保存至本地?
          retryTime = 0;
          if (OnSegmentFinished != null)
          {
            OnSegmentFinished(result, flag);
          }
        }
        else
        {
          // 重拍???
          Debug.LogError("[BBSM] Error foreground image null");
          retryTime = 0;
          if (OnSegmentFinished != null)
          {
            OnSegmentFinished(null, flag); // 没拍到 // 传空的回去
          }
        }
      }
      catch
      {
        Debug.Log("[BBSM] Image Analyze Failed.");
        RetryBodySegment();
      }
      #endregion
      yield break;
    }
  }

  private int retryTime = 0;
  private const int RETRYTIMELIMIT = 3;
  private void RetryBodySegment() => StartCoroutine(nameof(RetryBodySegmentAction));
  private IEnumerator RetryBodySegmentAction()
  {
    if (retryTime >= RETRYTIMELIMIT)
    {
      yield break;
    }
    yield return new WaitForSeconds(3f);
    retryTime++;
    Debug.Log($"[BBSM] Retry {retryTime} time(s).");
    StartBodySegment();
    yield break;
  }

  // ==================================================
  // 工具类

  /// <summary>
  /// 贴图转Base64
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string Texture2Base64(Texture2D value)
  {
    if (value == null)
    {
      return null;
    }
    Texture2D texture2d = new Texture2D(value.width, value.height, TextureFormat.RGBA32, false);
    texture2d.SetPixels(value.GetPixels());
    texture2d.Apply();
    byte[] bytes = texture2d.EncodeToPNG();
    string base64String = Convert.ToBase64String(bytes);
    return base64String;
  }

  /// <summary>
  /// Base64转贴图
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static Texture2D Base642Texture(string value)
  {
    if (value == null)
    {
      return null;
    }
    byte[] bytes = Convert.FromBase64String(value);
    Texture2D texture2d = new Texture2D(1, 1);
    texture2d.LoadImage(bytes);
    return texture2d;
  }

  // /// <summary>
  // /// 保存至本地
  // /// </summary>
  // private void Save2Local(Texture2D value)
  // {
  //   string path = @$"{Application.dataPath}/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-{new System.Random().Next(0, 100)}.png";
  //   byte[] bytes = value.EncodeToPNG();
  //   File.WriteAllBytes(path, bytes);
  //   Debug.Log(path);
  //   return;
  // }

  // ==================================================
  // 数据类

  [Serializable]
  public class TokenJson
  {
    public string refresh_token;
    public int expires_in;
    public string session_key;
    public string access_token;
    public string scope;
    public string session_secret;
  }

  [Serializable]
  public class ResultJson
  {
    public string log_id;
    public string labelmap;
    public string scoremap;
    public string foreground;
    public string person_num;
    public object person_info;
  }
}