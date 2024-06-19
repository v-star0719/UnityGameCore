using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class BlurTextureTest : MonoBehaviour
{
    public RawImage image;
    public Texture2D texture;
    [Header("或者按T键")]
    public bool btn = true;

    public void Update()
    {
        if (btn || Input.GetKeyUp(KeyCode.T))
        {
            btn = false;
            var tex = new Texture2D((int)(texture.width), (int)(texture.height), TextureFormat.ARGB32, false);
            Graphics.CopyTexture(texture, tex);
            image.texture = tex;
            BlurTextureMaker.Blur(tex);
        }
    }
}
