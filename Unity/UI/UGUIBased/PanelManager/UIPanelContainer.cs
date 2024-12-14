using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class UIPanelContainer: PanelContainerCore
    {
        public Canvas canvas;
        public RawImage bgTexture;

        protected override void EnableBgTexture(bool e)
        {
            bgTexture.enabled = e;
        }

        protected override void SetBgTexture(Texture t)
        {
            bgTexture.texture = t;
        }
    }
}

