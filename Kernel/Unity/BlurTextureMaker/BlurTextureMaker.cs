using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlurTextureMaker : MonoBehaviour
{
    public int iteration = 10;
    public Vector4 size = new Vector4(0.1f, 0.1f);
    public float offset = 1;

    private static BlurTextureMaker instance;
    public Material material;
    public RenderTexture renderTexture;//一大一小两张图来回进行模糊
    public RenderTexture halfRenderTexture;//一大一小两张图来回进行模糊

    public void Start()
    {
        //DualGaussianBlur有两个pass，Graphics.Blit同时处理两个通道会出不来图像。
        material = new Material(Shader.Find("CustomPostEffect/KawaseBlur"));
    }

    public void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            renderTexture = null;
        }
    }

    public IEnumerator Process(Texture texture)
    {
        yield return null;//等待start处理完

        if (renderTexture == null || (renderTexture.width != texture.width || renderTexture.height != texture.height))
        {
            renderTexture?.Release();
            halfRenderTexture?.Release();
            renderTexture = new RenderTexture(texture.width, texture.height, 0);
            halfRenderTexture = new RenderTexture(texture.width >> 2, texture.height >> 2, 0);
        }

        //复制到RenderTexture上准备干活
        Graphics.CopyTexture(texture, renderTexture);

        material.SetTexture("_MainTex", texture);
        material.SetFloat("_Offset", offset);
        material.SetVector("_MainTex_TexelSize", size);

        for (int i = 0; i < iteration; i++)
        {
            if ((i & 1) == 0)
            {
                Graphics.Blit(renderTexture, halfRenderTexture, material);
            }
            else
            {
                Graphics.Blit(halfRenderTexture, renderTexture, material);
                Graphics.CopyTexture(renderTexture, texture);//预览
            }
            yield return null;
        }

        if ((iteration & 1) == 1)
        {
            //奇数次的时候，结果在halfRenderTexture上
            Graphics.Blit(halfRenderTexture, renderTexture);
        }
        Graphics.CopyTexture(renderTexture, texture);
    }
    
    #region 手动模糊
    public static void Process2(Texture2D texture)
    {
        var datas = texture.GetPixels32();
        var tempTexture = new Texture2D(texture.width, texture.height);
        var datas2 = tempTexture.GetPixels32();
        for(int i = 0; i < tempTexture.width; i++)
        {
            for(int j = 0; j < tempTexture.height; j++)
            {
                //datas2[j * texture.width + i] = datas[j * texture.width + i];
                datas2[j * texture.width + i] = SampleDown(datas, i, j, texture.width, texture.height);
            }
        }
        for(int i = 0; i < tempTexture.width; i++)
        {
            for(int j = 0; j < tempTexture.height; j++)
            {
                //datas2[j * texture.width + i] = datas[j * texture.width + i];
                datas2[j * texture.width + i] = SampleUp(datas, i, j, texture.width, texture.height);
            }
        }
        texture.SetPixels32(datas2);
        texture.Apply();
    }

    private static Color SampleDown(Color32[] color32s, int i, int j, int w, int h)
    {
        var dir = new Vector2Int(2, 2);
        var c0 = SampleDownHelp(color32s, i, j, w, h);
        var c1 = SampleDownHelp(color32s, i + dir.x, j + dir.y, w, h);
        var c2 = SampleDownHelp(color32s, i + dir.x, j - dir.y, w, h);
        var c3 = SampleDownHelp(color32s, i - dir.x, j + dir.y, w, h);
        var c4 = SampleDownHelp(color32s, i - dir.x, j - dir.y, w, h);
        return c0 * 0.5f + (c1 + c2 + c3 + c4) * 0.125f;
    }

    private static Color SampleUp(Color32[] color32s, int i, int j, int w, int h)
    {
        var dir = new Vector2Int(2, 2);
        var dir2 = new Vector2Int(4, 4);
        var c1 = SampleDownHelp(color32s, i + dir.x, j + dir.y, w, h) * 2;
        var c2 = SampleDownHelp(color32s, i + dir.x, j - dir.y, w, h) * 2;
        var c3 = SampleDownHelp(color32s, i - dir.x, j + dir.y, w, h) * 2;
        var c4 = SampleDownHelp(color32s, i - dir.x, j - dir.y, w, h) * 2;
        var c5 = SampleDownHelp(color32s, i + dir2.x, j + dir2.y, w, h);
        var c6 = SampleDownHelp(color32s, i + dir2.x, j - dir2.y, w, h);
        var c7 = SampleDownHelp(color32s, i - dir2.x, j + dir2.y, w, h);
        var c8 = SampleDownHelp(color32s, i - dir2.x, j - dir2.y, w, h);
        return (c1 + c2 + c3 + c4 + c5 + c6 + c7 + c8) * 0.0833f;
    }

    private static  Color SampleDownHelp(Color32[] color32s, int i, int j, int w, int h)
    {
        i = Mathf.Clamp(i, 0, w-1);
        j = Mathf.Clamp(j, 0, h-1);
        return color32s[j * w + i];
    }
    #endregion

    //texture会被覆盖，如果不想被覆盖，请手动备份
    public static void Blur(Texture2D texture)
    {
        if (!texture.isReadable)
        {
            Debug.LogError("input texture in not readable.");
            return;
        }

        if (instance == null)
        {
            var go = new GameObject("BlurTextureMaker");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<BlurTextureMaker>();
        }

        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.Process(texture));
    }
}
