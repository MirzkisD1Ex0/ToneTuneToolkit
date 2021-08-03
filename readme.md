<font face="Source Han Sans TC" size=2 color=#FFFFFF>

#### <center><font size=2>Make Everything Simple.</font></center>
#### <center><font size=2>2021/07/29</font></center>
# <center><font color="#54FF9F" size=6>**Tone Tune Toolkit v1.1.0**</font></center>
## ToneTuneToolkit是什么?
一个致力于帮助Unity全能系开发者减轻开发负担的项目。</br>
<s>但更多的时候是在帮助程序员偷懒。</s></br>

一些存在于Unity/C#中却不为人知的技巧。</br>
一些很简单但不想自行开发的功能。</br>
一些古怪且迷惑的开发需求。</br>
<strong>这里的代码请随意取用。</strong></br>
</br>
<kbd>Ctrl</kbd> + <kbd>C</kbd></br>
<kbd>Ctrl</kbd> + <kbd>V</kbd></br>
</br>
<s>哈！逮到你了！</s></br>

</br>

# <center>*INTRODUCTION & LOG*</center>
1. 插件内容包含“<strong>ToneTuneToolkit</strong>”文件夹及“<strong>StreamingAssets/ToneTuneToolkit</strong>”文件夹。
2. 当某模块中包含“**Handler**”助手类时，仅添加助手类至对象即可自动为其添加依赖。避免发生错误的组装。例如“**UDP**”以及“**Verification**”。
3. 添加了思源黑体简中OTF格式全套。
4. 添加了两张简易贴图。
5. 添加了一些演示用场景。
6. 添加了两个可怕的工具。
7. Nothing here.

</br>

# <center>*SCRIPTS*</center>
### -> ToneTuneToolkit.Common/
* ToolkitManager // 管理类 // 多数功能的依赖
* DataConverter // 静态 // 数据转换 // 字符串与二进制之间转换
* EventListener // 数值监听器 // 提供了一个泛型事件
* FileNameCapturer // 静态 // 获取特定文件夹下特定格式的文件名
* JsonConstructer // 静态 // 将字典与json之间相互转换
* TextLoader // 静态 // 文字加载 // 可以读取txt及json
* TimestampCapturer // 静态 // 获取时间戳 // 本地获取静态方法 // 网络获取单例
* TipTools // 静态 // TTT工具箱专属Debug.Log

### -> ToneTuneToolkit.Camera/
* CameraFocusObject // 鼠标控制物体环绕注视对象
* CameraLookAround // 鼠标拖动控制相机旋转 // 可用于全景

### -> ToneTuneToolkit.Editor/
* Nothing Here.

### -> ToneTuneToolkit.Object/
* ObjectDrag // 对象拖拽
* ObjectFloating // 对象上下漂浮
* TraverseObejctChangeColor // 改变对象及所有子对象的颜色

### -> ToneTuneToolkit.UDP/
* UDPCommunicator // UDP收发工具
* UDPHandler // UDP工具助手

### -> ToneTuneToolkit.UI/
* Parallax // 简易视差效果
* TextFlick // 文字通过透明度闪烁

### -> ToneTuneToolkit.Verification/
* AntiVerifier // 反向验证器 // 二进制
* Verifier // 验证器
* VerifierHandler // 验证系统助手

### -> ToneTuneToolkit.WOL/
* WakeOnLan // 局域网唤醒器
* WakeOnLanHandler // 局域网唤醒助手

### -> ToneTuneToolkit.Other/
* AsyncLoadingWithProcessBar // 加载场景进度条

### -> ToneTuneToolkit.LED/
* LEDCommandCenter // LED命令中心
* LEDCommandHub // 凌恩指令集
* LEDHandler // LED助手
* LEDNuclearShow // DEBUG // 灯带压力测试

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
* LED Sample // LED灯控案例
* Panorama Sample // 全景案例
* Parallax Sample // 视差案例
* WOL Sample // 局域网唤醒案例
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

![isaacyang](Cache/profile.jpg)

</font>