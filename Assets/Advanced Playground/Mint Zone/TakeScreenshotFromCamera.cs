using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class TakeScreenshotFromCamera : MonoBehaviour
{
    public int resWidth = 200; 
    public int resHeight = 200;

    public new Camera camera;


    public bool CreateAscreenShot(string savePath)
    {
        camera.gameObject.SetActive(true);
        RenderTexture rt = new RenderTexture(resWidth, resHeight, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32, GraphicsFormat.D32_SFloat);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        camera.targetTexture = null;
        camera.gameObject.SetActive(false);
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath, bytes);
        Debug.Log(string.Format("Took NFTImage to: {0}", savePath));
        return true;
    }
}
