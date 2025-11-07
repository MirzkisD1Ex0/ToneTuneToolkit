/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.2
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ToneTuneToolkit.Common;
using UnityEngine.Events;
using Newtonsoft.Json;
using System;

namespace ToneTuneToolkit.Networking
{
  /// <summary>
  /// 对志城宝具
  /// </summary>
  public class Upload2ZCManager : SingletonMaster<Upload2ZCManager>
  {
    #region 2025.10 MichelinCIIE

    public static UnityAction<Texture2D> OnMichelinUploadFinished;

    private const string MichelinUPLOADURL = @"https://michelin-ciie.studiocapsule.cn/api/index/aiUpload";



    [SerializeField] private MichelinUserInfo mUserInfo = new MichelinUserInfo();
    [SerializeField] private MichelinRespon mRespon = new MichelinRespon();
    public void UpdateMUserInfo(string gender, string question_1, Texture2D t2d)
    {
      mUserInfo = new MichelinUserInfo();
      mUserInfo.gender = gender;
      mUserInfo.question_1 = question_1;
      mUserInfo.file = t2d.EncodeToPNG();
    }

    public void UploadMUserInfo() => StartCoroutine(nameof(UploadMUserInfoAction));
    private IEnumerator UploadMUserInfoAction()
    {
      Debug.Log("[U2ZCM] 开始上传");
      WWWForm wwwForm = new WWWForm();
      wwwForm.AddField("gender", mUserInfo.gender);
      wwwForm.AddField("question_1", mUserInfo.question_1);
      wwwForm.AddBinaryData("file", mUserInfo.file);

      using (UnityWebRequest www = UnityWebRequest.Post(MichelinUPLOADURL, wwwForm))
      {
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
          Debug.LogWarning($"[U2ZCM] {www.error}");
          yield break;
        }

        Debug.Log($"[U2ZCM] {www.downloadHandler.text}");
        try
        {
          mRespon = JsonConvert.DeserializeObject<MichelinRespon>(www.downloadHandler.text);
        }
        catch (Exception)
        {
          Debug.LogWarning($"[U2ZCM] 解析错误");
          yield break;
        }

        if (mRespon.code != 0)
        {
          Debug.LogWarning($"[U2ZCM] 解析错误");
          yield break;
        }
      }

      // 搞图
      using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(mRespon.data.qr_url)) // new UnityWebRequest(sunCodeURL, "GET"))
      {
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
          Debug.LogWarning($"[U2ZCM] {unityWebRequest.error}");
        }
        else
        {
          Debug.Log($"[U2ZCM] Get qr texture sucessed");
          OnMichelinUploadFinished?.Invoke(((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture); // 返回图
        }
      }
    }

    // ==================================================
    // 数据类
    [Serializable]
    public class MichelinUserInfo
    {
      public string gender;
      public string question_1;
      public byte[] file;
    }

    [Serializable]
    public class MichelinRespon
    {
      public int code;
      public string message;
      public MichelinResponData data;
    }
    public class MichelinResponData
    {
      public string qr_url;
    }

    #endregion
    // ==================================================
    // ==================================================
    // ==================================================
    #region 2025.10 LonginesBoutique

    // public static UnityAction<int> OnLBUploadFinished;

    // private const string LonginesBoutiqueUPLOADURL = @"https://longines-ai.studiocapsule.cn/api/index/upload";



    // [SerializeField] private LBUserInfo lbUserInfo = new LBUserInfo();
    // [SerializeField] private LBRespon lbRespon = new LBRespon();
    // public void UpdateLBUserInfo(string gender, string question_1, Texture2D t2d)
    // {
    //   lbUserInfo = new LBUserInfo();
    //   lbUserInfo.gender = gender;
    //   lbUserInfo.question_1 = question_1;
    //   lbUserInfo.file = t2d.EncodeToPNG();
    // }

    // public void UploadLBUserInfo() => StartCoroutine(nameof(UploadLBUserInfoAction));
    // private IEnumerator UploadLBUserInfoAction()
    // {
    //   Debug.Log("[U2ZCM] 开始上传");
    //   WWWForm wwwForm = new WWWForm();
    //   wwwForm.AddField("gender", lbUserInfo.gender);
    //   wwwForm.AddField("question_1", lbUserInfo.question_1);
    //   wwwForm.AddBinaryData("file", lbUserInfo.file);

    //   using (UnityWebRequest www = UnityWebRequest.Post(LonginesBoutiqueUPLOADURL, wwwForm))
    //   {
    //     www.downloadHandler = new DownloadHandlerBuffer();
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log($"[U2ZCM] {www.error}");
    //       yield break;
    //     }

    //     Debug.Log($"[U2ZCM] {www.downloadHandler.text}");
    //     try
    //     {
    //       lbRespon = JsonConvert.DeserializeObject<LBRespon>(www.downloadHandler.text);
    //     }
    //     catch (Exception)
    //     {
    //       Debug.Log($"[U2ZCM] 解析错误");
    //       yield break;
    //     }

    //     if (lbRespon.code != 0)
    //     {
    //       OnLBUploadFinished?.Invoke(0000);
    //       yield break;
    //     }

    //     OnLBUploadFinished?.Invoke(lbRespon.data.look_pwd);
    //   }
    // }

    // // ==================================================
    // // 数据类
    // [Serializable]
    // public class LBUserInfo
    // {
    //   public string gender;
    //   public string question_1;
    //   public byte[] file;
    // }

    // [Serializable]
    // public class LBRespon
    // {
    //   public int code;
    //   public string message;
    //   public LBResponData data;
    // }
    // public class LBResponData
    // {
    //   public int look_pwd;
    // }

    #endregion
    // ==================================================
    // ==================================================
    // ==================================================
    #region 2025.06 VWAIPhoto

    // public static UnityAction<Texture2D, bool> OnVWAvatarFinished;

    // private const string VWSUBMITURL = @"https://vw-v-space.studiocapsule.cn/api/device/submitTask";

    // public void UpgradeVWUserData(string gender, string car, string address, Texture2D file)
    // {
    //   uVWUD.gender = gender;
    //   uVWUD.car = car;
    //   uVWUD.address = address;
    //   uVWUD.file = file.EncodeToPNG();
    //   return;
    // }

    // // ==================================================
    // // 提交生成任务

    // public void SubmitVWUserPhoto() => StartCoroutine(nameof(SubmitVWUserPhotoAction));
    // private IEnumerator SubmitVWUserPhotoAction()
    // {
    //   WWWForm wwwForm = new WWWForm();
    //   wwwForm.AddField("gender", uVWUD.gender);
    //   wwwForm.AddField("car", uVWUD.car);
    //   wwwForm.AddField("address", uVWUD.address);
    //   wwwForm.AddBinaryData("file", uVWUD.file);

    //   using (UnityWebRequest www = UnityWebRequest.Post(VWSUBMITURL, wwwForm))
    //   {
    //     www.downloadHandler = new DownloadHandlerBuffer();
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log($"[U2ZCM] {www.error}");
    //       yield break;
    //     }

    //     Debug.Log($"[U2ZCM] {www.downloadHandler.text}");
    //     try
    //     {
    //       rVWUD = JsonConvert.DeserializeObject<Respon_VWUserData>(www.downloadHandler.text);
    //     }
    //     catch
    //     {
    //       OnVWAvatarFinished?.Invoke(null, false);
    //     }
    //     if (rVWUD.code != 0)
    //     {
    //       Debug.Log($"[U2ZCM] Code Error");
    //       yield break;
    //     }

    //     StartQueryTask(); // 轮询直到出答案
    //   }
    //   yield break;
    // }



    // private Upload_VWUserData uVWUD = new Upload_VWUserData();
    // [SerializeField] private Respon_VWUserData rVWUD = new Respon_VWUserData();

    // [Serializable]
    // public class Upload_VWUserData
    // {
    //   public string gender;
    //   public string car;
    //   public string address;
    //   public byte[] file;
    // }

    // [Serializable]
    // public class Respon_VWUserData
    // {
    //   public int code;
    //   public Respon_VWUserDataData data;
    // }

    // [Serializable]
    // public class Respon_VWUserDataData
    // {
    //   public string task_code;
    // }

    // // ==================================================
    // // 轮询

    // private int queryCount = 0;
    // private const string VWQUERYURL = @"https://vw-v-space.studiocapsule.cn/api/device/queryTask";

    // public void StartQueryTask() { StartCoroutine(nameof(QueryTaskAction)); }
    // public void StopQueryTask() { StopCoroutine(nameof(QueryTaskAction)); }
    // private IEnumerator QueryTaskAction()
    // {
    //   yield return new WaitForSeconds(5f); // 先等5秒
    //   while (true)
    //   {
    //     queryCount++;

    //     if (queryCount > 12)
    //     {
    //       Debug.Log($"[U2ZCM] 轮询次数过多，停止查询");
    //       OnVWAvatarFinished?.Invoke(null, false);
    //       yield break;
    //     }

    //     WWWForm wwwForm = new WWWForm();
    //     wwwForm.AddField("task_code", rVWUD.data.task_code);
    //     using (UnityWebRequest www = UnityWebRequest.Post(VWQUERYURL, wwwForm))
    //     {
    //       www.downloadHandler = new DownloadHandlerBuffer();
    //       yield return www.SendWebRequest();

    //       if (www.result != UnityWebRequest.Result.Success)
    //       {
    //         Debug.Log($"[U2ZCM] {www.error}");
    //         yield break;
    //       }

    //       Debug.Log($"[U2ZCM] {www.downloadHandler.text}");
    //       rVWAD = JsonConvert.DeserializeObject<Respon_VWAvatarData>(www.downloadHandler.text);

    //       if (rVWUD.code != 0)
    //       {
    //         Debug.Log($"[U2ZCM] Code Error");
    //         OnVWAvatarFinished?.Invoke(null, false);
    //         yield break;
    //       }

    //       switch (rVWAD.data.status)
    //       {
    //         default: break;
    //         case 3:
    //           queryCount = 0;
    //           Debug.Log("[U2ZCM] 开始下载图片");
    //           StartCoroutine(nameof(DownloadAvatarAction), rVWAD.data.avatar_url); // 下载图片
    //           yield break;
    //         case 4:
    //           OnVWAvatarFinished?.Invoke(null, false);
    //           break;
    //       }

    //       yield return new WaitForSeconds(5f);
    //     }
    //   }
    // }



    // [SerializeField] private Respon_VWAvatarData rVWAD = new Respon_VWAvatarData();

    // [Serializable]
    // public class Respon_VWAvatarData
    // {
    //   public int code;
    //   public string message;
    //   public Respon_VWAvatarDataData data;
    // }

    // [Serializable]
    // public class Respon_VWAvatarDataData
    // {
    //   public string avatar_url;
    //   public int status;
    //   public string status_text;
    // }

    // // ==================================================
    // // 获取图片

    // private IEnumerator DownloadAvatarAction(string url)
    // {
    //   using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
    //   {
    //     yield return www.SendWebRequest();
    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log($"[U2ZCM] {www.error}");
    //       yield break;
    //     }

    //     debug_peekQRCode = DownloadHandlerTexture.GetContent(www); // DEBUG
    //     OnVWAvatarFinished?.Invoke(DownloadHandlerTexture.GetContent(www), true);
    //   }
    //   yield break;
    // }

    // // ==================================================
    // // 上传图片并获取二维码地址

    // private const string VWUPLOADURL = @"https://vw-v-space.studiocapsule.cn/api/device/finalUpload";

    // public void UploadVWResult(Texture2D value) => StartCoroutine(nameof(UploadVWResultAction), value);
    // private IEnumerator UploadVWResultAction(Texture2D t2dResult)
    // {
    //   WWWForm wwwForm = new WWWForm();
    //   wwwForm.AddField("task_code", rVWUD.data.task_code);
    //   wwwForm.AddBinaryData("file", t2dResult.EncodeToJPG(), "t2d.jpg", "image/jpeg");

    //   using (UnityWebRequest www = UnityWebRequest.Post(VWUPLOADURL, wwwForm))
    //   {
    //     www.downloadHandler = new DownloadHandlerBuffer();
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log($"[U2ZCM] {www.error}");
    //       yield break;
    //     }

    //     Debug.Log($"[U2ZCM] {www.downloadHandler.text}");

    //     rVWRD = JsonConvert.DeserializeObject<Respon_VWResultData>(www.downloadHandler.text);

    //     if (rVWRD.code != 0)
    //     {
    //       Debug.Log($"[U2ZCM] Code Error");
    //       yield break;
    //     }

    //     DownloadQRCode(rVWRD.data.qr_url);
    //   }
    //   yield break;
    // }



    // [SerializeField] private Respon_VWResultData rVWRD = new Respon_VWResultData();

    // [Serializable]
    // public class Respon_VWResultData
    // {
    //   public int code;
    //   public string message;
    //   public Respon_VWResultDataData data;
    // }
    // [Serializable]
    // public class Respon_VWResultDataData
    // {
    //   public string qr_url;
    //   public string file_url;
    // }

    #endregion
    // ==================================================
    // ==================================================
    // ==================================================
    #region 2025.04 NikeADT

    // public static UnityAction<UserInfo> OnUserInfoDownloaded;

    // private const string USERINFOREQUESTURL = @"https://nike-adt.studiocapsule.cn/api/device/scanQr";

    // /// <summary>
    // /// 获取用户信息
    // /// </summary>
    // /// <param name="url"></param>
    // public void GetUserInfo(string url) => StartCoroutine(nameof(GetUserInfoAction), url);
    // private IEnumerator GetUserInfoAction(string url)
    // {
    //   WWWForm wwwForm = new WWWForm();
    //   wwwForm.AddField("qr_content", url);

    //   using (UnityWebRequest www = UnityWebRequest.Post(USERINFOREQUESTURL, wwwForm))
    //   {
    //     www.downloadHandler = new DownloadHandlerBuffer();
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log($"[U2ZCM] {www.error}");
    //       yield break;
    //     }

    //     Debug.Log($"[U2ZCM] {www.downloadHandler.text}");
    //     try
    //     {
    //       uird = JsonConvert.DeserializeObject<Respon_UserInfoData>(www.downloadHandler.text);
    //     }
    //     catch (Exception)
    //     {
    //       Debug.Log($"[U2ZCM] 解析错误");
    //       yield break;
    //     }

    //     if (uird.code != 0)
    //     {
    //       if (OnUserInfoDownloaded != null)
    //       {
    //         OnUserInfoDownloaded(null);
    //       }
    //       yield break;
    //     }

    //     if (OnUserInfoDownloaded != null)
    //     {
    //       UserInfo ui = new UserInfo();
    //       ui.name = uird.data.name;
    //       ui.code = uird.data.code;
    //       ui.save_car = uird.data.save_car;
    //       ui.can_play = uird.data.can_play;
    //       OnUserInfoDownloaded(ui);
    //     }
    //   }
    //   yield break;
    // }

    // private Respon_UserInfoData uird = new Respon_UserInfoData();

    // [Serializable]
    // public class Respon_UserInfoData
    // {
    //   public int code;
    //   public string message;
    //   public UserInfo data;
    // }

    // [Serializable]
    // public class UserInfo
    // {
    //   public string name;
    //   public string code;
    //   public string save_car;
    //   public string can_play;
    // }

    // // ==================================================

    // private const string USERIMAGEUPLOADURL = @"https://nike-adt.studiocapsule.cn/api/device/upload";

    // public static UnityAction<string> OnUserImageUploaded;

    // [SerializeField] private Upload_UserImageData uUID;
    // public void UpdateUserImageData(string user_code, Texture2D t2dPhoto)
    // {
    //   uUID = new Upload_UserImageData();
    //   uUID.user_code = user_code;

    //   byte[] fileBytes = t2dPhoto.EncodeToPNG();
    //   uUID.file = fileBytes;
    //   return;
    // }

    // public void UploadUserImage() => StartCoroutine(nameof(UploadUserImageAction));
    // private IEnumerator UploadUserImageAction()
    // {
    //   WWWForm wwwForm = new WWWForm();
    //   wwwForm.AddField("user_code", uUID.user_code);
    //   wwwForm.AddBinaryData("file", uUID.file);

    //   using (UnityWebRequest www = UnityWebRequest.Post(USERIMAGEUPLOADURL, wwwForm))
    //   {
    //     www.downloadHandler = new DownloadHandlerBuffer();
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log($"[U2ZCM] {www.error}");
    //       yield break;
    //     }

    //     Debug.Log($"[U2ZCM] {www.downloadHandler.text}");
    //     rUID = JsonConvert.DeserializeObject<Respon_UserImageData>(www.downloadHandler.text);

    //     if (rUID.code != 0)
    //     {
    //       Debug.Log($"[U2ZCM] Code Error");
    //       yield break;
    //     }

    //     DownloadQRCode(rUID.data.qr_url); // 搞图
    //   }
    //   yield break;
    // }

    // public class Upload_UserImageData
    // {
    //   public string user_code;
    //   public byte[] file;
    // }

    // private Respon_UserImageData rUID = new Respon_UserImageData();

    // [Serializable]
    // public class Respon_UserImageData
    // {
    //   public int code;
    //   public string message;
    //   public Respon_UserImageDataData data;
    // }

    // [Serializable]
    // public class Respon_UserImageDataData
    // {
    //   public string qr_url;
    // }

    #endregion
    // ==================================================
    // ==================================================
    #region 获取QR图片

    // public static UnityAction<Texture2D> OnQRImageDownloaded;

    // [SerializeField] private Texture2D debug_peekQRCode;

    // public void DownloadQRCode(string url) => StartCoroutine(nameof(DownloadQRCodeAction), url);
    // private IEnumerator DownloadQRCodeAction(string url)
    // {
    //   using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
    //   {
    //     yield return www.SendWebRequest();
    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //       Debug.Log($"[U2ZCM] {www.error}");
    //       yield break;
    //     }

    //     debug_peekQRCode = DownloadHandlerTexture.GetContent(www); // DEBUG
    //     OnQRImageDownloaded?.Invoke(DownloadHandlerTexture.GetContent(www));
    //   }
    //   yield break;
    // }

    #endregion
  }
}
