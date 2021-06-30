using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToneTuneToolkit
{
    public class TTTTest : MonoBehaviour
    {
        private void Start()
        {
            AsyncLoadingWithProcessBar.Instance.LoadingScene(01);
        }
    }
}