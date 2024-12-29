using System.Collections;
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
            var c = bgTexture.color;
            c.a = e ? 1 : 0;
            bgTexture.color = c;
        }

        protected override void SetBgTexture(Texture t)
        {
            bgTexture.texture = t;
        }

        protected override void HideBgTexture(bool b)
        {
            bgTexture.gameObject.SetActive(!b);
        }
    }
}

