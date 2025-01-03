using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System;

public class JsonUploader.cs : BaseSceneManager<JsonUploader>
{
  private const string authURL = @"http://192.168.10.211:4050/auth/student";

  // ==================================================

  public void Auth(string value) => StartCoroutine(AuthAction(value));
  private IEnumerator AuthAction(string value)
  {
    AuthData authData = new AuthData();
    authData.internalId = value;
    string json = JsonConvert.SerializeObject(authData);

    // 上传json的方法
    // 上传bytes否则jsonbody会被误作为head
    using (UnityWebRequest www = new UnityWebRequest(authURL, UnityWebRequest.kHttpVerbPOST))
    {
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
        Debug.Log(www.downloadHandler.text);
        ResponAuthData responAuthData = JsonConvert.DeserializeObject<ResponAuthData>(www.downloadHandler.text.ToString());

        if (responAuthData.code == 0)
        {
          PlayerManager.Instance.SetPlayerPrefsStudentID(value);
        }
        else
        {

        }

      }
    }
    yield break;
  }

  // ==================================================

  [Serializable]
  public class AuthData
  {
    public string internalId;
  }
  [Serializable]
  public class ResponAuthData
  {
    public int code;
    public string data;
  }
}