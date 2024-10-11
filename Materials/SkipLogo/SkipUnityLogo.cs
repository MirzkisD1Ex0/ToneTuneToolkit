#if !UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;

public class SkipUnityLogo
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void BeforeSplashScreen()
    {
#if UNITY_WEBGL
        Application.focusChanged += Application_focusChanged;
#else
        System.Threading.Tasks.Task.Run(AsyncSkip);
#endif
    }

#if UNITY_WEBGL
    private static void Application_focusChanged(bool obj)
    {
        Application.focusChanged -= Application_focusChanged;
        SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
    }
#else
    private static void AsyncSkip()
    {
        SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
    }
#endif
}
#endif

// 百度和谷歌了一下解决方法，最终是通过用国外ip来重新激活license（仍然选择个人免费版），就解决问题了。
// 就是说只要不是国内ip去激活，就不会显示 trial version 字样了。
// 删除它的project settings文件夹。删除这个文件夹会导致一些配置的丢失，所以如果这样做的话需要考虑把配置迁移过来。