using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class UIPanelContainer: PanelContainerCore
    {
        public Canvas canvas;
        public RawImage bgTexture;
        public Color grayBgColor = new(0f, 0f, 0f, 0.5f);

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

        protected override void ShowBg()
        {
            var bgType = Panel.settings.bgType;
            bgTexture.gameObject.SetActive(bgType != PanelBaseCore.BgType.None);
            switch(bgType)
            {
                case PanelBaseCore.BgType.Gray:
                    bgTexture.color = grayBgColor;
                    break;
                case PanelBaseCore.BgType.Empty:
                    bgTexture.color = new(0, 0, 0, 0);
                    break;
            }
        }
    }
}

