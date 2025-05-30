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

        public bool IsCapturingScreen { get; private set; }//����Ҫ�ȵ�֡�������л�������ʱ��Ҫ��һ��

        protected Argument arg;

        protected override void OnOpen(params object[] args)
        {
            arg = args[0] as Argument;
            bgImage.gameObject.SetActive(arg.bgType == LoadingBgType.Texture || arg.bgType == LoadingBgType.ScreenShot);
            switch (arg.bgType)
            {
                case LoadingBgType.Texture:
                    bgImage.material = null; //������Կռ䣬����Ҫ�Ҳ������ͼת��srgb�ռ䣬���ص���ͼ�ò��������
                    bgImage.SetTexture(arg.bgTextureRes);
                    var size = UGUIUtils.GetViewSize(RectTransform);
                    UGUIUtils.RectFitView(bgImage.rectTransform, size);
                    break;
                case LoadingBgType.ScreenShot:
                    bgImage.enabled = false;
                    bgImage.rectTransform.sizeDelta = UGUIUtils.GetViewSize(RectTransform);
                    IsCapturingScreen = true;
                    StartCoroutine(CaptureScreen());//��������Կռ䣬��ͼ������Ҳ�����Կռ��
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
            yield return new WaitForEndOfFrame(); //������Ҫ��һ֡
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
