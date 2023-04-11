using System.Text.Json;
using MetaMask.Models;
using MetaMask.Unity;
using NFTPort;
using UnityEngine;

namespace Nabeel.Scripts
{
    public class MyConnectPlayerWallet : ConnectPlayerWallet
    {
        private void Awake()
        {
            MetaMaskUnity.Instance.Initialize();
        }
        
        public new void WebSend_GetAddress()
        {
// #if UNITY_ANDROID
//             // TODO: Implement Wallet for Android
//             Debug.Log("WebSend_GetAddress: Android Called!");
// #elif UNITY_IOS
//             // TODO: Implement Wallet for IOS
//            Debug.Log("WebSend_GetAddress: IOS Called!");
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            // TODO: Implement Wallet for Android / IOS
            Debug.Log("WebSend_GetAddress: Android / IOS Called!");

            MetaMaskUnity.Instance.Wallet.Connect();
            MetaMaskUnity.Instance.Wallet.WalletConnected += (sender, args) =>
            {
                Debug.Log("Android/IOS Metamask Wallet is connected!");
            };
            MetaMaskUnity.Instance.Wallet.WalletAuthorized += (sender, args) =>
            {
                Debug.Log("Android/IOS Metamask Wallet is authorized!");
                MetaMaskUnity.Instance.Wallet.Request(new MetaMaskEthereumRequest{
                    Method = "eth_requestAccounts",
                });
            };
            MetaMaskUnity.Instance.Wallet.EthereumRequestResultReceived += (sender, args) =>
            {
                Debug.Log("Android/IOS EthereumRequestResult Received!");
                Debug.Log("EthereumRequestResultReceived(Result): " + args.Result);
                Debug.Log("EthereumRequestResultReceived(Sender): " + sender);
                Debug.Log("EthereumRequestResultReceived(ValueKing): " + args.Result.ValueKind);
                
                base.WebHook_GetNetworkID(MetaMaskUnity.Instance.Wallet.SelectedChainId);
                base.WebHook_GetAddress(MetaMaskUnity.Instance.Wallet.SelectedAddress);
            };
#else
            Debug.Log("WebSend_GetAddress: WebGL / Editor / Others Called!");
            base.WebSend_GetAddress();
#endif
        }
    }
}
