<font face="Source Han Sans TC" size=2 color=#FFFFFF>

#### <center><font size=2>Make Everything Simple.</font></center>
#### <center><font size=2>2022/01/22</font></center>
# <center><font color="#54FF9F" size=6>**Tone Tune Toolkit v1.3.7**</font></center>
## ToneTuneToolkit是什么?
一个致力于帮助Unity六边形战士减轻开发负担的项目。</br>
<s>但更多的时候是在帮助程序员偷懒。</s></br>

<strong>主线</strong>:完成至少<strong>[1]</strong>个有些奇怪却十分好用的工具包</br>
(√) 显现存在于Unity/C#中却不为人知的野路子</br>
(√) 加入很简单但不想自行开发的功能</br>
<s>(×) 了解古怪且迷惑的开发需求</s></br>

<strong>这里的代码请随意取用。</strong></br>
<kbd>Ctrl</kbd> + <kbd>C</kbd></br>
<kbd>Ctrl</kbd> + <kbd>V</kbd></br>
<s>哈！逮到你了！</s></br>

</br>

# <center>*INTRODUCTION & LOG*</center>
001. 请留意，“MirzkisD1Ex0”的“ToneTune Toolkit”基于<strong>GPL3.0</strong>(GNU General Public License v3.0)协议所开发。（对，就是那个传染性极强的协议。）
002. 插件内容包含“<strong>ToneTuneToolkit</strong>”文件夹及“<strong>StreamingAssets/ToneTuneToolkit</strong>”文件夹。
003. 当某模块中包含“**Handler**”助手类时，仅添加助手类至对象即可自动为其添加依赖。避免发生错误的组装。例如“**UDP**”以及“**Verification**”。
004. 添加了思源黑体简中OTF格式全套。
005. 2021/09/06 添加了两张简易贴图。
006. 2021/09/06 添加了一些演示用场景。
007. 2021/09/06 添加了三个可怕的工具，在StreamingAssets中。
008. 2021/09/22 路径检查现在有更为醒目的提示了。
009. 2021/09/23 添加了Funny命名空间，里面会存一些然并卵的鬼代码，比如冒泡排序，甚至还有冒泡排序的浮点型重载。添加了UDP响应器。
010. 2021/09/23 纠正了PathChecker中对文件夹路径检查的错误，更新了UDP和WOL非懒人方法的使用说明，移动了UDP消息接受体的位置。
011. 2021/09/24 为LedHandler添加了一个工具函数，可以根据输入的[-1f~0f~1f]生成[黄色~白色~蓝色]的Color。
012. 2021/10/11 添加了写入json的方法在TextLoad中。
013. 2021/11/10 添加了CameraSimpleMove，一个经典的场景漫游脚本，可以通过WSDA空格和LeftShift控制相机移动，按住鼠标右键以移动视角。
014. 2021/11/29 添加了ab包工具。
015. 2022/01/22 添加了“CorrectLookAtCamera”，一个使物体永远正对相机的脚本，改进了LookAt。
016. 2023/05/17 添加“ObjectDragRotate”，拖动物体使其跟随鼠标旋转。

</br>

# <center>*SCRIPTS*</center>
### -> ToneTuneToolkit.Common/
* ToolkitManager      // 管理类 // 存放路径 // 多数功能的依赖
* DataConverter       // 静态 // 数据转换 // 字符串与二进制之间转换 // 字符串与json之间转换
* EventListener       // 数值监听器 // 提供了一个泛型事件
* FileNameCapturer    // 静态 // 获取特定文件夹下特定格式的文件名
* PathChecker         // 静态 // 文件/文件夹检查 // 如果不存在则创建空的
* TextLoader          // 静态 // 文字加载 // 可以读取txt及json
* TimestampCapturer   // 静态 // 获取时间戳 // 本地获取静态方法 // 网络获取需单例
* TipTools            // 静态 // TTT工具箱专属Debug.Log

### -> ToneTuneToolkit.Editor/
* CreateAssetBundles  // AB包创建工具

### -> ToneTuneToolkit.Funny/
* BubbleSort // 冒泡排序

### -> ToneTuneToolkit.Mobile/
* ObjectRotateAndScale // 物体Android平台中的单指旋转及双指缩放

### -> ToneTuneToolkit.Object/
* CorrectLookAtCamera         // 使物体正对相机
* NeonLight                   // 随机霓虹灯
* ObjectDragMove              // 物体拖动移动
* ObjectDragRotate            // 物体拖动旋转
* ObjectFloating              // 物体上下漂浮
* ObjectSearcher              // 多种方式寻找目标
* TraverseObejctChangeColor   // 改变对象及所有子对象的颜色

### -> ToneTuneToolkit.Other/
* AsyncLoadingWithProcessBar    // 加载场景进度条
* CMDLauncher                   // CMD命令行
* KeyPressSimulator             // 物理键盘按键模拟

### -> ToneTuneToolkit.UDP/
* UDPCommunicator   // UDP通讯器
* UDPHandler        // UDP助手
* UDPResponder      // UDP响应器

### -> ToneTuneToolkit.UI/
* Parallax    // 多层次视差
* TextFlick   // 文字通过透明度闪烁

### -> ToneTuneToolkit.Verification/
* AntiVerifier      // 反向验证器 // 二进制
* Verifier          // 验证器
* VerifierHandler   // 验证系统助手

### -> ToneTuneToolkit.View/
* CameraFocusObject   // 鼠标拖动控制相机环绕注视对象
* CameraLookAround    // 鼠标拖动控制相机环视 // 可用于全景
* CameraSimpleMove    // 经典场景漫游
* CameraZoom          // 相机POV多层级缩放 // 开镜?

### -> ToneTuneToolkit.WOL/
* WakeOnLan           // 局域网唤醒器
* WakeOnLanHandler    // 局域网唤醒助手

### -> ToneTuneToolkit.LED/
* LEDCommandCenter  // LED命令中心
* LEDCommandHub     // 灯盒指令集
* LEDHandler        // LED助手
* LEDNuclearShow    // 灯带压力测试 // DEBUG

</br>

# <center>*TEXTURES*</center>
### -> 512x512地板贴图
* grayfloor
* royalbluefloor

</br>

# <center>// *FONTS (Removed)*</center>
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
* **[团队代言人博客]**</br>
  **[https://www.cnblogs.com/mirzkisd1ex0/](https://www.cnblogs.com/mirzkisd1ex0/ "记得常来光顾")**
  </br>

* **[开发者邮箱]**</br>
  **[dearisaacyang@outlook.com](https://outlook.live.com/ "欢迎来信联系")**
  </br>

* **[开发者微信]**</br>
  **[wxid_63t8w3035kvp22](https://weixin.qq.com/ "来啊交流啊")**
  </br>

* **[开发者企鹅]**</br>
  **[2957047371](https://im.qq.com/ "来啊交流啊")**
  </br>

![isaacyang](Materials/profile.jpg)

</font>