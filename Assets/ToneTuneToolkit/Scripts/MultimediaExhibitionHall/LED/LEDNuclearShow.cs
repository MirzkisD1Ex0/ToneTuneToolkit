using UnityEngine;
using UnityEngine.UI;

namespace ToneTuneToolkit.LED
{
    /// <summary>
    /// OK
    /// 核弹秀
    /// </summary>
    public class LEDNuclearShow : MonoBehaviour
    {
        private GameObject nuclearGO;
        private Image nICmpt;
        private Button nBCmpt;
        private Color color = Color.white;
        private int port = 0;
        private int begin = 0;
        private int end = 0;
        private bool isShowing = false;

        private void Start()
        {
            nuclearGO = GameObject.Find("Button - Nuclear");
            nICmpt = nuclearGO.GetComponent<Image>();
            nBCmpt = nuclearGO.GetComponent<Button>();

            nBCmpt.onClick.AddListener(StartNuclear);
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartNuclear()
        {
            if (!isShowing)
            {
                InvokeRepeating("RandomColor", 0, .1f);
            }
            else
            {
                CancelInvoke();
                nICmpt.color = new Color(0, 0, 0, 0);
            }
            isShowing = !isShowing;
            return;
        }

        private void RandomColor()
        {
            color = new Color(Random.Range(0f, 255f) / 255, Random.Range(0f, 255f) / 255, Random.Range(0f, 255f) / 255, 1);
            nICmpt.color = color;

            LEDCommandCenter.Instance.SLDimColor("#" + ColorUtility.ToHtmlStringRGB(color));
            return;
        }
    }
}