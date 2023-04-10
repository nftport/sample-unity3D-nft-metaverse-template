using NFTPort;
using UnityEngine;

namespace Nabeel.Scripts
{
    public class MyConnectPlayerWallet : ConnectPlayerWallet
    {
        public new void WebSend_GetAddress()
        {
#if UNITY_ANDROID
            // TODO: Implement Wallet for Android
            Debug.Log("WebSend_GetAddress: Android Called!");
#elif UNITY_IOS
            // TODO: Implement Wallet for IOS
           Debug.Log("WebSend_GetAddress: IOS Called!");
#else
            Debug.Log("WebSend_GetAddress: WebGL / Editor / Others Called!");
            base.WebSend_GetAddress();
#endif
        }
    }
}
