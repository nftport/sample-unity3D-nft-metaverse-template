using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NFTPort.Samples
{
    /// <summary>
    /// Updates a Text Element with Wallet Address / Network ID whenever the ConnnectPlayerWallet Feature of NFTPort is connected or updated with a wallet.
    /// </summary>
    public class WalletTextConnect : MonoBehaviour
    {
        private ConnectPlayerWallet _connectPlayerWallet;
        [SerializeField] private Text walletText;
        [SerializeField] private Text networkIDText;
        void OnEnable()
        {
            _connectPlayerWallet = FindObjectOfType<ConnectPlayerWallet>();
            _connectPlayerWallet.OnComplete((address, networkID ) => WalletConnectionUppdated(address, networkID));
        }

        void WalletConnectionUppdated(string address, string networkID)
        {
            if(walletText)
                walletText.text = address;
            if (networkIDText)
                networkIDText.text = networkID;
        }

        void Update()
        {
        
        }
    }
}

