using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace GameCore.Unity
{
    //所有面板都会放到这个容器下。方便做通用的效果，比如动画、背景模糊
    public class PanelContainerCore : MonoBehaviour
    {
        //弹窗动画。如果动画缩放从0开始的，将面板上的值设置为1,1,1，并且动画disable，
        //这样start初始化时面板都是(1,1,1)的，排版可以正常进行。动画update后就变成(0,0,0)了，排版会出问题。
        public PlayableDirector PlayableDirector;

        public PanelBaseCore Panel { get; private set; }
        private Texture2D screenCapture;
        private BlurTextureMaker.MakingData makingData;

        public void Start()
        {
            if (Panel == null)
            {
                Debug.LogError(TransformUtils.GetTransformPath(transform,  null));
            }
            EnableBgTexture(Panel.blurBg);
            PlayableDirector.enabled = Panel.popUpAnimation;
            if (Panel.blurBg)
            {
                PlayableDirector.gameObject.SetActive(false);
                StartCoroutine(MakeBlurBg());
            }
        }

        public void AddPanel(PanelBaseCore panel)
        {
            Debug.Log("AddPanel " + panel.PanelName);
            gameObject.name = $"{panel.PanelName}[Container]";
            Panel = panel;
            panel.Container = this;
            panel.transform.SetParent(PlayableDirector.transform, false);
        }

        public void OnBgClick()
        {
            Panel.OnBgClick();
        }

        protected virtual void EnableBgTexture(bool e)
        {
        }

        protected virtual void SetBgTexture(Texture t)
        {
        }

        private IEnumerator MakeBlurBg()
        {
            yield return new WaitForEndOfFrame(); //截屏需要等一帧
            screenCapture = ScreenCapture.CaptureScreenshotAsTexture();
            SetBgTexture(screenCapture);
            makingData = BlurTextureMaker.Blur(screenCapture);
            PlayableDirector.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            if (screenCapture != null)
            {
                Destroy(screenCapture);
            }

            if (makingData != null && !makingData.IsFinished)
            {
                BlurTextureMaker.Stop(makingData);
                makingData = null;
            }
        }
    }
}