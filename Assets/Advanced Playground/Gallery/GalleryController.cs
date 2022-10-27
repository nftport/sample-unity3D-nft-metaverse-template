using System;
using System.Collections;
using System.Collections.Generic;
using NFTPort;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Gallery))]
public class GalleryController : MonoBehaviour
{
    [SerializeField] private Dropdown galleryNetworkDropdownSelect;
    [SerializeField] private Toggle UseGifs;
    [SerializeField] private Toggle threeDGlbs;
    [SerializeField] private Text GalleryModeText;
    [SerializeField] private Text ModeAddress;
    [SerializeField] GameObject ContractModePanel;
    [SerializeField] GameObject WalletConnectPanel;
    public InputField CollectionAddressInput;
    public InputField FilterAccountFromContract;

    public bool contractMode = false;

    public bool DisplayGIFsOnNFTFrames;
    public bool Display3DAssetsIfAvailableatAnimationinMetadata;
    

    private Gallery _gallery;
    private void Awake()
    {
        PopulateChainDropDownListAccount();
        _gallery = GetComponent<Gallery>();
    }

    public void UseGifToggleeChanged()
    {
        DisplayGIFsOnNFTFrames = UseGifs.isOn;
    }
    public void threeDGlbsToggleeChanged()
    {
        Display3DAssetsIfAvailableatAnimationinMetadata = threeDGlbs.isOn;
    }

    public void ContractMode()
    {
        if (CollectionAddressInput.text == null)
        {
            Debug.Log("Enter Collection Contract Address");
            return;
        }

        ContractModePanel.SetActive(false);
        WalletConnectPanel.SetActive(false);
        FilterAccountFromContract.gameObject.SetActive(false);
        
        contractMode = true;
        GalleryModeText.text = "COLLECTION";
        ModeAddress.text = CollectionAddressInput.text;
        _gallery.Run();
    }
    public void AccountMode_WalletConnected()
    {
        ContractModePanel.SetActive(false);
        WalletConnectPanel.SetActive(false);
        FilterAccountFromContract.gameObject.SetActive(true);
        
        contractMode = false;
        GalleryModeText.text = "ACCOUNT";
        ModeAddress.text = Port.ConnectedPlayerAddress;
        _gallery.Run();
    }

    public void NetworkChanged()
    {
        if (contractMode)
            PopulateChainDropDownListContract();
        else
        {
            PopulateChainDropDownListAccount();
        }
        _gallery.Run();
        
    }


    #region Network DropDown 
    public NFTs_OwnedByAnAccount.Chains GetChainFromDropDownSelectAccount()
    {
        if (galleryNetworkDropdownSelect.value == 0)
            return NFTs_OwnedByAnAccount.Chains.ethereum;
        else if(galleryNetworkDropdownSelect.value == 1)
            return NFTs_OwnedByAnAccount.Chains.polygon;
        else 
            return NFTs_OwnedByAnAccount.Chains.goerli;
    }

    void PopulateChainDropDownListAccount()
    {
        galleryNetworkDropdownSelect.options.Clear();
        string[] enumChains = Enum.GetNames(typeof(NFTs_OwnedByAnAccount.Chains));
        List<string> chainNames = new List<string>(enumChains);
        galleryNetworkDropdownSelect.AddOptions(chainNames);
    }
    
    public NFTs_OfACollection.Chains GetChainFromDropDownSelectContract()
    {
        if (galleryNetworkDropdownSelect.value == 0)
            return NFTs_OfACollection.Chains.ethereum;
        else if(galleryNetworkDropdownSelect.value == 1)
            return NFTs_OfACollection.Chains.polygon;
        else 
            return NFTs_OfACollection.Chains.goerli;
    }

    void PopulateChainDropDownListContract()
    {
        galleryNetworkDropdownSelect.options.Clear();
        string[] enumChains = Enum.GetNames(typeof(NFTs_OwnedByAnAccount.Chains));
        List<string> chainNames = new List<string>(enumChains);
        galleryNetworkDropdownSelect.AddOptions(chainNames);
    }
    
    #endregion
    
   
}
