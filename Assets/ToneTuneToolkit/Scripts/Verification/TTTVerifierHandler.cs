using UnityEngine;

namespace ToneTuneToolkit
{
    /// <summary>
    /// OK
    /// 验证系统助手
    /// 需要正确的配置文件
    /// </summary>
    [RequireComponent(typeof(TTTManager))]
    [RequireComponent(typeof(TTTVerifier))]
    public class TTTVerifierHandler : MonoBehaviour
    {
        #region Paths
        public static string AuthorizationFilePath = TTTManager.DataPath + "/authorizationcode.json";
        #endregion

        #region KeyNames
        public static string UCName = "U C";
        public static string MCName = "M C";
        public static string TSName = "T S";
        #endregion

        private void Awake()
        {
            TTTManager.FileIntegrityCheck(AuthorizationFilePath);
        }
    }
}