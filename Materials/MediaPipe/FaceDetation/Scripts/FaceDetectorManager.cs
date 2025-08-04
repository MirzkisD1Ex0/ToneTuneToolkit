using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToneTuneToolkit.Common;

using Mediapipe.Tasks.Core;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;
using Mediapipe.Unity.Sample.FaceDetection;

/// <summary>
/// 面部检测工具管理器
/// 延后初始化
/// 
/// 基本配置文件
/// ..\MediaPipeUnity\Samples\Scenes\AppSettings.asset
/// </summary>
public class FaceDetectorManager : SingletonMaster<FaceDetectorManager>
{
  [SerializeField] private FaceDetectorRunner runner;

  // ==================================================

  private void Start() => Init();

  // ==================================================

  private void Init() => StartCoroutine(nameof(DelayInit));
  private IEnumerator DelayInit()
  {
    yield return new WaitForSeconds(3f);
    InitImageSource();
    InitRunnerConfig();
    runner.Play();
  }

  // ==================================================
  #region 面部检测状态控制

  /// <summary>
  /// 开关暂停恢复检测
  /// </summary>
  /// <param name="value"></param>
  public void SwitchFaceDetector(FaceDetectorState value)
  {
    switch (value)
    {
      default: break;
      case FaceDetectorState.Play: runner.Play(); break;
      case FaceDetectorState.Stop: runner.Stop(); break;
      case FaceDetectorState.Pause: runner.Pause(); break;
      case FaceDetectorState.Resume: runner.Resume(); break;
    }
  }

  public enum FaceDetectorState
  {
    Play = 0, // 具备Reload功能
    Stop = 1, Pause = 2, Resume = 3
  }

  #endregion
  // ==================================================
  #region 画面源、面部检测参数初始化

  private void InitImageSource()
  {
    ImageSourceProvider.Switch(ImageSourceType.WebCamera);
    ImageSource imageSource = ImageSourceProvider.ImageSource;
    imageSource.SelectSource(0); // 0:BRIO 1:USBHD
    imageSource.SelectResolution(7); // 1280x720 30fps
    imageSource.isHorizontallyFlipped = false;
  }

  private void InitRunnerConfig()
  {
    runner.config.Delegate = BaseOptions.Delegate.CPU;
    runner.config.ImageReadMode = ImageReadMode.CPUAsync;
    runner.config.Model = ModelType.BlazeFaceShortRange;
    runner.config.RunningMode = Mediapipe.Tasks.Vision.Core.RunningMode.LIVE_STREAM;
    runner.config.MinDetectionConfidence = 0.4f; // 0.0 - 1.0 // 越高越不容易误检
    runner.config.MinSuppressionThreshold = 0.3f; // 0.0 - 1.0
    runner.config.NumFaces = 3; // 1-3
  }

  #endregion
}