using System;
using System.Text.Json;
using System.Threading.Tasks;
using MetaMask;
using MetaMask.Models;
using MetaMask.Unity;
using NFTPort;
using UnityEngine;
using UnityEngine.UI;

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
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL)
            Debug.Log("WebSend_GetAddress: Android / IOS / WebGL Called!");

            MetaMaskUnity.Instance.Wallet.Connect();
            MetaMaskUnity.Instance.Wallet.WalletConnected += (sender, args) =>
            {
                Debug.Log("Android / IOS / WebGL Metamask Wallet is connected!");
            };
            MetaMaskUnity.Instance.Wallet.WalletAuthorized += async (sender, args) =>
            {
                Debug.Log("Android / IOS / WebGL Metamask Wallet is authorized!");
                await MetaMaskUnity.Instance.Wallet.Request(new MetaMaskEthereumRequest{
                    Method = "eth_requestAccounts",
                });
            };
            MetaMaskUnity.Instance.Wallet.EthereumRequestResultReceived += WalletOnEthereumRequestResultReceived;
#else
            Debug.Log("WebSend_GetAddress: Editor / Others Called!");
            base.WebSend_GetAddress();
#endif
        }

        public async void SendTransaction(InputField addressInputField)
        {
            if (!MetaMaskUnity.Instance.Wallet.IsConnected)
            {
                Debug.Log("SendTransaction: Wallet Not Connected!");
                return;
            }
            
            if (!MetaMaskUnity.Instance.Wallet.IsAuthorized)
            {
                Debug.Log("SendTransaction: Wallet Not Authorized!");
                return;
            }
            
            if (MetaMaskUnity.Instance.Wallet.IsPaused)
            {
                Debug.Log("SendTransaction: Wallet is Paused!");
                return;
            }
            
            if (addressInputField.text == String.Empty)
            {
                Debug.Log("SendTransaction: Address Input Field is Empty!");
                return;
            }
            
            var transactionParams = new MetaMaskTransaction
            {
                To = addressInputField.text,
                From = MetaMaskUnity.Instance.Wallet.SelectedAddress,
                Value = "0x0"
            };

            var request = new MetaMaskEthereumRequest
            {
                Method = "eth_sendTransaction",
                Parameters = new[] { transactionParams }
            };
            await MetaMaskUnity.Instance.Wallet.Request(request);
        }

        private void WalletOnEthereumRequestResultReceived(object sender, MetaMaskEthereumRequestResultEventArgs e)
        {
            switch (e.Request.Method)
            {
                case "eth_requestAccounts":
                    Debug.Log("'eth_requestAccounts' EthereumRequestResult Received!");
                    Debug.Log("EthereumRequestResultReceived(Sender): " + sender);
                    Debug.Log("EthereumRequestResultReceived(ValueKing): " + e.Result.ValueKind);
                    Debug.Log("EthereumRequestResultReceived: Request Account Result Received: " + e.Result);
                
                    base.WebHook_GetNetworkID(MetaMaskUnity.Instance.Wallet.SelectedChainId);
                    base.WebHook_GetAddress(MetaMaskUnity.Instance.Wallet.SelectedAddress);
                    break;
                case "eth_sendTransaction":
                    Debug.Log("'eth_sendTransaction' EthereumRequestResult Received!");
                    Debug.Log("EthereumRequestResultReceived(Sender): " + sender);
                    Debug.Log("EthereumRequestResultReceived(ValueKing): " + e.Result.ValueKind);
                    Debug.Log("EthereumRequestResultReceived: Send Transaction Result Received: " + e.Result);
                    
                    break;
            }
        }
    }
}
