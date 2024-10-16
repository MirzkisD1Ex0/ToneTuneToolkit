using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// using OpenCVSharp;
using OpenCVForUnity;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ObjdetectModule;

/// <summary>
/// 面部识别
/// </summary>
public class FacialRecognitionHandler : MonoBehaviour
{
  private Mat gray; // 灰度图，方便识别
  private Mat rotatedNewMat;
  private MatOfRect faceRect; // 识别到的人脸的区域
  private CascadeClassifier classifier; // 人脸识别分类器
  private string cascadePath = Application.streamingAssetsPath + "/haarcascade_frontalface_alt2.xml";

  [SerializeField] private float timer = 0;

  // ==================================================

  private void Start() => Init();
  private void OnDestroy() => Dispose();

  // ==================================================

  private void Init()
  {
    // LogAllWebCam();
    gray = new Mat(); // 初始化Mat
    faceRect = new MatOfRect(); // 初始化识别到的人脸的区域
    classifier = new CascadeClassifier(cascadePath); // 初始化人脸识别分类器

    previewMat = new Mat();
    resultPreviewTexture2D = new Texture2D(440, 440, TextureFormat.RGBA32, false);
    return;
  }

  // ==================================================
  #region 资源释放

  private void Dispose()
  {
    if (rotatedNewMat != null)
    {
      rotatedNewMat.Dispose();
      rotatedNewMat = null;
    }

    if (previewMat != null)
    {
      previewMat.Dispose();
      previewMat = null;
    }
    if (resultPreviewTexture2D != null)
    {
      Destroy(resultPreviewTexture2D);
      resultPreviewTexture2D = null;
    }
    return;
  }
  #endregion

  // ==================================================
  #region 面部识别
  public void DetectFace(Mat rgbaMat)
  {
    // rotatedNewMat = MatRotate(rgbaMat.clone()); // 旋转原数据
    rotatedNewMat = rgbaMat.clone(); // 原数据

    Imgproc.cvtColor(rotatedNewMat, gray, Imgproc.COLOR_RGBA2GRAY); // 将获取到的摄像头画面转化为灰度图并赋值给gray

    // mat/面部矩形向量组/识别精度越高越快越不准/面部识别次数2次以上算识别/?性能优化/最小检测尺寸/最大检测尺寸
    // classifier.detectMultiScale(gray, faceRect, 1.1d, 2, 2, new Size(20, 20), new Size()); // 检测gray中的人脸
    classifier.detectMultiScale(gray, faceRect, 1.1d, 3, 2, new Size(20, 20), new Size());

    OpenCVForUnity.CoreModule.Rect[] rects = faceRect.toArray();
    if (rects.Length > 0)
    {
      for (int i = 0; i < rects.Length; i++)
      {
        Imgproc.rectangle(rotatedNewMat, new Point(rects[i].x, rects[i].y), new Point(rects[i].x + rects[i].width, rects[i].y + rects[i].height), new Scalar(0, 255, 0, 255), 2);  //在原本的画面中画框，框出人脸额位置,其中rects[i].x和rects[i].y为框的左上角的顶点，rects[i].width、rects[i].height即为框的宽和高
      }

      timer += Time.deltaTime;
      if (timer > .2f)
      {
        Debug.Log("检测到面部");
      }
    }
    else
    {
      // if (timer > 0) // 缓慢减少检测值
      // {
      //   timer -= Time.deltaTime / 2;
      //   if (timer < 0)
      //   {
      //     timer = 0;
      //   }
      // }
      timer = 0;
    }

    // 深复制识别、画框数据并显示
    UpdateResultPreview(rotatedNewMat);
    return;
  }
  #endregion

  // ==================================================
  #region 预览画面
  [SerializeField] private Image resultPreviewImage;
  private Mat previewMat;
  private Texture2D resultPreviewTexture2D;
  private void UpdateResultPreview(Mat value)
  {
    if (resultPreviewImage == null)
    {
      return;
    }

    previewMat = value.clone();
    Utils.matToTexture2D(previewMat, resultPreviewTexture2D);
    resultPreviewImage.sprite = Sprite.Create(resultPreviewTexture2D, new UnityEngine.Rect(0, 0, 440, 440), Vector2.zero);
    return;
  }
  #endregion

  // ==================================================

  /// <summary>
  /// Mat旋转方法
  /// </summary>
  /// <param name="orginalMat"></param>
  /// <returns></returns>
  private Mat MatRotate(Mat orginalMat)
  {
    Mat rotatedMat = new Mat(orginalMat.height(), orginalMat.width(), CvType.CV_8UC4, new Scalar(0, 0, 0, 255)); // DEBUG:宽高可能相反

    // mat旋转
    Point center = new Point(orginalMat.cols() / 2f, orginalMat.rows() / 2f);
    Mat mat = Imgproc.getRotationMatrix2D(center, 90, 1);
    Imgproc.warpAffine(orginalMat, rotatedMat, mat, orginalMat.size());

    return rotatedMat;
  }

  /// <summary>
  /// 预览有多少相机
  /// </summary>
  private void LogAllWebCam()
  {
    Debug.Log($"Found {WebCamTexture.devices.Length} device...<color=green>[OK]</color>");
    foreach (WebCamDevice webcamDevice in WebCamTexture.devices)
    {
      Debug.Log($"{webcamDevice.name}...<color=green>[OK]</color>");
    }
    return;
  }
}