using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryReUsableAssets : MonoBehaviour
{


    public List<UniGifImage_PortMod> ReUsableUniGifs = new List<UniGifImage_PortMod>();
    public UniGifImage_PortMod CheckIfAssetAlreadyDownloadedUniGif(string url)
    {
        foreach (var reUse in ReUsableUniGifs)
        {
            if (url == reUse.m_loadOnStartUrl)
            {
                return  reUse;
            }
            else
            {
                return null;
            }
        }
        return null;
    }
    
    public void UniGifClearCanvases()
    {
        foreach (var reUse in ReUsableUniGifs)
        {
            reUse. m_rawImage.Clear();
        }
    }

    public void UniGifRemove(UniGifImage_PortMod UniGif)
    {
        foreach (var reUse in ReUsableUniGifs)
        {
            if (UniGif == reUse)
            {
                ReUsableUniGifs.Remove(reUse);
                Destroy(reUse.gameObject);
                break;
            }
        }
    }

    public UniGifImage_PortMod UniGifDLCompleted(UniGifImage_PortMod UniGif)
    {
        foreach (var reUse in ReUsableUniGifs)
        {
            if (UniGif == reUse)
            {
                return  reUse;
            }
            else
            {
                return null;
            }
        }
        return null;
    }
}
