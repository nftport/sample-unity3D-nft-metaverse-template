using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using NFTPort;
using TMPro;
using UnityEngine.Rendering.Universal;
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
        [SerializeField] private AudioClip alreadyMintedAudio;

        [Header("Easy Mint URL Parameters")]
        [SerializeField] private Mint_URL mintURL;
        [SerializeField] private TMP_InputField mintURLInputField;
        [SerializeField] private bool isMintedURL = false;
        
        [Header("Easy Mint File Parameters")]
        [SerializeField] private Mint_File mintFile;
        [SerializeField] private TMP_InputField mintFileInputField;
        [SerializeField] private string nameOfGameObjectInStreamingAssetssFolder;
        [SerializeField] private bool isMintedFile  = false;

        [Header("Custom Mint Parameters")] 
        [SerializeField] private TakeScreenshotFromCamera takeScreenshotFromCamera;
        [SerializeField] private ExportGameobject exportGameobject;
        [SerializeField] private GameObject gameObjectToExportRoot;
        [SerializeField] private Storage_UploadFile storageUploadFile;
        [SerializeField] private Storage_UploadMetadata storageUploadMetadata;
        [SerializeField] private Mint_Custom mintCustom;
        [SerializeField] GameObject customMaterialGameObject;
        [SerializeField] public ColorPicker colorPicker; 
        [SerializeField] private bool isMintedCustom  = false;

        enum MintType
        {
            URL,
            File,
            Custom
        }

        public void ZoneStart()
        {
            onMintZoneStarted.Invoke();
            if(shopCharAnimator)
                shopCharAnimator.SetBool("HelloWave", true);
        }
        
        public void ZoneExit()
        {
            onMintZoneSEnded.Invoke();
            if(shopCharAnimator)
                shopCharAnimator.SetBool("HelloWave", false);
        }

        public void EasyMintURL()
        {
            if(shopCharAnimator)
                shopCharAnimator.SetTrigger("objectInteract");

            if (isMintedURL)
            {
                AlreadyMinted();
                return;
            }

            //Via https://docs.nftport.xyz/docs/nftport/ZG9jOjU1MDM4OTgw-easy-minting-w-url
            mintURL
                .SetChain(Mint_URL.Chains.polygon)
                .SetParameters(
                    //FileURL: "https://i.imgur.com/tzAbx5D.png", //We have set this in editor, new value can be passed here to override.
                    //Name: "NFTPort.xyz",  //We have set this in editor, new value can be passed here to override.
                    Description: "Custom player description: " + mintURLInputField.text,
                    MintToAddress: Port.ConnectedPlayerAddress
                )
                .OnError(error=> ReturnedError(error))
                .OnComplete(Minted=> MintSuccess(Minted, MintType.URL))
                .Run();
        }

        public void EasyMintFile()
        {
            if(shopCharAnimator)
                shopCharAnimator.SetTrigger("objectInteract");
            
            if (isMintedFile)
            {
                AlreadyMinted();
                return;
            }


            var path = Path.Combine(Application.streamingAssetsPath, nameOfGameObjectInStreamingAssetssFolder);

              // via https://docs.nftport.xyz/docs/nftport/ZG9jOjczMDEwMjIx-easy-minting-with-file-upload
            mintFile
                .SetChain(Mint_File.Chains.polygon)
                .SetParameters(
                    FilePath: path,
                    //Name: "Awesome NFT", //We have set this in editor, new value can be passed here to override.
                    Description: "Custom player description: " + mintFileInputField.text,
                    MintToAddress: Port.ConnectedPlayerAddress
                )
                .OnStarted(started => Debug.Log(started))
                .OnProgress(percent => Debug.Log("Uploading File: " + percent.ToString() + "%"))
                .OnError(error=> ReturnedError(error))
                .OnComplete(Minted=> MintSuccess(Minted, MintType.File))
                .Run();
        }


        #region Custom Mint + Runtiime generated NFT Screenshot and 3D Model

        private bool _customNftImageUploaded = false;
        private string _customNftImageURL;
        private bool _customNftAssetUploaded = false;
        private string _customNftAssetURL;
        public void CustomMintProcess()
        {
            
            //storageUploadFile.Stop(false); /dispose any previous running request for custom min 3D object file upload in case.
            
            if(shopCharAnimator)
                shopCharAnimator.SetTrigger("objectInteract");
            
            if (isMintedCustom)
            {
                AlreadyMinted();
                return;
            }

            //0. We will also create a procedural gameObject and metadata according to user Input, Similar can be done for EasyMint with File.
            _customNftImageUploaded = false;
            _customNftAssetUploaded = false;
            CreateCustomGameObject();
            CreateAScreeenShotImageForNFT();
            //via https://docs.nftport.xyz/docs/nftport/ZG9jOjYzMDIzNDgx-customizable-minting
            //1. we have already deployed a custom product contract via document page under custom mint and noted the contract_address provided by it.
            //2. We uploaded 3D object via Storage_FileUpload and note the urls of it.
            //3  We Create and Upload custom metadata according to user input
            //4. We Run the Custom Mint.
        }

        void CreateAScreeenShotImageForNFT()
        {
            Debug.Log("Creating NFT Image,");
            var path = Application.persistentDataPath + "NFTImage.png";
            if (takeScreenshotFromCamera.CreateAscreenShot(path))
            {
                //2. Upload Asset to IPFS
                //via https://docs.nftport.xyz/docs/nftport/ZG9jOjYwODM0NTY3-storage-upload-file
                Storage_UploadFile
                    .Initialize()
                    .SetFilePath(path)
                    .OnStarted(a=> Debug.Log(a))
                    .OnProgress(progress=> Debug.Log("Uploading NFTImage to IPFS: " + progress.ToString() + "%"))
                    .OnError(error=> Debug.Log(error))
                    .OnComplete(model => CheckIfBothImageAndAssetIsUploaded(_customNFTImageURL: model.ipfs_url))
                    .Run();
            }
            else
            {
                Debug.Log("Error creating NFT Image");
            }
        }

        #region Create 3D GLB and upload to IPFS
        void CreateCustomGameObject() 
        {
            Debug.Log("Creating NFT 3D Object to IPFS");
            //0. Create some procedural GameObject 
            customMaterialGameObject.GetComponent<MeshRenderer>().material.color = colorPicker.color;
            
            //0. Export Procedural GameObject via GLTFast //https://github.com/atteneder/glTFast/blob/main/Documentation~/ExportRuntime.md
            var path = Application.persistentDataPath + "object.glb";
            Debug.Log(path);
            var toExport = new GameObject[] {gameObjectToExportRoot };
            exportGameobject
                .OnComplete(isSuccess => CreateCustomGameObjectComplete(isSuccess, path))
                .AdvancedExport(path, toExport);
        }

        void CreateCustomGameObjectComplete(bool isSuccess, string path)
        {
            if (isSuccess)
            {
                Debug.Log("Custom Gameobject Made at: " + path);
                UploadObjectToIPFS();
            }
            else
            {
                Debug.Log("Unable to create custom GLB Object, Minting with Image");
                UploadCusomMetadatatoIPFS();
            }
        }

        void UploadObjectToIPFS()
        {
            //2. Upload our 3D Asset to IPFS
            //via https://docs.nftport.xyz/docs/nftport/ZG9jOjYwODM0NTY3-storage-upload-file
            storageUploadFile
                .SetFilePath(Application.persistentDataPath + "object.glb")
                .OnStarted(a=> Debug.Log(a))
                .OnProgress(progress=> Debug.Log("Uploading Procedural Object to IPFS: " + progress.ToString() + "%"))
                .OnError(error=> Debug.Log(error))
                .OnComplete(model => CheckIfBothImageAndAssetIsUploaded(_customNFTAssetURL: model.ipfs_url))
                .Run();
        }
        #endregion

        void CheckIfBothImageAndAssetIsUploaded(string _customNFTImageURL = null,  string _customNFTAssetURL = null)
        {
            if (_customNFTImageURL != null)
            {
                _customNftImageUploaded = true;
                _customNftImageURL = _customNFTImageURL;
                Debug.Log("Uploaded Custom Image to IPFS");
            }
            
            //webgl ovwerride for 3d object
            /*
            if (_customNFTAssetURL != null)
            {
                _customNftAssetUploaded = true;
                _customNftAssetURL = _customNFTAssetURL;
                Debug.Log("Uploaded Custom 3D Object to IPFS");
            }

            if (_customNftImageUploaded && _customNftAssetUploaded)
            {
                Debug.Log(_customNftImageURL);
                Debug.Log(_customNftAssetURL);
                */
                UploadCusomMetadatatoIPFS();
           // }
                
        }

        void UploadCusomMetadatatoIPFS()
        {
            Debug.Log("Creatind and Uploading custom metadata to IPFS");
            
            //3. Create Custom Metadata
            Storage_MetadataToUpload_model metadataModel = new Storage_MetadataToUpload_model
            {
                file_url = _customNftImageURL, //Got via CreateAScreeenShotImageForNFT()
                name = "Magic Potion",
                description = "A custom runtime generated magic potion according to player color input. NFT image, 3D GLB Asset and Metadata which includes color information and date NFT is minted, all generated at runtime game via custom mint NFTPort Unity SDK feature.",
                external_url = "https://github.com/nftport/nftport-unity",
                animation_url = _customNftAssetURL, //Got via UploadObjectToIPFS()
                attributes = new List<Attribute>{
                    new Attribute
                    {
                        trait_type = "Potion Version",
                        value = "1",
                        max_value = 7,
                        display_type = Displaytype.number
                    },
                    new Attribute
                    {
                        trait_type = "Color",
                        value = colorPicker.color.ToString() // User Input for Color
                    },
                    new Attribute
                    {
                        trait_type = "Date Minted", //following https://docs.opensea.io/docs/metadata-standards#date-traits
                        value = ((int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds).ToString(),
                        display_type = Displaytype.date
                    },
                    new Attribute
                    {
                        trait_type = "Via",
                        value = "NFTPort.xyz Unity Gaming SDK"
                    },
                    new Attribute
                    {
                        trait_type = "Feature",
                        value = "Custom Mint"
                    },
                    new Attribute
                    {
                        trait_type = "compiler",
                        value = "sz-101"
                    },
                },
                custom_fields = new List<custom_fields>{
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
                },
            };
            
            //3. Upload Custom Metadata
            storageUploadMetadata
                .SetMetadata(metadataModel)
                .OnStarted(a=> Debug.Log(a))
                //.OnProgress(progress=> Debug.Log(progress.ToString()))
                .OnError(error=> ReturnedError(error))
                .OnComplete(storageModel=> CustomMint(storageModel))
                .Run();
        }
        
        void CustomMint(Storage_model.Storage metadataStorageModel)
        {
            //4. Final Step: Custom Mint
            mintCustom
                .SetChain(Mint_Custom.Chains.polygon)
                .SetParameters(
                    contract_address: "0x42B57de948D05d17Fb11d4E527DF80A0420A4392",
                    metadata_uri: metadataStorageModel.metadata_uri,
                    mintToAddress: Port.ConnectedPlayerAddress,
                    token_id: 0 //Set to 0 to mint to any available ID.
                )
                .OnError(error=> ReturnedError(error))
                .OnComplete(Minted=> MintSuccess(Minted, MintType.Custom))
                .Run();
        }
        #endregion

        public void ObjectClicked()
        {
            if(shopCharAnimator)
                shopCharAnimator.SetTrigger("handyes");
        }
        void ReturnedError(string error)
        {
            Debug.Log(error); 
            if(shopCharAnimator)
                shopCharAnimator.SetTrigger("no");
            audioPlayer.clip = mintErrorAudio;
            audioPlayer.Play();
        }
        void MintSuccess(Minted_model mintedModel, MintType mintType)
        {
            if(shopCharAnimator)
                shopCharAnimator.SetTrigger("nod");
            audioPlayer.clip = mintSuccessAudio;
            audioPlayer.Play();

            //We check is user has already minted. 
            //This is just a local check - to have more proper check call API endpoint NFTs of Account and filter from Game connection
            //Then compare NFT name the account holds. Or Use Transactions of Account Feature and filter by Type Mint.
            switch (mintType)
            {
                case MintType.Custom:
                    isMintedCustom = true;
                    break;
                case MintType.File:
                    isMintedFile = true;
                    break;
                case MintType.URL:
                    isMintedURL = true;
                    break;
                    
            }
        }
        void AlreadyMinted()
        {
            if(shopCharAnimator)
                shopCharAnimator.SetTrigger("no");
            
            audioPlayer.clip = alreadyMintedAudio;
            audioPlayer.Play();
        }
    }
}