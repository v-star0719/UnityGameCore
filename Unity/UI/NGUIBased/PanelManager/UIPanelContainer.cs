using Kernel.Core;
using Kernel.Unity;
using UnityEngine;

public class UIPanelContainer : PanelContainerCore
{
    public UITexture bgTexture;

    protected override void EnableBgTexture(bool e)
    {
        bgTexture.enabled = e;
    }

    protected override void SetBgTexture(Texture t)
    {
        bgTexture.mainTexture = t;
    }
}
