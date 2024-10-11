using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Events;

public class BaiduBodySegmentManager : MonoBehaviour
{
  public static BaiduBodySegmentManager Instance;

  private const string CLIENTID = @"ltiCIE7Rq17Nt2MH77LX6Qmv";
  private const string CLIENTSECRET = @"fjSdI4zFd9QjfFTWymf1sXKQrjzy0UjH";
  private const string TOKENURL = @"https://aip.baidubce.com/oauth/2.0/token";
  private const string BODYSEGURL = @"https://aip.baidubce.com/rest/2.0/image-classify/v1/body_seg?access_token=";
  private string token = @"25.0acc4e48d0f7450dd320126240dbaa7c.315360000.2037861152.282335-101570444"; // 后续会Get // 可以用一个月

  [SerializeField] private Texture2D texture2dOriginalPhoto;
  [SerializeField] private Texture2D texture2dResultPhoto;
  private TokenJson tokenJson;
  private ResultJson resultJson;

  public static event UnityAction<Texture2D> OnResultCallback;

  // ==================================================

  private void Awake() => Instance = this;

  // private void Update()
  // {
  //   if (Input.GetKeyUp(KeyCode.U))
  //   {
  //     string testPath = @"D:\2024-06-08 00.33.12.1717777992216_myPic_0.jpg";
  //     preuploadTexture = TextureProcessor.ReadTexture(testPath);
  //     preuploadTexture = TextureProcessor.RotateTexture(preuploadTexture, false);
  //     preuploadTexture = TextureProcessor.HorizontalFlipTexture(preuploadTexture);
  //     preuploadTexture = TextureProcessor.ScaleTexture(preuploadTexture, preuploadTexture.width * .7f, preuploadTexture.height * .7f);
  //     preuploadTexture.Apply();
  //     UploadPhoto2Baidu(preuploadTexture);
  //   }
  // }

  // ==================================================

  /// <summary>
  /// 更新原图
  /// </summary>
  /// <param name="value"></param>
  public void UpdateOriginalPhotoTexture2D(Texture2D value)
  {
    texture2dOriginalPhoto = value;
    return;
  }


  /// <summary>
  /// 人像分割
  /// </summary>
  public void StartBodySegment() => StartCoroutine(nameof(BodySegmentAction));
  private IEnumerator BodySegmentAction()
  {
    #region GetToken // 获取Token
    string url = $"{TOKENURL}?client_id={CLIENTID}&client_secret={CLIENTSECRET}&grant_type=client_credentials";
    using (UnityWebRequest request = UnityWebRequest.Post(url, ""))
    {
      request.SetRequestHeader("Content-Type", "application/json");
      request.SetRequestHeader("Accept", "application/json");
      yield return request.SendWebRequest();

      if (request.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError("[BBSM] Error " + request.error);
        yield break;
      }

      tokenJson = JsonConvert.DeserializeObject<TokenJson>(request.downloadHandler.text);
      token = tokenJson.access_token;
    }
    #endregion


    #region BodySegment // 人像分割
    string base64 = Texture2Base64(texture2dOriginalPhoto);

    WWWForm form = new WWWForm();
    form.AddField("image", base64);

    using (UnityWebRequest request = UnityWebRequest.Post(BODYSEGURL + token, form))
    {
      request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
      yield return request.SendWebRequest();

      if (request.result != UnityWebRequest.Result.Success)
      {
        Debug.LogError("[BBSM] Error " + request.error);
        yield break;
      }

      // Debug.Log(request.downloadHandler.text);
      resultJson = JsonConvert.DeserializeObject<ResultJson>(request.downloadHandler.text);
      string foregroundBase64 = resultJson.foreground;

      if (!string.IsNullOrEmpty(foregroundBase64)) // 判断是否有图
      {
        texture2dResultPhoto = Base642Texture(foregroundBase64);
        if (OnResultCallback != null)
        {
          OnResultCallback(texture2dResultPhoto);
        }
      }
      else
      {
        // 重拍???
        Debug.LogError("[BBSM] Error foreground image null");
        texture2dResultPhoto = null;
        if (OnResultCallback != null)
        {
          OnResultCallback(null); // 没拍到 // 传空的回去
        }
      }
      #endregion
      yield break;
    }
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