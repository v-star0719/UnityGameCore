using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class UIPanelContainerUGUI : PanelContainerCore
    {
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

