// ssp_test.cpp — zcam_ssp 第二阶段验证宿主：轮询解码后的 RGBA 帧，
// 首帧落盘 BMP 供人工/程序检视，用相机侧帧号差计算真实帧率
#include <chrono>
#include <cstdio>
#include <cstring>
#include <cstdint>
#include <thread>
#include <vector>

typedef struct ZSspCtx *ZSspHandle;
extern "C"
{
ZSspHandle zssp_create(const char *ip, unsigned short port, unsigned bufMb);
int zssp_start(ZSspHandle);
int zssp_stop(ZSspHandle);
void zssp_destroy(ZSspHandle);
int zssp_poll_frame(ZSspHandle, unsigned char *dst, int dstCap, int *outW, int *outH,
                    unsigned long long *outPts, unsigned int *outFrmNo);
int zssp_has_first_frame(ZSspHandle);
unsigned int zssp_dropped_aus(ZSspHandle);
const char *zssp_last_error(ZSspHandle);
}

// 32bpp BGRA 顶向下 BMP（行宽 w*4 恒为 4 的倍数，无行填充）
static void WriteBmp(const char *path, const uint8_t *rgba, int w, int h)
{
#pragma pack(push, 1)
  struct FileHdr
  {
    uint16_t magic;
    uint32_t size;
    uint32_t reserved;
    uint32_t offset;
  };
  struct InfoHdr
  {
    uint32_t size;
    int32_t width;
    int32_t height;
    uint16_t planes;
    uint16_t bpp;
    uint32_t compression;
    uint32_t imageSize;
    int32_t xppm, yppm;
    uint32_t colorsUsed, colorsImportant;
  };
#pragma pack(pop)

  FileHdr fh{};
  fh.magic = 0x4D42; // 'BM'
  fh.size = (uint32_t)(sizeof(FileHdr) + sizeof(InfoHdr) + (size_t)w * h * 4);
  fh.offset = (uint32_t)(sizeof(FileHdr) + sizeof(InfoHdr));

  InfoHdr ih{};
  ih.size = sizeof(InfoHdr);
  ih.width = w;
  ih.height = -h; // 负值 = 顶向下，与 RGBA 行序一致
  ih.planes = 1;
  ih.bpp = 32;
  ih.imageSize = (uint32_t)((size_t)w * h * 4);

  FILE *f = nullptr;
  if (fopen_s(&f, path, "wb") != 0 || !f) return;
  fwrite(&fh, sizeof(fh), 1, f);
  fwrite(&ih, sizeof(ih), 1, f);

  std::vector<uint8_t> row((size_t)w * 4);
  for (int y = 0; y < h; y++)
  {
    const uint8_t *src = rgba + (size_t)y * w * 4;
    for (int x = 0; x < w; x++)
    {
      row[x * 4 + 0] = src[x * 4 + 2]; // B
      row[x * 4 + 1] = src[x * 4 + 1]; // G
      row[x * 4 + 2] = src[x * 4 + 0]; // R
      row[x * 4 + 3] = 255;
    }
    fwrite(row.data(), row.size(), 1, f);
  }
  fclose(f);
}

int main(int argc, char **argv)
{
  const char *ip = argc > 1 ? argv[1] : "10.98.32.1";
  std::vector<uint8_t> buf; // 按 -2 反馈的尺寸动态扩容

  ZSspHandle h = zssp_create(ip, 9999, 4);
  if (!h) { printf("create failed\n"); return 1; }
  if (zssp_start(h) != 0) { printf("start failed\n"); return 1; }
  printf("connecting to %s:9999 (decoding via Media Foundation)...\n", ip);

  auto t0 = std::chrono::steady_clock::now();
  unsigned frmFirst = 0, frmLast = 0;
  int samples = 0, bmpSaved = 0, resizeCount = 0;
  int lastW = 0, lastH = 0;

  for (int i = 0; i < 100; i++)
  {
    std::this_thread::sleep_for(std::chrono::milliseconds(100));
    int w = 0, hgt = 0;
    unsigned long long pts = 0;
    unsigned frm = 0;
    for (;;)
    {
      int n = zssp_poll_frame(h, buf.empty() ? nullptr : buf.data(), (int)buf.size(), &w, &hgt, &pts, &frm);
      if (n == -2)
      {
        buf.resize((size_t)w * hgt * 4); // 缓冲不够：按告知尺寸扩容后重试（帧仍在槽内）
        resizeCount++;
        continue;
      }
      if (n <= 0) break; // 0=无新帧，其他负值=错误
      if (samples == 0) frmFirst = frm;
      frmLast = frm;
      samples++;
      lastW = w; lastH = hgt;
      if (bmpSaved == 0)
      {
        // 像素活性检查（避免全黑/全白假帧）+ 首帧落盘
        uint32_t sum = 0;
        for (int p = 0; p < n / 4 && p < 40000; p++) sum += buf[p * 4] + buf[p * 4 + 1] + buf[p * 4 + 2];
        printf("frame#%u pts=%llu %dx%d first-frame-pixel-sum=%u\n", frm, pts, w, hgt, sum);
        WriteBmp("decode_first_frame.bmp", buf.data(), w, hgt);
        bmpSaved = 1;
      }
    }
  }

  auto secs = std::chrono::duration<double>(std::chrono::steady_clock::now() - t0).count();
  const char *err = zssp_last_error(h);
  printf("---- result ----\n");
  printf("decoded=%d  frm# %u -> %u  span=%.1fs  size=%dx%d  resizes=%d\n",
         samples, frmFirst, frmLast, secs, lastW, lastH, resizeCount);
  if (frmLast > frmFirst)
    printf("camera-side fps = %.1f\n", (frmLast - frmFirst) / secs);
  printf("bmp saved: %s\n", bmpSaved ? "decode_first_frame.bmp" : "NO (no decoded frame)");
  printf("dropped AUs: %u (should be 0)\n", zssp_dropped_aus(h));
  if (err && err[0]) printf("error: %s\n", err);
  else printf("no error\n");

  zssp_stop(h);
  zssp_destroy(h);
  return (samples > 0) ? 0 : 2;
}
