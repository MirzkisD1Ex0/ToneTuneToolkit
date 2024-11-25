using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;

public class SimplyUploadManager : MonoBehaviour
{
  public static SimplyUploadManager Instance;

  private const string uploadURL = @"https://open.skyelook.com/api/device/saveScore";

  // ==================================================

  private void Awake()
  {
    Instance = this;
  }

  // ==================================================

  private UploadData uploadData;
  public void SetUploadData(UploadData value)
  {
    uploadData = value;
    return;
  }

  // ==================================================

  public void StartUploadData() => UploadDataAction();
  private IEnumerator UploadDataAction()
  {
    WWWForm wwwForm = new WWWForm();
    wwwForm.AddField("uuid", uploadData.uuid);
    wwwForm.AddField("clock", uploadData.clock);
    wwwForm.AddField("start_code", uploadData.start_code);
    wwwForm.AddField("game_score", uploadData.game_score);

    using (UnityWebRequest www = UnityWebRequest.Post(uploadURL, wwwForm))
    {
      www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
      www.downloadHandler = new DownloadHandlerBuffer();
      yield return www.SendWebRequest();
      if (www.result != UnityWebRequest.Result.Success)
      {
        Debug.Log($"{www.error}...<color=red>[ER]</color>");
        yield break;
      }
      else
      {
        // StatusData = JsonConvert.DeserializeObject<UploadData>(www.downloadHandler.text);
        Debug.Log($"{www.downloadHandler.text}...<color=green>[OK]</color>");
      }
    }
    yield break;
  }

  [Serializable]
  public class UploadData
  {
    public string uuid;
    public string clock;
    public string start_code;
    public string game_score;
  }
}