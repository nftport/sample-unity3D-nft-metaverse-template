using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using GLTFast.Logging;
using NFTPort;
using UnityEngine;
using UnityEngine.UI;

public class GalleryNFT_frame : MonoBehaviour
{
    //////â‰§â— â€¿â— â‰¦âœŒ _sz_ Î© //≧◠‿◠≦✌ _sz_ Ω embracingearth.space
    public Nft nft;

    [Space] public RawImage NftCanvas;
    [SerializeField] private GameObject ThreeDRoot;
    [SerializeField] private GameObject NFTFrame;
    [SerializeField] private Text NameText;
    [SerializeField] private Text TokenIDText;
    [SerializeField] private Text AttibutesText;
    [SerializeField] private GameObject threeDScaleTo;
  
    private GalleryController _galleryController;

    public bool filled = false;

    private void Awake()
    {
        _galleryController = FindObjectOfType<GalleryController>();
    }

    public void  FillNftTexture( Nft nft)
    {
        if(filled)
            return;
        
        if (nft.assets.image_texture != null)
        {
            this.nft = nft;
            NftCanvas.texture = nft.assets.image_texture;
            filled = true;
        } 
        
        AfterNFTFrameFill();
    }

    private UniGifImage_PortMod UniGif;
    public void  FillNftGif( Nft nft, UniGifImage_PortMod UniGif)
    {
        if(filled)
            return;
        
        this.nft = nft;
        this.UniGif = UniGif;
        UniGif.m_rawImage.Add(NftCanvas);
        if(UniGif.nowState==UniGifImage_PortMod.State.Ready)
            UniGif.Play();
        filled = true;

        AfterNFTFrameFill();
    }

    void AfterNFTFrameFill()
    {
        NFTFrame.SetActive(true);
        FillUI();
        //Right now only support for 3D glbs but here under HandleAssetContentType() you can also check for other things like video/audio and more.
        if(_galleryController.Display3DAssetsIfAvailableatAnimationinMetadata)
            ShowAnimationAsset();
    }

    void FillUI()
    {
        //Name
        if(nft.name!=null)
            NameText.text = nft.name;
        else if(nft.metadata.name!=null)
        {
            NameText.text = nft.metadata.name;
        }
        else
        {
            NameText.text = nft.token_id;
        }
        
        //TokenID
        TokenIDText.text = nft.token_id;

        //Attributes
        if (nft.metadata.attributes != null || nft.metadata.traits != null)
        {
            foreach (var trait in nft.metadata.traits)
            {
                AttibutesText.text +=  " | " + trait.trait_type + ": " + trait.value;
            }
            foreach (var attribute in nft.metadata.attributes)
            {
                AttibutesText.text +=  " | " + attribute.trait_type + ": " + attribute.value;
            }
        }
    }

    public void Reset()
    {
        if(UniGif)
            UniGif.m_rawImage.Remove(NftCanvas);
        nft = null;
        NftCanvas.texture = null;
        filled = false;
        Post3DDownloadReset();
        NFTFrame.SetActive(false);
        NameText.text = null;
        TokenIDText.text = null;
        AttibutesText.text = null;
        if(gltf1)
            gltf1.Dispose();
        
        threeDScaleTo.SetActive(false);
    }

    public void EnableDefaultVisuals()
    {
        threeDScaleTo.SetActive(true);
        NFTFrame.SetActive(true);
    }

    #region 3DGlb
    string animationUrl;
    void ShowAnimationAsset()
    {
        if (nft.cached_animation_url != null )
        {
            animationUrl = nft.cached_animation_url;
        }
        else if (nft.animation_url != null)
        {
            animationUrl = nft.animation_url;
        }
        else
        {
            animationUrl = null;
            return;
        }
        
        if (animationUrl.Contains("ipfs://")) //check just in case user is passing ipfs strings
                animationUrl = "https://cloudflare-ipfs.com/ipfs/" + animationUrl.Replace("ipfs://", "");
        
        
        var temp = AssetDownloader.DetemineURLContentType
            .Initialize()
            .OnError(error => Debug.Log("Asset Downloader: " + error))
            .OnComplete((nft, contentType) => HandleAssetContentType(contentType))
            .Run(nft, animationUrl);
            
        LoadFromUrl(animationUrl);
    }

    void HandleAssetContentType(string contentType)
    {
        if(contentType.Contains("gltf") || contentType.Contains("application/json"))
            LoadFromUrl(animationUrl);
    }
    
    public async void LoadFromUrl(string url) => await LoadUrl(url);
    //For GLTF
    //GLTFast.IDeferAgent deferAgent;
    //GLTFast.UninterruptedDeferAgent deferAgent;
    GLTFast.TimeBudgetPerFrameDeferAgent deferAgent;
    /// TimeBudgetPerFrameDeferAgent for stable frame rate
    ///UninterruptedDeferAgent for fastest, uninterrupted loading
    /// </summary>
    GltfAsset gltf1;

    private ICodeLogger logger;
    public async Task LoadUrl(string url)
    {
        Debug.Log("Found some 3D assets in your NFTs Meta, hold tight , downloading! They'll be visible in front of your NFT Frames.");
        
        if (ThreeDRoot.GetComponent<GltfAsset>())
        {
            gltf1  = ThreeDRoot.GetComponent<GltfAsset>();
        }
        else
        {
            gltf1 = this.ThreeDRoot.AddComponent<GltfAsset>();
        }
        gltf1.loadOnStartup = false;
        var success = await gltf1.Load(url,null,deferAgent, null, logger);
        //loadingEnd?.Invoke();
        
        if(success) {
            if (!gltf1.currentSceneId.HasValue && gltf1.sceneCount > 0) {
                // Fallback to first scene
                //Debug.LogWarning("glTF has no main scene. Falling back to first scene.");
                gltf1.InstantiateScene(0);
            }
            Post3DDownloadSuccess();
            //GLTFast_onLoadComplete(gltf1); 
        }
        else
        {
            Post3DDownloadReset();
        }
    }

    public Renderer[] glbrenderer;
    void Post3DDownloadSuccess()
    {
        //Scaling the 3D GLB
        var x = GetLocalBoundsForObject(ThreeDRoot);
        var d = (Math.Max(Math.Max(x.extents.x, x.extents.y), x.extents.z));
        if(d>1)
            ThreeDRoot.transform.localScale = new Vector3(1 / d, 1 / d, 1 / d);
        else if(d==0)
        {
            ThreeDRoot.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        }
        else
        {
            ThreeDRoot.transform.localScale = new Vector3(d, d, d);
        }

        //ThreeDRoot.GetComponentInChildren<Transform>().localScale = sizeCalculated;
        //threeDScaleTo.SetActive(false);
    }
    static Bounds GetLocalBoundsForObject(GameObject go)
    {
        var referenceTransform = go.transform;
        var b = new Bounds(Vector3.zero, Vector3.zero);
        RecurseEncapsulate(referenceTransform, ref b);
        return b;
                       
        void RecurseEncapsulate(Transform child, ref Bounds bounds)
        {
            var mesh = child.GetComponent<MeshFilter>();
            if (mesh)
            {
                var lsBounds = mesh.sharedMesh.bounds;
                var wsMin = child.TransformPoint(lsBounds.center - lsBounds.extents);
                var wsMax = child.TransformPoint(lsBounds.center + lsBounds.extents);
                bounds.Encapsulate(referenceTransform.InverseTransformPoint(wsMin));
                bounds.Encapsulate(referenceTransform.InverseTransformPoint(wsMax));
            }
            foreach (Transform grandChild in child.transform)
            {
                RecurseEncapsulate(grandChild, ref bounds);
            }
        }
    }

    public void StopGLTF()
    {
        Destroy(gltf1);
    }
    
    void Post3DDownloadReset()
    {
        if(gltf1)
            gltf1.ClearScenes();
        
        for (var i = ThreeDRoot.transform.childCount - 1; i >= 0; i--)
        {
            //Destroy(ThreeDRoot.transform.GetChild(i).transform.gameObject);
        }
    }
    

    #endregion
    
}
