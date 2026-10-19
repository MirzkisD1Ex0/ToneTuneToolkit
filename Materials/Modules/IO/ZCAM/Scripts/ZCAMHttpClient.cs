/// <summary>
/// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
/// Z CAM HTTP 控制客户端：命令串行化（唯一会话持有者）、JSON 解析、二进制下载
/// 注意：MJPEG 取景流是长连接，严禁走本客户端，否则会阻塞所有控制命令
/// </summary>

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ToneTuneToolkit.IO.ZCAM;
using UnityEngine;
using UnityEngine.Networking;

namespace ToneTuneToolkit.IO.ZCAM
{
  /// <summary>
  /// Unity 2022.3 的 UnityWebRequestAsyncOperation 不带 GetAwaiter（2023.1 才内置），
  /// 这里补一个事件驱动的 awaiter，避免每帧轮询。
  /// </summary>
  internal static class ZCAMUnityWebRequestAwaiter
  {
    public static TaskAwaiter GetAwaiter(this UnityWebRequestAsyncOperation op)
    {
      var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
      op.completed += _ => tcs.SetResult(null);
      return ((Task)tcs.Task).GetAwaiter();
    }
  }

  /// <summary>
  /// HTTP API 薄封装。控制命令经 SemaphoreSlim 串行下发，
  /// 相机侧同一时刻只有一个控制客户端，队列化保证命令不交叉。
  /// </summary>
  public class ZCAMHttpClient : IDisposable
  {
    private readonly string _baseUrl;
    private readonly float _timeoutSeconds;
    private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);
    private bool _disposed = false;

    public ZCAMHttpClient(string cameraIp, float timeoutSeconds = 5f)
    {
      _baseUrl = "http://" + cameraIp;
      _timeoutSeconds = timeoutSeconds;
    }

    // ================================ 基础请求 ================================

    /// <summary>
    /// JsonUtility 对非法 JSON（空串 / HTML 错误页）会抛裸 ArgumentException 绕过模块异常契约，
    /// 这里统一转成 ZCAMException 并附响应预览，便于定位。
    /// </summary>
    private static T ParseJson<T>(string body, string pathForError) where T : class
    {
      try
      {
        return JsonUtility.FromJson<T>(body);
      }
      catch (Exception e)
      {
        var preview = body == null ? "(null)" : (body.Length > 200 ? body.Substring(0, 200) + "..." : body);
        throw new ZCAMException($"响应不是合法 JSON [{pathForError}]: {e.Message} | 响应预览: {preview}");
      }
    }

    /// <summary>
    /// 发送 GET 并解析为 ZCAMResponse；HTTP 失败或 code!=0 时抛 ZCAMException
    /// </summary>
    public async Task<ZCAMResponse> GetAsync(string pathAndQuery)
    {
      var body = await GetRawAsync(pathAndQuery);
      var resp = ParseJson<ZCAMResponse>(body, pathAndQuery);
      if (resp.code != 0)
      {
        throw new ZCAMException($"相机返回错误 [{pathAndQuery}]: {resp.msg}", resp.code, resp.desc);
      }
      return resp;
    }

    /// <summary>
    /// 发送 GET 并返回原始响应体（供动态结构解析，如目录列表、参数元数据）
    /// </summary>
    public async Task<string> GetRawAsync(string pathAndQuery)
    {
      await _gate.WaitAsync();
      try
      {
        if (_disposed) throw new ZCAMException("ZCAMHttpClient 已释放");
        using (var req = UnityWebRequest.Get(_baseUrl + pathAndQuery))
        {
          req.timeout = Mathf.CeilToInt(_timeoutSeconds);
          await req.SendWebRequest();

          if (req.result != UnityWebRequest.Result.Success)
          {
            // HTTP 409 = 会话被其他客户端占用
            if (req.responseCode == 409)
            {
              throw new ZCAMException("会话被其他客户端占用（HTTP 409）", 409);
            }
            throw new ZCAMException($"请求失败 [{pathAndQuery}]: {req.error}");
          }
          return req.downloadHandler.text;
        }
      }
      finally
      {
        _gate.Release();
      }
    }

    /// <summary>
    /// 发送 GET 并下载二进制内容（照片、缩略图等）。
    /// 不走命令队列：大文件下载不应阻塞控制命令。
    /// </summary>
    public async Task<byte[]> GetBytesAsync(string pathAndQuery, float timeoutSeconds = 30f)
    {
      if (_disposed) throw new ZCAMException("ZCAMHttpClient 已释放");
      using (var req = UnityWebRequest.Get(_baseUrl + pathAndQuery))
      {
        req.timeout = Mathf.CeilToInt(timeoutSeconds);
        req.downloadHandler = new DownloadHandlerBuffer();
        await req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
        {
          throw new ZCAMException($"下载失败 [{pathAndQuery}]: {req.error}");
        }
        return req.downloadHandler.data;
      }
    }

    // ================================ 高级命令 ================================

    /// <summary>GET /info：探测相机并返回信息（兼作 ping）</summary>
    public async Task<ZCAMCameraInfo> GetInfoAsync()
    {
      var body = await GetRawAsync("/info");
      var payload = ParseJson<ZCAMInfoPayload>(body, "/info");
      if (string.IsNullOrEmpty(payload.model))
      {
        throw new ZCAMException($"无法解析相机信息: {body}");
      }
      return new ZCAMCameraInfo
      {
        Model = payload.model,
        Firmware = payload.sw,
        Mac = payload.mac,
        EthIp = payload.eth_ip,
        Sn = payload.sn,
      };
    }

    /// <summary>抢占控制会话（普通抢占，非 occupy 独占）</summary>
    public Task GrabSessionAsync()
    {
      return GetAsync("/ctrl/session");
    }

    /// <summary>释放控制会话</summary>
    public Task QuitSessionAsync()
    {
      return GetAsync("/ctrl/session?action=quit");
    }

    /// <summary>连接时对时（官方建议每次连接都同步一次）</summary>
    public Task SyncDateTimeAsync()
    {
      var now = DateTime.Now;
      return GetAsync($"/datetime?date={now:yyyy-MM-dd}&time={now:HH:mm:ss}");
    }

    /// <summary>查询工作模式，返回原始字符串：rec / rec_ing / pb / standby 等</summary>
    public async Task<string> QueryModeAsync()
    {
      var resp = await GetAsync("/ctrl/mode?action=query");
      return resp.msg;
    }

    /// <summary>切换工作模式：to_rec / to_pb / to_standby / exit_standby</summary>
    public Task ChangeModeAsync(string action)
    {
      return GetAsync($"/ctrl/mode?action={action}");
    }

    /// <summary>开始录像（含延时录像）</summary>
    public Task StartRecordAsync()
    {
      return GetAsync("/ctrl/rec?action=start");
    }

    /// <summary>停止录像</summary>
    public Task StopRecordAsync()
    {
      return GetAsync("/ctrl/rec?action=stop");
    }

    /// <summary>格式化存储卡（不可逆！清空全部照片与视频）。文件系统由相机按卡容量自动决定</summary>
    public Task FormatCardAsync()
    {
      return GetAsync("/ctrl/card?action=format");
    }

    /// <summary>删除卡内文件（不可逆）。调用方需确保本地已留存副本</summary>
    public Task DeleteFileAsync(string remotePath)
    {
      return GetAsync($"{remotePath}?act=rm");
    }

    /// <summary>拍照：按当前 movfmt 抓帧，文件落在 DCIM</summary>
    public Task CaptureStillAsync()
    {
      return GetAsync("/ctrl/still?action=cap");
    }

    /// <summary>写参数：GET /ctrl/set?key=value</summary>
    public Task SetParamAsync(string key, string value)
    {
      return GetAsync($"/ctrl/set?{key}={Uri.EscapeDataString(value)}");
    }

    /// <summary>
    /// 读参数：返回原始响应体。
    /// /ctrl/get 的元数据挂在动态键名下（data.&lt;key&gt;.type/ro/opts...），
    /// JsonUtility 解不了动态键，P0 暴露原文，参数系统阶段再引 Newtonsoft。
    /// </summary>
    public Task<string> GetParamRawAsync(string key)
    {
      return GetRawAsync($"/ctrl/get?k={key}");
    }

    /// <summary>
    /// 读参数当前值（/ctrl/get 响应的 "value":"..." 字段），如 movfmt → "4KP29.97"。
    /// </summary>
    public async Task<string> GetParamValueAsync(string key)
    {
      var raw = await GetRawAsync($"/ctrl/get?k={key}");
      const string marker = "\"value\":\"";
      int s = raw.IndexOf(marker, StringComparison.Ordinal);
      if (s < 0)
      {
        throw new ZCAMException($"参数 {key} 响应无 value 字段: {raw.Substring(0, Math.Min(200, raw.Length))}");
      }
      s += marker.Length;
      int e = raw.IndexOf('"', s);
      return raw.Substring(s, e - s);
    }

    /// <summary>拍照：按当前 movfmt 抓帧；成功时 msg 可能直接带新文件路径（/tmp/mnt/sata/DCIM/...），否则为空</summary>
    public async Task<string> CaptureStillPathAsync()
    {
      var resp = await GetAsync("/ctrl/still?action=cap");
      return resp.msg ?? string.Empty;
    }

    // ================================ 文件管理 ================================

    /// <summary>
    /// 列出 DCIM 下全部文件夹名，如 100ZCAME。
    /// 注意：部分固件可能不返回 code 字段（DTO 默认 -1），故只在「无 files 且 code 明确非 0」时报错，
    /// 避免把合法的空列表误判为失败。
    /// </summary>
    public async Task<string[]> ListDcimFoldersAsync()
    {
      var body = await GetRawAsync("/DCIM/");
      var list = ParseJson<ZCAMFileListResponse>(body, "/DCIM/");
      EnsureListHealthy(list, "/DCIM/");
      return list.files ?? new string[0];
    }

    /// <summary>列出某文件夹内全部文件名</summary>
    public async Task<string[]> ListFilesAsync(string folder)
    {
      var body = await GetRawAsync($"/DCIM/{folder}");
      var list = ParseJson<ZCAMFileListResponse>(body, $"/DCIM/{folder}");
      EnsureListHealthy(list, $"/DCIM/{folder}");
      return list.files ?? new string[0];
    }

    private static void EnsureListHealthy(ZCAMFileListResponse list, string pathForError)
    {
      if (list.files == null && list.code != 0)
      {
        throw new ZCAMException($"目录列表返回错误 [{pathForError}]: {list.desc}", list.code, list.desc);
      }
    }

    /// <summary>下载文件原始字节（?act=thm / ?act=scr 直接拼在 pathAndQuery 上）</summary>
    public Task<byte[]> DownloadFileAsync(string pathAndQuery, float timeoutSeconds = 30f)
    {
      return GetBytesAsync(pathAndQuery, timeoutSeconds);
    }

    public void Dispose()
    {
      if (_disposed) return;
      _disposed = true;
      _gate.Dispose();
    }
  }
}
