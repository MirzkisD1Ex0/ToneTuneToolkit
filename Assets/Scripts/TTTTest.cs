using UnityEngine;
using System.Net.NetworkInformation;

namespace ToneTuneToolkit
{
    public class TTTTest : MonoBehaviour
    {

        private void Start()
        {
            // Debug.Log(SystemInfo.deviceUniqueIdentifier);

            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces(); // Get全部网卡
            for (int i = 0; i < nis.Length; i++)
            {
                if (nis[i].NetworkInterfaceType.ToString() == "Ethernet") // Get以太网
                {
                    Debug.Log(nis[i].GetPhysicalAddress().ToString()); // Mac地址确认
                }
            }
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