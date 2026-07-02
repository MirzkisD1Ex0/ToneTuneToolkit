<font face="Source Han Sans TC" size=2 color=#FFFFFF>

#### <center><font size=2>Make everything f<font color="#FF0000">or</font>king simple.</font></center>
#### <center><font size=2>2026/06/17</font></center>
# <center><font color="#54FF9F" size=6>**Tone Tune Toolkit v1.6.0**</font></center>



<font size=5><strong>ToneTuneToolkit是什么?</strong></font><br>
一个致力于帮助Unity六边形战士减轻开发负担的项目。<s>但更多的时候是在帮助工程师们偷懒。</s><br>
这下好了，Agent+Skill人人都可以VibeCoding了~<br>
但别担心，此工具库也正在由AI维护哟~

完成至少<strong>[1]</strong>个有些奇怪却十分好用的工具包：<br>
(√) 显现存在于Unity/C#中却不为人知的野路子<br>
(√) 添加需求简单但就是不想亲自编写的小功能<br>
<s>(×) 解决古怪/无理取闹/迷惑的开发需求</s><br>

<kbd>Ctrl</kbd> + <kbd>C</kbd><br>
<kbd>Ctrl</kbd> + <kbd>V</kbd><br>
<s>哈！逮到你了！</s><br>



## <center>DIRECTORY</center>
| EN | 中文
| :--- | :---
| [INTRODUCTION](#INTRODUCTION) | 介绍
| [LOG](#LOG) | 日志
| [MODULES](#MODULES) | 模组
| [EXTRA MODULES](#EXTRA-MODULES) | 扩展模组
| [SCRIPTS](#SCRIPTS) | 脚本
| [SHADERS](#SHADERS) | 着色器
| [TEXTURES](#TEXTURES) | 贴图
| [FONTS](#FONTS) | 字体
| [DEMOS](#DEMOS) | 示例
| [STORAGE](#STORAGE) | 仓库
| [THIRDPARTY](#THIRDPARTY) | 第三方
| [TUTORIALS](#TUTORIALS) | 教程
| [CONTACT](#CONTACT) | 联系



## <center><a id="INTRODUCTION"></a>*INTRODUCTION*</center>
- 请留意，“MirzkisD1Ex0”的“ToneTune Toolkit”基于**GPL3.0**(GNU General Public License v3.0)协议所开发。(对，就是那个传染性极强的协议。)
- 工具包存在“**Assets/ToneTuneToolkit**”文件夹及“**Assets/StreamingAssets/ToneTuneToolkit**”文件夹，两部分内容。
- 当某一模块中包含“**Handler**”助手类时，通常添加助手类至对象即可自动为其添加依赖。避免发生错误的组装。例如“**UDP**”模块以及“**Verification**”模块。
- 添加了思源黑体简中OTF格式全套。



## <center><a id="LOG"></a>*LOG*</center>
0. 2021/09/06 添加了两张简易场景地板贴图；添加了一些演示用场景；添加了三个可怕的工具，在“**Assets/StreamingAssets**”中。
0. 2021/09/22 路径检查现在有更为醒目的提示；添加了“Funny”命名空间，里面会存一些然并卵的鬼代码，比如冒泡排序，甚至还有冒泡排序的浮点型重载。添加了UDP响应器。
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
0. 2026/06/18 AI重新整理了本文档且对一些功能进行了修缮。与此同时，用户一次性删除了所有的历史Commits，为本仓库进行了瘦身处理。



## <center><a id="MODULES"></a>*MODULES*</center>
### ToneTuneToolkit.Data.XML/
* XML 模组 // XML2Class 生成与编辑

### ToneTuneToolkit.IO.Printer/
* 打印机模组

### ToneTuneToolkit.Networking.Upload/
* 上传模组 // 七牛云 / ZC / OY

### ToneTuneToolkit.Other.QR/
* QR 模组 // 二维码生成(支持透明底)



## <center><a id="EXTRA-MODULES"></a>*EXTRA MODULES*</center>
下列文件/功能位于工程同级目录“**Materials/Modules/**”下(不在 Unity 工程目录内)
### Bridge/
* JSCommunicator.cs

### DoTween/
* CanvasGroupMaster.cs
* UIStageManager.cs

### IO/
* CamFi/
  * Configer.cs
  * LiveViewer.cs
  * RESTSender.cs
  * SocketListener.cs
  * Tester.cs
* Canon/
  * Configer.cs
  * DslrBridgeRunner.cs
  * Tester.cs
  * UniOSCCallbackReceiver.cs
  * UniOSCManager.cs
* SeaoryS26/
  * SeaoryManager.cs
  * SeaoryTester.cs
  * SeaorySDK/SeaoryPrinter.cs
  * SeaorySDK/SeaoryScard.cs
  * SeaorySDK/SeaoryUhf.cs
* SerialPortUtilityPro/
  * SerialPortUtilityProConfiger.cs
  * SerialPortUtilityProManager.cs
  * SerialPortUtilityProResponder.cs
  * 红外感应/SerialPortUtilityProResponder.cs

### LeapMotion/
* LeapMotionManager.cs

### RealisticCarController/
* RCC_InputVisualizer.cs
* RCC_LogiSteeringManager.cs
* RCC_PlayerManager.cs

### VisionPro/
* ARAnchor/
  * ARAnchorHandler.cs
  * ARAnchorSetter.cs
* HandGesture/
  * HandGestureManager.cs
  * SimpleStaticHandGesture.cs



## <center><a id="SCRIPTS"></a>*SCRIPTS*</center>
### ToneTuneToolkit.Common/
* AFKDetector.cs
* EventListener.cs
* PathChecker.cs
* SingletonMaster.cs
* TTTDebug.cs
* ToolkitManager.cs

### ToneTuneToolkit.Data/
* DataConverter.cs
* DataProcessor.cs
* ImageLoader.cs
* LitJsonEditor.cs
* NewtonsoftJsonEditor.cs
* SensitiveWordUtility.cs
* TextLoader.cs
* TimestampCapturer.cs

### ToneTuneToolkit.Editor/
* CreateAssetBundles.cs
* EditorStorage.cs
* RenameFolders.cs
* UpdateCopyrights.cs

### ToneTuneToolkit.Funny/
* BubbleSort.cs
* SuspiciousCubeController.cs

### ToneTuneToolkit.IO/
* FTPMaster.cs
* FileCapturer.cs
* NewFileAlerter.cs

### ToneTuneToolkit.Media/
* FullAngleScreenshotTool.cs
* ScreenshotMaster.cs
* TextureProcessor.cs
* WebCamHandler.cs
* WebCamManager.cs

### ToneTuneToolkit.Mobile/
* ObjectRotateAndScale.cs

### ToneTuneToolkit.MultimediaExhibitionHall.LED/
* LEDCommandCenter.cs
* LEDCommandHub.cs
* LEDHandler.cs
* LEDNuclearShow.cs

### ToneTuneToolkit.Networking/
* JsonUploadManager.cs
* UDP/UDPCommunicator.cs
* UDP/UDPHandler.cs
* UDP/UDPResponder.cs

### ToneTuneToolkit.Object/
* CorrectLookAtCamera.cs
* Mesh2PointCloudHandler.cs
* NeonLight.cs
* ObjectDragMove.cs
* ObjectDragRotate.cs
* ObjectFloating.cs
* ObjectSearcher.cs
* TraverseObejctChangeColor.cs

### ToneTuneToolkit.Other/
* AsyncLoadingWithProcessBar.cs
* CMDLauncher.cs
* KeyPressSimulator.cs
* LongTimeNoOperationDetector.cs

### ToneTuneToolkit.UI/
* Parallax.cs
* SequenceFrameHandler.cs
* SequenceFrameManager.cs
* UICurved.cs

### ToneTuneToolkit.Verification/
* AntiVerifier.cs
* Verifier.cs
* VerifierHandler.cs

### ToneTuneToolkit.Video/
* VideoMaster.cs

### ToneTuneToolkit.View/
* CameraFocusObject.cs
* CameraLookAround.cs
* CameraSimpleMove.cs
* CameraZoom.cs

### ToneTuneToolkit.WOL/
* WakeOnLan.cs
* WakeOnLanHandler.cs



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