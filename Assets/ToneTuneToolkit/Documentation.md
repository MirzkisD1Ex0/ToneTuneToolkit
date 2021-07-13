<font face="STHeiti" size=2 color=#FFFFFF>

## <center><font size=6>**Tone Tune Toolkit**</font></center>
#### <center><font size=2>Make Everything Easy.</font></center>

---

## Note
1. 当某模块中包含“**Handler**”助手类，仅添加助手类至对象即可自动为其添加依赖。避免发生错误的组装。例如“**UDP**”以及“**Verification**”。
2. Nothing here.

---

## <center>*Script*</center>

## -> Common/
* TTTManager.cs // 管理类 // 多数功能的依赖
* TTTTextLoader.cs // 文字加载 // 可以读取txt及json
* TTTTipTools.cs // TTT工具箱专属Debug.Log

## -> Camera/
* TTTCameraFocusObject // 鼠标控制物体环绕注视对象
* TTTCameraLookAround // 鼠标拖动控制相机旋转 // 可用于全景

## -> Editor/
* Nothing Here.

## -> Object/
* TTTObjectDrag.cs // 对象拖拽
* TTTObjectFloating.cs // 对象上下漂浮
* TTTTraverseObejctChangeColor.cs // 改变对象及所有子对象的颜色

## -> UDP/
* TTTUDPCommunicator.cs // UDP收发工具
* TTTUDPHandler.cs // UDP工具助手

## -> Verification/
* TTTVerifier.cs // 验证系统
* TTTVerifierHandler.cs // 验证系统助手

---



</font>