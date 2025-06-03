#if NGUI
namespace GameCore.Unity.NGUIEx
{
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
}
#endif