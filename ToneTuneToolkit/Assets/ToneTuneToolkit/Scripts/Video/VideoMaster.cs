/// <summary>
/// Copyright (c) 2025 MirzkisD1Ex0 All rights reserved.
/// Code Version 1.5.1
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace ToneTuneToolkit.Video
{
  public class VideoMaster : MonoBehaviour
  {
    public static VideoMaster Instance;

    public List<VideoClip> VideoClips = new List<VideoClip>();

    private VideoPlayer videoPlayer;
    public VideoPlayer VideoPlayer
    {
      get { return videoPlayer; }
      set { }
    }

    // ==================================================

    private void Awake()
    {
      Instance = this;
      videoPlayer = GetComponent<VideoPlayer>();
    }

    // ==================================================

    /// <summary>
    /// 播放视频
    /// </summary>
    /// <param name="index"></param>
    public void PlayVideo(int index)
    {
      if (videoPlayer.isPlaying)
      {
        videoPlayer.Stop();
      }

      if (videoPlayer.isPaused)
      {
        videoPlayer.Play();
      }
      else
      {
        videoPlayer.clip = VideoClips[index];
        videoPlayer.Play();
      }
      return;
    }

    /// <summary>
    /// 显示第一帧
    /// </summary>
    /// <param name="index"></param>
    public void LoadVideo(int index)
    {
      videoPlayer.clip = VideoClips[index];
      videoPlayer.Play();
      videoPlayer.Pause();
      return;
    }

    // ==================================================

    /// <summary>
    /// 视频播放回调
    /// </summary>
    public void VideoCallback()
    {
      videoPlayer.loopPointReached += Callback;
    }
    private void Callback(VideoPlayer vp)
    {
      // videoPlayer.loopPointReached -= Callback;
      vp.loopPointReached -= Callback;
      return;
    }
  }
}
