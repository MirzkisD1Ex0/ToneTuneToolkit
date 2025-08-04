/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using ToneTuneToolkit.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace ToneTuneToolkit.Networking
{
  /// <summary>
  /// Json上传
  /// </summary>
  public class JsonUploadManager : SingletonMaster<JsonUploadManager>
  {
    private const string URL = "https://ai.digital-event.cn/api/ai/chat";

    public static UnityAction<string> OnSentenceComplete;

    // ==================================================

    // private void Update()
    // {
    //   if (Input.GetKeyUp(KeyCode.Q))
    //   {
    //     UploadJson(new UploadJsonData() { message = "fox" });
    //   }
    // }

    // ==================================================

    [SerializeField] private UploadJsonData uploadJson = new UploadJsonData();
    [SerializeField] private ResponJsonData responJson;

    [Serializable]
    public class UploadJsonData
    {
      public string message;
    }
    [Serializable]
    public class ResponJsonData
    {
      public bool status;
      public string message;
      public string data;
    }

    // ==================================================

    public void UploadJson(UploadJsonData value)
    {
      uploadJson = new UploadJsonData();
      uploadJson = value;
      StartCoroutine(nameof(UploadJsonAction));
      return;
    }

    private IEnumerator UploadJsonAction()
    {
      string json = JsonConvert.SerializeObject(uploadJson);

      using (UnityWebRequest www = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbPOST))
      {
        // www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        byte[] bytes = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bytes);

        DownloadHandler downloadHandler = new DownloadHandlerBuffer();
        www.downloadHandler = downloadHandler;

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
          Debug.Log($"{www.error}...<color=red>[ER]</color>");
          yield break;
        }
        else
        {
          // Debug.Log(www.downloadHandler.text);
          responJson = new ResponJsonData();
          responJson = JsonConvert.DeserializeObject<ResponJsonData>(www.downloadHandler.text.ToString());
          if (OnSentenceComplete != null)
          {
            OnSentenceComplete(responJson.data);
          }
        }
      }
      yield break;
    }
  }
}
