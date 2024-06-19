using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class UIPanelContainer : MonoBehaviour
{
    public UITexture bgTexture;
    //弹窗动画。如果动画缩放从0开始的，将面板上的值设置为1,1,1，并且动画disable，
    //这样start初始化时面板都是(1,1,1)的，排版可以正常进行。动画update后就变成(0,0,0)了，排版会出问题。
    public PlayableDirector PlayableDirector;

    public UIPanelBaseCore Panel { get; private set; }
    private Texture2D screenCapture;

    public void Start()
    {
        bgTexture.enabled = Panel.blurBg;
        PlayableDirector.enabled = Panel.popUpAnimation;
        if (Panel.blurBg)
        {
            PlayableDirector.gameObject.SetActive(false);
            StartCoroutine(RecordFrame());
        }
    }

    public void AddPanel(UIPanelBaseCore panel)
    {
        gameObject.name = $"{panel.PanelName}[Container]";
        Panel = panel;
        panel.Container = gameObject;
        panel.transform.SetParent(PlayableDirector.transform, false);
    }

    public void OnClick()
    {
        Panel.OnBgClick();
    }

    private IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();//截屏需要等一帧
        screenCapture = ScreenCapture.CaptureScreenshotAsTexture();
        bgTexture.mainTexture = screenCapture;
        BlurTextureMaker.Blur(screenCapture);
        PlayableDirector.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (screenCapture != null)
        {
            Destroy(screenCapture);
        }
    }
}
