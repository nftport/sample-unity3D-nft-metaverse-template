using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NFTPort;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(NFTs_OfACollection))]
[RequireComponent(typeof(NFTs_OwnedByAnAccount))]
public class Gallery : MonoBehaviour
{
    //////â‰§â— â€¿â— â‰¦âœŒ _sz_ Î© //≧◠‿◠≦✌ _sz_ Ω embracingearth.space
    [SerializeField] private GalleryController _galleryController;
    [SerializeField] private GalleryReUsableAssets _galleryReUsableAssets;
    [SerializeField] NFTs_OwnedByAnAccount _nfTOwnedByAnAccount;
    [FormerlySerializedAs("_nfTsOfAContract")] [SerializeField] NFTs_OfACollection _nfTsOfACollection;
    public float WaitBeforeContinuation = 0.01f;
    public int StartNextLoopWhenThisNumberofFramesifStillDownloading = 8;
    
    [Space]
    [ReadOnly] public GalleryNFT_frame[] GalleryNftFrames;
    [ReadOnly] public int framesFilled = 0;
    private bool allFramesFilled = false;

    private void Start()
    {
        // Debug.Log("NFTPort Metaverse Sample");
        SetGalleryFrames();
    }

    public void SetGalleryFrames()
    {
        ResetGalleryFrames();
        GalleryNftFrames = FindObjectsOfType<GalleryNFT_frame>(); //We store all the Gallery Frames in the scene in an array, This way they just can be added or removed in scene.
        
        foreach (var galleryNftFrame in GalleryNftFrames)
        {
            galleryNftFrame.EnableDefaultVisuals();
        }
    }

    private void ResetGalleryFrames()
    {
        foreach (var galleryNftFrame in GalleryNftFrames)
        {
            galleryNftFrame.Reset();
        }
        framesFilled = 0;
        allFramesFilled = false;
        NftsWithGif.Clear();
        //GifLoopRunning = false;
        ContinuationContractModePageNumb = 0;
        NftsCheckedInCollection = 0;
        firstContinuationLoop = true;

        var UniGifs = FindObjectsOfType<UniGifImage_PortMod>();
        foreach (var UniGif in UniGifs)
        {
            UniGif.Stop();
        }

        StopAllImageAssetDownloaders();
        KillAnyUniGifsStillDownloading();
        AssetDownloadersContentTypeForceEnd();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        
    }

    public void Run()
    {
        
        ResetGalleryFrames();
        ContinuationLoopReset();
        CheckModeAndGo();
    }

    void ContinuationLoopReset()
    {
        CurrentNftsModelTotalCount = 0;
        NFTsCheckedinCurrentModel = 0;
        //gifloopInt = 0;
    }

    void ContinuationLoopToNextBatch(string continuation)
    {
        ContinuationLoopReset();
        CheckModeAndGo(continuation);
        this.continuation = "";
    }

    void CheckModeAndGo(string continuation = "")
    {
        if (_galleryController.contractMode)
            ContractMode();
        else
        {
            AccountMode(continuation);
        }
    }

    public void AccountMode(string continuation = "")
    {
        CheckWalletNFTs(continuation);
    }
    public void ContractMode()
    {
        if (firstContinuationLoop)
        {
            CheckContractNFTs();
            firstContinuationLoop = false;
        }
        else
        {
            NftsCheckedInCollection = (ContinuationContractModePageNumb * 50);// one API call provides max 50 nfts
            if(NftsCheckedInCollection < TotalNftsOfCollection)
                CheckContractNFTs();
            else
            {
                Debug.Log("All NFTs of this collection have been queried");
            }
        }
    }

    private bool firstContinuationLoop = true;
    [ReadOnly]public int ContinuationContractModePageNumb = 0;
    [ReadOnly]public int TotalNftsOfCollection = 0;
    [ReadOnly]public int NftsCheckedInCollection = 0;
    void CheckContractNFTs()
    {
        Debug.Log("Using NFTPort : NFTs of Contract on " + _galleryController.GetChainFromDropDownSelectContract()); 
        ContinuationContractModePageNumb++;
        
        //From https://docs.nftport.xyz/docs/nftport/ZG9jOjUzNjI2MzQ0-nf-ts-of-a-contract-collection
        _nfTsOfACollection
            .SetChain(_galleryController.GetChainFromDropDownSelectContract())
            .SetParameters(
                
                collection:_galleryController.CollectionAddressInput.text,
                include:NFTs_OfACollection. Includes.metadata
                )
            //.OnError(error=>Debug.Log(error)) 
            .SetContinuation(ContinuationContractModePageNumb)
            .OnComplete(NFTsOfContract=> DownloadAssets(NFTsOfContract))
            .Run();
    }

    void CheckWalletNFTs(string continuation = "")
    {
        Debug.Log("Using NFTPort : NFTs of Account on " + _galleryController.GetChainFromDropDownSelectAccount() + " .Filter " + _galleryController.FilterAccountFromContract.text);
        
        // From https://docs.nftport.xyz/docs/nftport/ZG9jOjUyMzI4NTkz-nf-ts-owned-by-an-account
        _nfTOwnedByAnAccount
            .SetChain(_galleryController.GetChainFromDropDownSelectAccount())
            .SetAddress(Port.ConnectedPlayerAddress)
            .SetInclude(NFTs_OwnedByAnAccount.Includes.metadata)
            .SetFilterFromCollection(_galleryController.FilterAccountFromContract.text)
            .SetContinuation(continuation)
            //.OnError(error=>Debug.Log(error)) 
            .OnComplete(NFTsOfUser=> DownloadAssets(NFTsOfUser))
            .Run();
    }

    #region Assets Download for Main Frame Image
    //Determine ContentType of Assets we want to Download for NFTs for Main Frame
    void DownloadAssets(NFTs_model NFTsModel)
    {
        if(NFTsModel.total != 0)
            TotalNftsOfCollection = NFTsModel.total; //Total NFTs of collection - Used for continuation for NFTs of contract
        
        CurrentNftsModelTotalCount = NFTsModel.nfts.Count; //CurrentNftsModelTotalCount used for API pagination/continuation at end
        continuation = NFTsModel.continuation;
        
        if(NFTsModel.nfts.Count==0)
            Debug.Log("No NFTs found on this network with this filter");

        foreach (var nft in NFTsModel.nfts)
        {
            //Stop All downloads and return next loop if all frames are filled.
            if (allFramesFilled)
            {
                StopAllImageAssetDownloaders();
                return;
            }
            
            //Checks to filter out nfts with missing parameters in meetadata, that we want.
            if ((nft.file_url != null || nft.cached_file_url != null) 
                && (nft.name !=null || nft.metadata.name !=null || nft.token_id != null))//&&)
            {
                //via https://docs.nftport.xyz/docs/nftport/ZG9jOjU2NjAzOTE0-asset-downloader
                //content-type to download
                var temp = AssetDownloader.DetemineURLContentType
                    .Initialize()
                    //.OnError(error => Debug.Log(error))
                    .OnError(error => NFTsCheckedPlus())
                    .OnComplete((nft, contentType) => HandleAssetContentType(nft, contentType))
                    .Run(nft, NFTFileURLCacheCheck(nft));
                //temp.debugErrorLog = true;
            }
            else
            {
                NFTsCheckedPlus();
            }
        }
        
        //Remove Null elements from List - used in continuation , where previous asset downloaders have ended
        AssetDlImages.RemoveAll(x => x == null);
    }


    //we check if file url is cached at nftport api as it provides faster download if available.
    string NFTFileURLCacheCheck(Nft nft)
    {
        string fileurl;
        if (nft.cached_file_url != null)
        {
            fileurl = nft.cached_file_url;
        }
        else
        {
            fileurl = nft.file_url;
        }

        return fileurl;
    }

    
    public List<AssetDownloader.GetImage> AssetDlImages = new List<AssetDownloader.GetImage>();
    public List<Nft> NftsWithGif = new List<Nft>(); // Useful to debug
    //Download Assets According to Content Type Gif or Texture for Main Frame.
    void HandleAssetContentType(Nft nft, string contentType)
    {
        //Unity Texture2D doesn't support Gif natively so we process it separately, if simple Image we use AssetDownloader.GetImage Feature in NFTPort SDK to get Texture2D.
        if (contentType.Contains("image") && !contentType.Contains("gif"))
        {
            var aDownloadGetImage = AssetDownloader.GetImage
                .Initialize()
                //.OnError(error => Debug.Log(error))
                .OnError(error => NFTsCheckedPlus())
                //.OnComplete(NFTtexture => LinkNFTtoGalleryFrame(NFTtexture))
                .OnCompleteReturnLinkedNft(nft,ReturnedNft => LinkNFTtoGalleryFrameTexture(ReturnedNft))
                //.OnAllAssetDownloadersDone(x => Debug.Log(x)) 
                .Download(NFTFileURLCacheCheck(nft), isIPFS: false);
        
            //Add to List, we use it stop and kill any AssetDownloaders if all frames are filled and any AssetDownloaders are still in progress.
            AssetDlImages.Add(aDownloadGetImage);
            
            Debug.Log("Downloading Textures from IPFS and the Web!!");
        }
        else if(contentType.Contains("gif")) 
        {
            NftsWithGif.Add(nft); //Useful for debugging

            if (_galleryController.DisplayGIFsOnNFTFrames)
            {
                var fileUrl = NFTFileURLCacheCheck(nft);
                if (_galleryReUsableAssets.CheckIfAssetAlreadyDownloadedUniGif(fileUrl))
                {
                    var UniGif = _galleryReUsableAssets.CheckIfAssetAlreadyDownloadedUniGif(fileUrl);
                    //We check if we have already downloaded this asset or are downloading,
                    if(UniGif.nowState == UniGifImage_PortMod.State.Playing || UniGif.nowState == UniGifImage_PortMod.State.Pause || UniGif.nowState == UniGifImage_PortMod.State.Ready)
                        //Already downloaded so link
                        LinkNFTtoGalleryFrameGIF(nft,UniGif);
                    else
                    {
                        //Add in que to be linked after downloaf
                        UniGif.AddNft(nft);
                    }
                }
                else
                {
                    // You can either use this or next function GifDownLoadLoop which limits many gif downloaders running parallel.
                    //A Modded UniGif to support nft classes and some actions. Original at : https://github.com/WestHillApps/UniGif
                    var UniGif = new GameObject("UniGifPlayer").AddComponent<UniGifImage_PortMod>();
                    UniGif.m_loadOnStartUrl = fileUrl;
                    UniGif
                        .OnComplete(nft, (Returnednft, UniGif) => LinkNFTtoGalleryFrameGIF(Returnednft, UniGif))
                        .OnError((errorStr, unigif) =>
                        {
                            _galleryReUsableAssets.UniGifRemove(unigif);
                            NFTsCheckedPlus();
                            //Debug.Log(errorStr);
                        })
                        .AddNft(nft)
                        .SetGifFromUrl(fileUrl, false)
                        .AutoKillIfLongDownload(45f);
                
                    _galleryReUsableAssets.ReUsableUniGifs.Add(UniGif);
                    /*
                    if(!GifLoopRunning)
                        GifDownLoadLoop();
                        
                        */
                    
                    Debug.Log("OOh You have GIFS too, hold tight, downloading! You can disable GIF downloads in gallery settings");
                }
        
                    
            }
        }
    }
    
    /*
    private bool GifLoopRunning;
    private int gifloopInt;
    
    public int GifDownloaderstoRunMultiplier;
    void GifDownLoadLoop()
    {
        GifLoopRunning = true;
        if (allFramesFilled)
            return;

        for (int i = 0; i < GifDownloaderstoRunMultiplier; i++)
        {
            if (NftsWithGif.Count > gifloopInt)
            {
                var nft = NftsWithGif[gifloopInt];
                var UniGif = new GameObject("UniGifPlayer").AddComponent<UniGifImage_PortMod>();
    
                //A Modded UniGif to support nft classes and some actions. Original at : https://github.com/WestHillApps/UniGif
                UniGif
                    .OnComplete(nft, (Returnednft, UniGif) => LinkNFTtoGalleryFrameGIF(nft, UniGif))
                    .OnError(x=>GifDownLoadLoop())
                    .SetGifFromUrl(nft.cached_file_url, false)
                    .AutoKillIfLongDownload(17f);
                gifloopInt++;
            }
        }
    }
*/
    #endregion


    #region Link Nfts with succesfull Assets for frames to NFT Frames
    //Link NFT to frame class type Gif
    void LinkNFTtoGalleryFrameGIF(Nft Nft, UniGifImage_PortMod UniGif)
    {
        if (allFramesFilled)
            return;
        
        GalleryNftFrames[framesFilled].FillNftGif(Nft, UniGif);
        framesFilled++;
        NFTsCheckedPlus();
        CheckFilledFrames();
        //GifDownLoadLoop();
    }
    //Link NFT to frame class type Texture2D
    void LinkNFTtoGalleryFrameTexture(Nft Nft)
    {
        if (allFramesFilled)
            return;

        GalleryNftFrames[framesFilled].FillNftTexture(Nft);
        framesFilled++;
        NFTsCheckedPlus();
        CheckFilledFrames();
    }
    #endregion


    #region Loopers and Checkers
    //Check if all frames are filled
    void CheckFilledFrames()
    {
        if (GalleryNftFrames.Length <= framesFilled)
        {
            allFramesFilled = true;
            StopAllImageAssetDownloaders();
            KillAnyUniGifsStillDownloading();
            Debug.Log("All Gallery Frames Filled");
        }
    }

    //Stop gallery loading at current state
    public void Stop()
    {
        Debug.Log("Stopping Asset Downloads");
        StopAllImageAssetDownloaders();
        KillAnyUniGifsStillDownloading();
        AssetDownloadersContentTypeForceEnd();
        StopGLTFs();
    }
    
    //For API Pagination ->
    [ReadOnly]public int NFTsCheckedinCurrentModel;
    [ReadOnly]public int CurrentNftsModelTotalCount;
    private string continuation = "";
    void NFTsCheckedPlus()
    {
        NFTsCheckedinCurrentModel++;
        if (!allFramesFilled)
        {
            if (NFTsCheckedinCurrentModel > CurrentNftsModelTotalCount - StartNextLoopWhenThisNumberofFramesifStillDownloading && CurrentNftsModelTotalCount == 50) //API Provides Max only 50 NFts per req, we use this to reloop next batch with a buffer of -8 for  any downloaders are going
            {
                //ContinuationLoopToNextBatch(continuation);
                if(!ContLoopWaiting)
                    StartCoroutine(Continuation());
            }
        }
        else
        {
            Debug.Log("All Gallery Frames Filled");
        }
    }
    
    private bool ContLoopWaiting = false;
    IEnumerator Continuation()
    {
        ContLoopWaiting = true;
        yield return new WaitForSeconds(WaitBeforeContinuation);
        Debug.Log("Continuation Loop");
        ContinuationLoopToNextBatch(continuation);
        ContLoopWaiting = false;
    }
    

    void KillAnyUniGifsStillDownloading()
    {
        for (int i = 0; i <= _galleryReUsableAssets.ReUsableUniGifs.Count; i++)
        {
            if (_galleryReUsableAssets.ReUsableUniGifs.Count > i) //because it may get destoyed , null check is imp.
            {
                if (_galleryReUsableAssets.ReUsableUniGifs[i].nowState == UniGifImage_PortMod.State.Loading)
                {
                    _galleryReUsableAssets.UniGifRemove(_galleryReUsableAssets.ReUsableUniGifs[i]);
                }
            }
        }
    }
    
    //Kill any AssetDownloaders if all frames filled
    void StopAllImageAssetDownloaders()
    {
        foreach (var AssetDlImage in AssetDlImages.ToList())
        {
            if (AssetDlImage != null)
            {
                AssetDlImage.End();
            }
            AssetDlImages.Remove(AssetDlImage);
        }
    }

    void StopGLTFs()
    {
        foreach (var nftFrame in GalleryNftFrames)
        {
            nftFrame.StopGLTF();
        }
    }

    void AssetDownloadersContentTypeForceEnd()
    {
        var x = FindObjectsOfType<AssetDownloader.DetemineURLContentType>();
        foreach (var i in x)
        {
            i.Stop();
        }
    }
    #endregion

}
