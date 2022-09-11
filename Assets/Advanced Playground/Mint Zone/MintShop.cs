using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using NFTPort;
using TMPro;
using Object = System.Object;


namespace NFTPort.Samples
{
    public class MintShop : MonoBehaviour
    {
        [SerializeField] private UnityEvent onMintZoneStarted;
        [SerializeField] private UnityEvent onMintZoneSEnded;

        public Animator shopCharAnimator;
        [SerializeField] private AudioSource audioPlayer;
        [SerializeField] private AudioClip mintSuccessAudio;
        [SerializeField] private AudioClip mintErrorAudio;

        [Header("Easy Mint URL Parameters")]
        [SerializeField] private Mint_URL _mintURL;
        [SerializeField] private TMP_InputField _mintURLInputField;
        
        [Header("Easy Mint File Parameters")]
        [SerializeField] private Mint_File _mintFile;
        [SerializeField] private TMP_InputField _mintFileInputField;
        [SerializeField] private string nameOfGameObjectInStreamingAssetssFolder;

        [Header("Custom Mint Parameters")] 
        [SerializeField] private Storage_UploadMetadata _storageUploadMetadata;
        [SerializeField] private Mint_Custom _mintCustom;
        [SerializeField] private ExportGameobject _exportGameobject;
        [SerializeField] private GameObject gameObjectToExportRoot;

        public void ZoneStart()
        {
            onMintZoneStarted.Invoke();
            shopCharAnimator.SetBool("HelloWave", true);
        }
        
        public void ZoneExit()
        {
            onMintZoneSEnded.Invoke();
            shopCharAnimator.SetBool("HelloWave", false);
        }

        public void EasyMintURL()
        {
            shopCharAnimator.SetTrigger("objectInteract");
            
            //Via https://docs.nftport.xyz/docs/nftport/ZG9jOjU1MDM4OTgw-easy-minting-w-url
            _mintURL
                .SetChain(Mint_URL.Chains.polygon)
                .SetParameters(
                    //FileURL: "https://i.imgur.com/tzAbx5D.png", //We have set this in editor, new value can be passed here to override.
                    //Name: "NFTPort.xyz",  //We have set this in editor, new value can be passed here to override.
                    Description: "Custom player description: " + _mintURLInputField.text,
                    MintToAddress: Port.ConnectedPlayerAddress
                )
                .OnError(error=> ReturnedError(error))
                .OnComplete(Minted=> MintSuccess(Minted))
                .Run();
        }

        public void EasyMintFile()
        {
            shopCharAnimator.SetTrigger("objectInteract");
            
            // via https://docs.nftport.xyz/docs/nftport/ZG9jOjczMDEwMjIx-easy-minting-with-file-upload
            _mintFile
                .SetChain(Mint_File.Chains.polygon)
                .SetParameters(
                    FilePath: Path.Combine(Application.streamingAssetsPath, nameOfGameObjectInStreamingAssetssFolder),
                    //Name: "Awesome NFT", //We have set this in editor, new value can be passed here to override.
                    Description: "Custom player description: " + _mintFileInputField.text,
                    MintToAddress: Port.ConnectedPlayerAddress
                )
                .OnStarted(started => Debug.Log(started))
                .OnProgress(percent => Debug.Log("Uploading File: " + percent.ToString() + "%"))
                .OnError(error=> ReturnedError(error))
                .OnComplete(Minted=> MintSuccess(Minted))
                .Run();
        }

        private void OnEnable()
        {
            CustomMintProcess();
        }

        public void CustomMintProcess()
        {
            shopCharAnimator.SetTrigger("objectInteract");
            
            //We will create a custom procedural gameObject according to user Input
            CreateCustomPotion();
            
            //via https://docs.nftport.xyz/docs/nftport/ZG9jOjYzMDIzNDgx-customizable-minting
            //1. we have already deployed a custom product contract via document page under custom mint and noted the contract_address provided by it.
            //2. We have uploaded an Image and a 3D object via Storage_FileUpload and noted the urls of it, this step can also be added by code if objects are procedural.
            //3  We Create and Upload custom metadata according to user input
            //4. We Run the Custom Mint.
        }

        void CreateCustomPotion()
        {
            
            var path = Application.persistentDataPath + "object.glb";
            Debug.Log(path);
            var toExport = new GameObject[] {gameObjectToExportRoot };
            _exportGameobject
                .OnComplete(isSuccess => CreateCustomPotionComplete(isSuccess))
                .AdvancedExport(path, toExport);
        }

        void CreateCustomPotionComplete(bool isSuccess)
        {
            //f (isSuccess)
                //UploadCusomMetadata();
        }

        void UploadCusomMetadata()
        {
            Storage_MetadataToUpload_model metadataModel = new Storage_MetadataToUpload_model
            {
                file_url = "https://ipfs.io/ipfs/bafkreig4azycjdng6odwqp5s32rp55gdanaiw7blyl4eehzbhxom52ciui",
                name = "NFTPort Unity SDK",
                description = "Fast track your game development in Unity at NFTPort SDK with cross chain NFTs and fast and reliable data",
                external_url = "https://github.com/nftport/nftport-unity",
                animation_url = "https://design.embracingearth.space/wp-content/uploads/2022/05/index.html",
                attributes = new List<Attribute>{
                    new Attribute
                    {
                        trait_type = "Power",
                        value = "Fireball"
                    }},
                custom_fields = new List<custom_fields>{
                    new custom_fields
                    {
                        key = "wow factor",
                        value = "(இ௦இ)꒳ᵒ꒳ᵎᵎᵎ"
                    },
                    new custom_fields
                    {
                        key = "compiler",
                        value = "sz-101"
                    },
                    new custom_fields
                    {
                        key = "dna",
                        value = "T2T-CHM13"
                    },
                    new custom_fields
                    {
                        key = "background_color",
                        value = "Skobeloff"
                    },
                    new custom_fields
                    {
                        key = "id",
                        value = "Ser-007"
                    }
                },
            };
            
            
            _storageUploadMetadata
                .SetMetadata(metadataModel)
                .OnStarted(a=> Debug.Log(a))
                //.OnProgress(progress=> Debug.Log(progress.ToString()))
                .OnError(error=> ReturnedError(error))
                .OnComplete(storageModel=> CustomMint(storageModel))
                .Run();
        }

        void CustomMint(Storage_model.Storage metadataStorageModel)
        {
            _mintCustom
                .SetChain(Mint_Custom.Chains.polygon)
                .SetParameters(
                    contract_address: "0x42B57de948D05d17Fb11d4E527DF80A0420A4392",
                    metadata_uri: metadataStorageModel.ipfs_uri,
                    MintToAddress: Port.ConnectedPlayerAddress,
                    token_id: 0 //Set to 0 to mint to any available ID.
                )
                .OnError(error=> ReturnedError(error))
                .OnComplete(Minted=> MintSuccess(Minted))
                .Run();
                
        }


        public void ObjectClicked()
        {
            shopCharAnimator.SetTrigger("handyes");
        }
        void ReturnedError(string error)
        {
            Debug.Log(error); 
            shopCharAnimator.SetTrigger("no");
            audioPlayer.clip = mintErrorAudio;
            audioPlayer.Play();
        }
        void MintSuccess(Minted_model mintedModel)
        {
            shopCharAnimator.SetTrigger("nod");
            audioPlayer.clip = mintSuccessAudio;
            audioPlayer.Play();
        }
    }
}