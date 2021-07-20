using UnityEngine;
using UnityEngine.UI;

namespace ToneTuneToolkit
{
    /// <summary>
    /// OK
    /// 文字闪烁
    /// 挂在对象上
    /// </summary>
    public class TTTTextFlick : MonoBehaviour
    {
        public float minAlpha = 102f; // 最小透明度
        public float maxAlpha = 255f; // 最大透明度
        public float speed = 15f; // 速度

        private float floatingValue = 0;
        private bool isFull = false;
        private Color newColor;
        private Text tCmpt;

        private void Start()
        {
            tCmpt = GetComponent<Text>();
            newColor = tCmpt.color;
        }

        private void Update()
        {
            TextAlphaFlick();
        }

        /// <summary>
        /// 文字透明度浮动
        /// </summary>
        private void TextAlphaFlick()
        {
            if (floatingValue < maxAlpha && !isFull)
            {
                floatingValue += Time.deltaTime * 10 * speed;
                if (floatingValue >= maxAlpha)
                {
                    isFull = true;
                }
            }
            else if (floatingValue > minAlpha && isFull)
            {
                floatingValue -= Time.deltaTime * 10 * speed;
                if (floatingValue <= minAlpha)
                {
                    isFull = false;
                }
            }
            newColor.a = floatingValue / 255;
            tCmpt.color = newColor;
        }
    }
}