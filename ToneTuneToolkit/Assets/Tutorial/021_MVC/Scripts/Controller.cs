using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
  public static Controller Instance;

  // ==================================================

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    Init();
  }

  private void OnDestroy()
  {
    View.Instance.InputField.onEndEdit.RemoveListener(Model.Instance.UpdateTheFuckingData);
  }

  // ==================================================

  private void Init()
  {
    // 监听view组件
    View.Instance.InputField.onEndEdit.AddListener(Model.Instance.UpdateTheFuckingData);

    // 监听model变化
    Model.Instance.AddEventListener(
    (value) =>
    {
      View.Instance.UpdateText(value);
    });
    return;
  }
}