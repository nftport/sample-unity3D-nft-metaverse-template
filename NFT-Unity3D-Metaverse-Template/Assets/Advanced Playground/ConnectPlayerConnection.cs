using System;
using System.Collections;
using System.Collections.Generic;
using NFTPort;
using UnityEngine;
using UnityEngine.UI;

namespace NFTPort.Samples.Playground
{
    public class ConnectPlayerConnection : MonoBehaviour
    {
        [SerializeField] private Text[] walletTexts;
        [SerializeField] private InputField AddressInputField;
        [SerializeField] private ConnectPlayerWallet _connectPlayerWallet;

        private void Start()
        {
            _connectPlayerWallet.OnComplete((address, network) => PlayerWalletConnected());
        }

        //Button Linked
        public void EnterWalletAddress()
        {
            if(AddressInputField.text!=String.Empty)
                _connectPlayerWallet.ConnectThisToNFTPortWalletConnect(AddressInputField.text);
        }
    
        public void PlayerWalletConnected()
        {
            foreach (var walleText in walletTexts)
            {
                walleText.text = Port.ConnectedPlayerAddress;
            }
            Debug.Log("Wallet Connected: " + Port.ConnectedPlayerAddress);
        }
    
    }
}
