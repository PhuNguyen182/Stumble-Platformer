using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestExportToPNG : MonoBehaviour
{
    public RenderTexture renderTexture;
    public RawImage rawImage;

    private void Start()
    {
        ShotRenderTexture();
    }

    private void ShotRenderTexture()
    {
        Texture2D tex2D = (Texture2D)rawImage.mainTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        Color[] colors = tex2D.GetPixels();
        tex.SetPixels(colors);
    }
}
