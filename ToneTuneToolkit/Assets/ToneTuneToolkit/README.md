<font face="Source Han Sans TC" size=2 color=#FFFFFF>

#### <center><font size=2>Make everything f<font color="#FF0000">or</font>king simple.</font></center>
#### <center><font size=2>2025/10/14</font></center>
# <center><font color="#54FF9F" size=6>**Tone Tune Toolkit v1.5.2**</font></center>



<font size=5><strong>ToneTuneToolkit是什么?</strong></font><br>
一个致力于帮助Unity六边形战士减轻开发负担的项目。<br>
<s>但更多的时候是在帮助互动工程师偷懒。</s><br>

完成至少<strong>[1]</strong>个有些奇怪却十分好用的工具包：<br>
(√) 显现存在于Unity/C#中却不为人知的野路子<br>
(√) 添加需求简单但就是不想亲自编写的小功能<br>
<s>(×) 解决古怪且迷惑的开发需求</s><br>

<kbd>Ctrl</kbd> + <kbd>C</kbd><br>
<kbd>Ctrl</kbd> + <kbd>V</kbd><br>
<s>哈！逮到你了！</s><br>

<font size=5><strong>Directory - 文档目录</strong></font><br>
[INTRODUCTION　　介绍](#INTRODUCTION)<br>
[LOG　　　　　　　&ensp;日志](#LOG)<br>
[MODULE　　　　　&ensp;模组](#MODULE)<br>
[SCRIPTS　　　　　&ensp;脚本](#SCRIPTS)<br>
[EXTRA　　　　　　&ensp;额外内容](#EXTRA)<br>
[SHADERS　　　　　着色器](#SHADERS)<br>
[TEXTURES　　　　&ensp;贴图](#TEXTURES)<br>
[FONTS　　　　　　字体](#FONTS)<br>
[DEMOS　　　　　　示例](#DEMOS)<br>
[STORAGE　　　　　仓库](#STORAGE)<br>
[THIRDPARTY　　　&ensp;第三方](#THIRDPARTY)<br>
[TUTORIALS　　　　教程](#TUTORIALS)<br>
[CONTACT　　　　　联系](#CONTACT)<br>



## <center><a id="INTRODUCTION"></a>*INTRODUCTION*</center>
- 请留意，“MirzkisD1Ex0”的“ToneTune Toolkit”基于**GPL3.0**(GNU General Public License v3.0)协议所开发。(对，就是那个传染性极强的协议。)
- 工具包存在“**Assets/ToneTuneToolkit**”文件夹及“**Assets/StreamingAssets/ToneTuneToolkit**”文件夹，两部分内容。
- 当某一模块中包含“**Handler**”助手类时，通常添加助手类至对象即可自动为其添加依赖。避免发生错误的组装。例如“**UDP**”模块以及“**Verification**”模块。
- 添加了思源黑体简中OTF格式全套。



## <center><a id="LOG"></a>*LOG*</center>
0. 2021/09/06 添加了两张简易场景地板贴图。
0. 2021/09/06 添加了一些演示用场景。
0. 2021/09/06 添加了三个可怕的工具，在“**Assets/StreamingAssets**”中。
0. 2021/09/22 路径检查现在有更为醒目的提示。
0. 2021/09/23 添加了“Funny”命名空间，里面会存一些然并卵的鬼代码，比如冒泡排序，甚至还有冒泡排序的浮点型重载。添加了UDP响应器。
0. 2021/09/23 纠正了“PathChecker”中对文件夹路径检查的错误，更新了UDP和WOL非懒人方法的使用说明，移动了UDP消息接受体的位置。
0. 2021/09/24 为“LedHandler”添加了一个工具函数，可以根据输入的[-1f~0f~1f]生成[黄色~白色~蓝色]的Color。
0. 2021/10/11 添加了写入json的方法在“TextLoad”中。
0. 2021/11/10 添加了“CameraSimpleMove”，一个经典的场景漫游脚本，可以通过WSDA空格和LeftShift控制相机移动，按住鼠标右键以移动视角。
0. 2021/11/29 添加了AssetBundle包工具。
0. 2022/01/22 添加了“CorrectLookAtCamera”，一个使物体永远正对相机的脚本，改进了LookAt。
0. 2023/05/17 添加了“ObjectDragRotate”，拖动物体使其跟随鼠标旋转。
0. 2023/07/20 工具包结构巨幅整理。
0. 2023/07/21 添加了“FTPMaster”，从已架设FTP服务的服务器中下载文件，为“FileNameCapturer”添加了一种返回List的方法。
0. 2023/09/07 添加了“ScreenshotMaster”，通过UIRectTransform获取截图范围并进行就截图的截图大师。
0. 2023/10/10 添加了“UDPCommunicatorLite”，轻量版的UDP通讯工具，贼省事儿。
0. 2023/10/26 于工程同级目录下“Materials”文件夹中添加了“KinectV2”相关工具。添加了“VideoMaster”，具有播放视频、播放视频第一帧、视频播放结束回调功能。
0. 2023/11/06 UI模块下的截图工具与Media模块下的截图工具功能合并，新增全角度截图工具“FullAngleScreenshotTool”。
0. 2023/12/04 新增“SHADERS”模块，新增了一个可以将UGUI去色的Shader，需要额外创建材质球，详见“Examples/022_UGUIGray”。
0. 2023/12/28 分离“TextLoader”的json读写功能至“Data”分类下的“JsonManager”。
0. 2024/06/03 添加了“TextureProcessor”，读/写/旋转/缩放Texture。
0. 2024/06/18 添加了“LongTimeNoOperationDetector”，用于检测用户长时间无操作。
0. 2024/07/18 添加了“UDPCommunicatorServer”，单端口非一次性play，用于作为server大量接收数据。
0. 2024/10/11 更新了“ObjectDragRotate”，增加了旋转角度的限制，增加了一个角度校正的方法。
0. 2024/12/18 添加了“RenameFolders”，一个用于在编辑器内批量化改变文件夹名的工具，直接更新选中的文件夹的文件夹名为新文件夹名或更新所有匹配原文件夹名的文件夹的文件夹名为新文件夹名，嗯。
0. 2025/01/03 添加了“DataProcessor”，一个用于二级加工数据的工具，开新坑了，家人们。
0. 2025/01/07 添加了“UpdateCopyrights”，一个用于批量添加版权信息的工具，在“Project”面板中选择“.cs”文件后可正常执行。
0. 2025/01/10 添加了“ImageLoader”，用于运行时在弹窗内选择并加载图片，添加了第三方资源文件夹。
0. 2025/01/13 添加了“JsonUploadManager”，用于上传json的工具。
0. 2025/02/19 “QRCodeMaster”现在支持透明底二维码生成。
0. 2025/03/27 “FileCapturer”被重制，拥有更高级的功能。
0. 2025/04/18 “UI”类目下新增3个功能，滚动视图助手，序列帧图片播放助手，序列帧播放管理器。
0. 2025/08/04 “IO”类目下，新增“NewFileAlerter”，用于“定期检查指定文件夹下是否出现新文件并返回该文件路径”。
0. 2025/10/14 重新分类一些独立、综合的小功能，例如目前位于目录下的“Module/IO/Printer”，其包含了打印机控制相关的必须组件、脚本，而非仅有“Script”。



## <center><a id="MODULE"></a><font color="#44ff00">*MODULE*</font></center>
### ToneTuneToolkit.IO.Printer/
* 打印机模组

### ToneTuneToolkit.Other.QR/
* QR模组



## <center><a id="SCRIPTS"></a><font color="#FF0000">*SCRIPTS*</font></center>
### ToneTuneToolkit.Common/
* EventListener.cs      // 数值监听器 // 提供了一个泛型事件
* PathChecker.cs        // 静态 // 文件/文件夹检查 // 如果不存在则创建空的
* SingletonMaster.cs    // 单例大师
* ToolkitManager.cs     // 管理类 // 存放路径 // 多数功能的依赖
* TTTDebug.cs           // 静态 // TTT工具箱专属Debug.Log

### ToneTuneToolkit.Data/
* DataConverter.cs        // 静态 // 数据转换 // 字符串与二进制之间转换 // 字符串与json之间转换
* DataProcessor.cs        // 数据处理
* ImageLoader.cs          // 图片选择和加载
* JsonManager.cs          // newtonsoftjson管理器
* LitJsonManager.cs       // litjson管理器
* SensitiveWordUtility.cs // 关键词加载
* TextLoader.cs           // 静态 // 文字加载 // 可以读取txt及json
* TimestampCapturer.cs    // 静态 // 获取时间戳 // 本地获取静态方法 // 网络获取需单例

### ToneTuneToolkit.Editor/
* CreateAssetBundles.cs // AB包创建工具
* RenameFolders.cs      // 批量化重命名文件夹工具
* UpdateCopyrights.cs   // 更新版权工具

### ToneTuneToolkit.Funny/
* BubbleSort.cs         // 静态 // 冒泡排序

### ToneTuneToolkit.IO/
* FileCapturer.cs       // 静态 // 获取特定文件夹下特定格式的文件名
* FTPMaster.cs          // FTP文件下载(暂无上传)器
* NewFileAlerter.cs     // 检测指定文件夹下是否有新文件传入

### ToneTuneToolkit.Media/
* ScreenshotMaster.cs         // 透明通道截图工具
* FullAngleScreenshotTool.cs  // 全角度截图工具
* TextureProcessor.cs         // 图片处理工具

### ToneTuneToolkit.Mobile/
* ObjectRotateAndScale.cs     // 物体Android平台中的单指旋转及双指缩放

### ToneTuneToolkit.MultimediaExhibitionHall.LED/
* LEDCommandCenter.cs   // LED命令中心
* LEDCommandHub.cs      // 灯盒指令集
* LEDHandler.cs         // LED助手
* LEDNuclearShow.cs     // 灯带压力测试 // DEBUG

### ToneTuneToolkit.Networking/
* JsonUploadManager.cs // Json上传

### ToneTuneToolkit.Object/
* CorrectLookAtCamera.cs        // 使物体正对相机
* NeonLight.cs                  // 随机霓虹灯
* ObjectDragMove.cs             // 物体拖动移动
* ObjectDragRotate.cs           // 物体拖动旋转
* ObjectFloating.cs             // 物体上下漂浮
* ObjectSearcher.cs             // 多种方式寻找目标
* TraverseObejctChangeColor.cs  // 改变对象及所有子对象的颜色

### ToneTuneToolkit.Other/
* AsyncLoadingWithProcessBar.cs   // 加载场景进度条
* CMDLauncher.cs                  // CMD命令行
* KeyPressSimulator.cs            // 物理键盘按键模拟
* LongTimeNoOperationDetector.cs  // 长时间无操作检测

### ToneTuneToolkit.UDP/
* UDPCommunicator.cs        // UDP通讯器 // 已残
* UDPCommunicatorLite.cs    // UDP通讯器客户端轻量版
* UDPCommunicatorServer.cs  // UDP通讯器服务端
* UDPHandler.cs             // UDP助手
* UDPResponder.cs           // UDP响应器

### ToneTuneToolkit.UI/
* Parallax.cs           // 多层次视差
* ScrollViewHandler.cs  // 滚动视图定位元素
* TextFlick.cs          // 文字通过透明度闪烁
* UICurved.cs           // UI弯曲

### ToneTuneToolkit.Verification/
* AntiVerifier.cs     // 反向验证器 // 二进制
* Verifier.cs         // 验证器
* VerifierHandler.cs  // 验证系统助手

### ToneTuneToolkit.Video/
* VideoMaster.cs      // 视频大师

### ToneTuneToolkit.View/
* CameraFocusObject.cs  // 鼠标拖动控制相机环绕注视对象
* CameraLookAround.cs   // 鼠标拖动控制相机环视 // 可用于全景
* CameraSimpleMove.cs   // 经典场景漫游
* CameraZoom.cs         // 相机POV多层级缩放 // 开镜?

### ToneTuneToolkit.WOL/
* WakeOnLan.cs          // 局域网唤醒器
* WakeOnLanHandler.cs   // 局域网唤醒助手



## <center><a id="EXTRA"></a>*EXTRA*</center>
下列文件/功能位于与工程同级的“Materials”文件夹下
### 2D/
* // Tile贴图与尺寸保持1:1的设置方式

### 3D/
* // 创建一个物理引力点

### Alpha Video Mask/
* // 透明视频遮罩

### AzureKinect/
* AzureKinectDriver.cs // AzureKinect驱动模块

### Backend & Upload/
* // 后端上传模块

### CamFi2/
* // CamFi2驱动模块

### DG/Tools
* CanvasGroupMaster.cs // 透明度管理
* UIStageManager.cs // UI转阶段管理

### Game/
### IOS对策/
### Keyboard/

### KeyboardMapping/
* // 键盘错位映射模块

### KinectV2/
* KinectV2Driver.cs // KinectV2驱动模块

### LeapMotion/
* LeapMotionManager.cs // LP管理模块

### MediaPipe/

### MQTT/
* // MQTT驱动模块

### OpenCV/
* // 面部识别模块

### OSC/
* // 收发模块

### RemoveBG & BaiduBodySegment/
* // 人像分割模块

### RemoveTrial/
* // 移除试用版标记

### ScrollView/
* ScrollViewHandler.cs // 滚动视图驱动模块

### SerialPortUtilityPro/
* // 串口收发模块

### SocketIO/
* SocketIOManager.cs // SocketIO通讯模块

### TCP/
* // TCP传图传文字模块

### SkipLogo/
* // 跳过开屏Logo功能

### UI/
* ClickListener.cs // UI交互检测

### WebGL/
* // 背景透明化功能

### 打包后分辨率设置/
### 后置相机拍摄/
### 扫码枪/
### 图片选择和加载/



## <center><a id="SHADERS"></a>*SHADERS*</center>
### UGUI转灰色
* GreyscaleShader(Sprites/GreyscaleShader)



## <center><a id="TEXTURES"></a>*TEXTURES*</center>
### 512x512地板贴图
* grayfloor
* royalbluefloor



## <center><a id="FONTS"></a>*FONTS*</center>
### // 思源黑体简体中文
* SourceHanSansSC-Bold
* SourceHanSansSC-ExtraLight
* SourceHanSansSC-Heavy
* SourceHanSansSC-Light
* SourceHanSansSC-Medium
* SourceHanSansSC-Normal
* SourceHanSansSC-Regular
* 已移至ToneTuneToolkit工程目录“<strong>Assets/Fonts</strong>”中



## <center><a id="DEMOS"></a>*DEMOS*</center>
### 演示场景
* LED Sample      // LED灯控示例
* Panorama Sample // 全景示例
* Parallax Sample // 视差示例
* WOL Sample      // 局域网唤醒示例
* ……



## <center><a id="STORAGE"></a>*STORAGE*</center>
### 用于储存仅在Demo中出现且与核心功能无关的资源
* Materials
* Textures



## <center><a id="THIRDPARTY"></a>*THIRDPARTY*</center>
### 第三方脚本或资源
* StandaloneFileBrowser // 运行时弹出窗口




## <center><a id="TUTORIALS"></a>*TUTORIALS*</center>
### 一些教程
* 该功能依赖ToneTuneToolkit
* 场景文件、教程辅助用脚本文件位于ToneTuneToolkit工程目录“<strong>Assets/Tutorials/</strong>”中
* 博客内容保存在位于ToneTuneToolkit工程目录“<strong>Assets/PDFs</strong>”中



## <center><a id="CONTACT"></a>*CONTACT*</center>
### Developer
* **[团队代言人博客]** *随缘更新<br>
  **[https://www.cnblogs.com/mirzkisd1ex0/](https://www.cnblogs.com/mirzkisd1ex0/ "记得常来光顾")**
  <br>

* **[开发者邮箱]**<br>
  **[mirzkisd1ex0@outlook.com](https://outlook.live.com/ "欢迎来信联系")**
  <br>

* **[开发者微信]**<br>
  **[qq1005410781](https://weixin.qq.com/ "来啊交流啊")**
  <br>

* **[开发者企鹅]**<br>
  **[1005410781](https://im.qq.com/ "来啊交流啊")**
  <br>

![MirzkisD1Ex0](Materials/profile.jpg)

</font>