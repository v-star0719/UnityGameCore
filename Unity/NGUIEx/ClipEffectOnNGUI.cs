using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NGUI

//通过设置特效shader的_ScreenClip参数来裁剪特效
public class ClipEffectOnNGUI : MonoBehaviour
{
    private UIPanel panel;

    private Renderer renderer;

    private Vector3 lastBL;
    private Vector3 lastTR;
    // Start is called before the first frame update
    void Start()
    {
        var p = GetComponentInParent<UIPanel>();
        if (p.hasClipping)
        {
            panel = p;
            renderer = GetComponentInChildren<Renderer>();
        }
        else
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var corners = panel.worldCorners;
        var bl = corners[0];
        var tr = corners[1];
        if (lastBL == bl && tr == lastTR)
        {
            enabled = false;
            return;
        }

        lastBL = bl;
        lastTR = tr;

        var camera = panel.anchorCamera;
        bl = camera.WorldToScreenPoint(corners[0]);
        tr = camera.WorldToScreenPoint(corners[2]);
        var w = 1f / Screen.width;
        var h = 1f / Screen.height;
        bl.x = bl.x * w;
        bl.y = bl.y * h;
        tr.x = tr.x * w;
        tr.y = tr.y * h;

        var matrls = renderer.materials;
        for (int i = 0; i < matrls.Length; i++)
        {
            matrls[i].SetVector("_ScreenClip", new Vector4(bl.x, bl.y, tr.x, tr.y));
        }
    }
}

#endif