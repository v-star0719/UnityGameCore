using System;
using System.Collections;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class LoadingPanelBase : UIPanelBase
    {
        public RawImage bgImage;
        public Slider progressSlider;
        public TMP_Text progressText;

        public bool IsCapturingScreen { get; private set; }//截屏要等到帧结束，切换场景的时候要等一下

        protected Argument arg;

        protected override void OnOpen(params object[] args)
        {
            arg = args[0] as Argument;
            bgImage.gameObject.SetActive(arg.bgType == LoadingBgType.Texture || arg.bgType == LoadingBgType.ScreenShot);
            switch (arg.bgType)
            {
                case LoadingBgType.Texture:
                    bgImage.material = null; //如果线性空间，上面要挂材质球把图转到srgb空间，加载的贴图用不到这个。
                    bgImage.SetTexture(arg.bgTextureRes);
                    var size = UGUIUtils.GetViewSize(RectTransform);
                    UGUIUtils.RectFitView(bgImage.rectTransform, size);
                    break;
                case LoadingBgType.ScreenShot:
                    bgImage.enabled = false;
                    bgImage.rectTransform.sizeDelta = UGUIUtils.GetViewSize(RectTransform);
                    IsCapturingScreen = true;
                    StartCoroutine(CaptureScreen());//如果是线性空间，截图出来是也是线性空间的
                    break;
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (arg.bgType == LoadingBgType.ScreenShot)
            {
                Destroy(bgImage.texture);
            }
        }

        private IEnumerator CaptureScreen()
        {
            yield return new WaitForEndOfFrame(); //截屏需要等一帧
            bgImage.enabled = true;
            bgImage.texture = ScreenCapture.CaptureScreenshotAsTexture();
            IsCapturingScreen = false;
        }

        public class Argument
        {
            public LoadingBgType bgType;
            public string bgTextureRes;
            public Func<float> progressGetter;

            public static Argument TextureBg(string textureRes, Func<float> progressGetter = null)
            {
                return new Argument()
                {
                    bgType = LoadingBgType.Texture,
                    bgTextureRes = textureRes,
                    progressGetter = progressGetter
                };
            }

            public static Argument ScreenShot(Func<float> progressGetter = null)
            {
                return new Argument()
                {
                    bgType = LoadingBgType.ScreenShot,
                    progressGetter = progressGetter
                };
            }
        }

        public enum LoadingBgType
        {
            ScreenShot,
            Texture,
            Spine,
        }
    }
}
