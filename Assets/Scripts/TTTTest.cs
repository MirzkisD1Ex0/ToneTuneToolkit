using UnityEngine;
using System.Net.NetworkInformation;

namespace ToneTuneToolkit
{
    public class TTTTest : MonoBehaviour
    {

        private void Start()
        {
    Red();
        }

        public void Red()
        {
            TTTUDPCommunicator.Instance.SendMessageOut("[Dev]REQ=SemanticLighting&Action=DimColor&Port=1&Begin=1&End=144&Color=#FF0000");
        }

        public void Yellow()
        {
            TTTUDPCommunicator.Instance.SendMessageOut("[Dev]REQ=SemanticLighting&Action=DimColor&Port=1&Begin=1&End=144&Color=#FFFF00");
        }
    }
}