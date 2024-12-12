using GameCore.Core;
using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    public class UIPanelBase : PanelBaseCore
    {
        private Canvas canvas;

        public override int Depth
        {
            get => depth;
            set
            {
                depth = value;
                if(canvas == null)
                {
                    LoggerX.Error("panel 尚未初始化");
                    return;
                }

                canvas.sortingOrder = depth;
            }
        }

        public override void Open(PanelManagerCore mgr, params object[] args)
        {
            canvas = container.GetComponent<Canvas>();
            base.Open(mgr, args);
        }

        public void OnBackBtnClick()
        {
            Close(false);
        }

        protected override void PlayOpenSound()
        {
            SoundManager.Instance.PlayNormalSound("Audio_PanelOpen");
        }

        protected override void PlayCloseSound()
        {
            SoundManager.Instance.PlayNormalSound("Audio_PanelClose");
        }
    }
}