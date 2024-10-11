<font face="Source Han Sans TC" size=2 color=#FFFFFF>

#### <center><font size=2>Make everything f<font color="#FF0000">or</font>king simple.</font></center>
#### <center><font size=2>2024/10/11</font></center>
# <center><font color="#54FF9F" size=6>**Tone Tune Toolkit v1.4.17**</font></center>
## ToneTuneToolkit是什么?
一个致力于帮助Unity六边形战士减轻开发负担的项目。</br>
<s>但更多的时候是在帮助互动工程师偷懒。</s></br>

完成至少<strong>[1]</strong>个有些奇怪却十分好用的工具包：</br>
(√) 显现存在于Unity/C#中却不为人知的野路子</br>
(√) 添加需求简单但就是不想亲自编写的小功能</br>
<s>(×) 解决古怪且迷惑的开发需求</s></br>

<kbd>Ctrl</kbd> + <kbd>C</kbd></br>
<kbd>Ctrl</kbd> + <kbd>V</kbd></br>
<s>哈！逮到你了！</s></br>

</br>

# <center>*INTRODUCTION & LOG*</center>
1. 请留意，“MirzkisD1Ex0”的“ToneTune Toolkit”基于**GPL3.0**(GNU General Public License v3.0)协议所开发。(对，就是那个传染性极强的协议。)
2. 工具包存在“**Assets/ToneTuneToolkit**”文件夹及“**Assets/StreamingAssets/ToneTuneToolkit**”文件夹，两部分内容。
3. 当某一模块中包含“**Handler**”助手类时，通常添加助手类至对象即可自动为其添加依赖。避免发生错误的组装。例如“**UDP**”模块以及“**Verification**”模块。
4. 添加了思源黑体简中OTF格式全套。
5. 2021/09/06 添加了两张简易场景地板贴图。
6. 2021/09/06 添加了一些演示用场景。
7. 2021/09/06 添加了三个可怕的工具，在“**Assets/StreamingAssets**”中。
8. 2021/09/22 路径检查现在有更为醒目的提示。
9. 2021/09/23 添加了“Funny”命名空间，里面会存一些然并卵的鬼代码，比如冒泡排序，甚至还有冒泡排序的浮点型重载。添加了UDP响应器。
10. 2021/09/23 纠正了“PathChecker”中对文件夹路径检查的错误，更新了UDP和WOL非懒人方法的使用说明，移动了UDP消息接受体的位置。
11. 2021/09/24 为“LedHandler”添加了一个工具函数，可以根据输入的[-1f~0f~1f]生成[黄色~白色~蓝色]的Color。
12. 2021/10/11 添加了写入json的方法在“TextLoad”中。
13. 2021/11/10 添加了“CameraSimpleMove”，一个经典的场景漫游脚本，可以通过WSDA空格和LeftShift控制相机移动，按住鼠标右键以移动视角。
14. 2021/11/29 添加了AssetBundle包工具。
15. 2022/01/22 添加了“CorrectLookAtCamera”，一个使物体永远正对相机的脚本，改进了LookAt。
16. 2023/05/17 添加了“ObjectDragRotate”，拖动物体使其跟随鼠标旋转。
17. 2023/07/20 工具包结构巨幅整理。
18. 2023/07/21 添加了“FTPMaster”，从已架设FTP服务的服务器中下载文件，为“FileNameCapturer”添加了一种返回List的方法。
19. 2023/09/07 添加了“ScreenshotMaster”，通过UIRectTransform获取截图范围并进行就截图的截图大师。
20. 2023/10/10 添加了“UDPCommunicatorLite”，轻量版的UDP通讯工具，贼省事儿。
21. 2023/10/26 于工程同级目录下“Materials”文件夹中添加了“KinectV2”相关工具。添加了“VideoMaster”，具有播放视频、播放视频第一帧、视频播放结束回调功能。
22. 2023/11/06 UI模块下的截图工具与Media模块下的截图工具功能合并，新增全角度截图工具“FullAngleScreenshotTool”。
23. 2023/12/04 新增“SHADERS”模块，新增了一个可以将UGUI去色的Shader，需要额外创建材质球，详见“Examples/022_UGUIGray”。
24. 2023/12/28 分离“TextLoader”的json读写功能至“Data”分类下的“JsonManager”。
25. 2024/06/03 添加了“TextureProcessor”，读/写/旋转/缩放Texture。
26. 2024/06/18 添加了“LongTimeNoOperationDetector”，用于检测用户长时间无操作。
27. 2024/07/18 添加了“UDPCommunicatorServer”，单端口非一次性play，用于作为server大量接收数据。
28. 2024/10/11 更新了“ObjectDragRotate”，增加了旋转角度的限制，增加了一个角度校正的方法。

</br>

# <center>*SCRIPTS*</center>
### -> ToneTuneToolkit.Common/
* DataConverter.cs      // 静态 // 数据转换 // 字符串与二进制之间转换 // 字符串与json之间转换
* EventListener.cs      // 数值监听器 // 提供了一个泛型事件
* FileNameCapturer.cs   // 静态 // 获取特定文件夹下特定格式的文件名
* PathChecker.cs        // 静态 // 文件/文件夹检查 // 如果不存在则创建空的
* SingletonMaster.cs    // 单例大师
* TextLoader.cs         // 静态 // 文字加载 // 可以读取txt及json
* TimestampCapturer.cs  // 静态 // 获取时间戳 // 本地获取静态方法 // 网络获取需单例
* TipTools.cs           // 静态 // TTT工具箱专属Debug.Log
* ToolkitManager.cs     // 管理类 // 存放路径 // 多数功能的依赖

### -> ToneTuneToolkit.Editor/
* CreateAssetBundles.cs // AB包创建工具

### -> ToneTuneToolkit.Funny/
* BubbleSort.cs         // 静态 // 冒泡排序

### -> ToneTuneToolkit.IO/
* FTPMaster.cs          // FTP文件下载(暂无上传)器

### -> ToneTuneToolkit.Media/
* ScreenshotMaster.cs         // 透明通道截图工具
* FullAngleScreenshotTool.cs  // 全角度截图工具
* TextureProcessor.cs         // 图片处理工具

### -> ToneTuneToolkit.Mobile/
* ObjectRotateAndScale.cs   // 物体Android平台中的单指旋转及双指缩放

### -> ToneTuneToolkit.MultimediaExhibitionHall.LED/
* LEDCommandCenter.cs   // LED命令中心
* LEDCommandHub.cs      // 灯盒指令集
* LEDHandler.cs         // LED助手
* LEDNuclearShow.cs     // 灯带压力测试 // DEBUG

### -> ToneTuneToolkit.Object/
* CorrectLookAtCamera.cs        // 使物体正对相机
* NeonLight.cs                  // 随机霓虹灯
* ObjectDragMove.cs             // 物体拖动移动
* ObjectDragRotate.cs           // 物体拖动旋转
* ObjectFloating.cs             // 物体上下漂浮
* ObjectSearcher.cs             // 多种方式寻找目标
* TraverseObejctChangeColor.cs  // 改变对象及所有子对象的颜色

### -> ToneTuneToolkit.Other/
* AsyncLoadingWithProcessBar.cs // 加载场景进度条
* CMDLauncher.cs                // CMD命令行
* KeyPressSimulator.cs          // 物理键盘按键模拟
* QRCodeMaster.cs               // 二维码加载器
* LongTimeNoOperationDetector.cs        // 长时间无操作检测

### -> ToneTuneToolkit.UDP/
* UDPCommunicator.cs        // UDP通讯器 // 已残
* UDPCommunicatorLite.cs    // UDP通讯器客户端轻量版
* UDPCommunicatorServer.cs  // UDP通讯器服务端
* UDPHandler.cs             // UDP助手
* UDPResponder.cs           // UDP响应器

### -> ToneTuneToolkit.UI/
* Parallax.cs         // 多层次视差
* TextFlick.cs        // 文字通过透明度闪烁

### -> ToneTuneToolkit.Verification/
* AntiVerifier.cs     // 反向验证器 // 二进制
* Verifier.cs         // 验证器
* VerifierHandler.cs  // 验证系统助手

### -> ToneTuneToolkit.Video/
* VideoMaster.cs      // 视频大师

### -> ToneTuneToolkit.View/
* CameraFocusObject.cs  // 鼠标拖动控制相机环绕注视对象
* CameraLookAround.cs   // 鼠标拖动控制相机环视 // 可用于全景
* CameraSimpleMove.cs   // 经典场景漫游
* CameraZoom.cs         // 相机POV多层级缩放 // 开镜?

### -> ToneTuneToolkit.WOL/
* WakeOnLan.cs          // 局域网唤醒器
* WakeOnLanHandler.cs   // 局域网唤醒助手

</br>

# <center>*Extra*</center>
下列文件/功能位于与工程同级的“Materials”文件夹下
### -> 3D/
* // 创建一个物理引力点

### -> AzureKinect/
* AzureKinectDriver.cs // AzureKinect驱动模块

### -> Backend & Upload/
* // 后端上传模块

### -> CamFi2/
* // CamFi2驱动模块

### -> KeyboardMapping/
* // 键盘错位映射模块

### -> KinectV2/
* // KinectV2Driver.cs // KinectV2驱动模块

### -> MQTT/
* // MQTT驱动模块

### -> OpenCV/
* // 面部识别模块

### -> OSC/
* // 收发模块

### -> RemoveBG & BaiduBodySegment/
* // 人像分割模块

### -> RemoveTrial/
* // 移除试用版标记

### -> ScrollView/
* ScrollViewHandler.cs // 滚动视图驱动模块

### -> SequenceFrame/
* // 序列帧播放控制模块

### -> SerialPortUtilityPro/
* // 收发模块

### -> SkipLogo/
* // 跳过开屏Logo功能

### -> WebGL/
* // 背景透明化功能

### -> 后置相机拍摄/
* // 拍摄功能

</br>

# <center>*SHADERS*</center>
### -> UGUI转灰色

* GreyscaleShader(Sprites/GreyscaleShader)

</br>

# <center>*TEXTURES*</center>
### -> 512x512地板贴图
* grayfloor
* royalbluefloor

</br>

# <center>*FONTS (Removed)*</center>
### // -> 思源黑体简体中文
* // SourceHanSansSC-Bold
* // SourceHanSansSC-ExtraLight
* // SourceHanSansSC-Heavy
* // SourceHanSansSC-Light
* // SourceHanSansSC-Medium
* // SourceHanSansSC-Normal
* // SourceHanSansSC-Regular
* 因体积原因已从插件中移除
* 已移至ToneTuneToolkit工程目录“<strong>Assets/Fonts</strong>”中

</br>

# <center>*DEMOS*</center>
### -> 演示场景
* LED Sample      // LED灯控示例
* Panorama Sample // 全景示例
* Parallax Sample // 视差示例
* WOL Sample      // 局域网唤醒示例
* ……

</br>

# <center>*WAREHOUSE*</center>
### -> 用于储存仅在Demo中出现且与核心功能无关的资源
* Materials
* Textures

</br>

# <center>*EXAMPLES*</center>
### -> 一些教程
* 该功能依赖ToneTuneToolkit
* 场景文件、教程辅助用脚本文件位于ToneTuneToolkit工程目录“<strong>Assets/Examples/</strong>”中
* 博客内容保存在位于ToneTuneToolkit工程目录“<strong>Assets/PDFs</strong>”中

</br>

# <center>*CONTACT*</center>
### -> Developer
* **[团队代言人博客]** *随缘更新</br>
  **[https://www.cnblogs.com/mirzkisd1ex0/](https://www.cnblogs.com/mirzkisd1ex0/ "记得常来光顾")**
  </br>

* **[开发者邮箱]**</br>
  **[mirzkisd1ex0@outlook.com](https://outlook.live.com/ "欢迎来信联系")**
  </br>

* **[开发者微信]**</br>
  **[qq1005410781](https://weixin.qq.com/ "来啊交流啊")**
  </br>

* **[开发者企鹅]**</br>
  **[1005410781](https://im.qq.com/ "来啊交流啊")**
  </br>

![MirzkisD1Ex0](Materials/profile.jpg)

</font>