using GameCore.Core;
using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    public class UIPanelBase : PanelBaseCore
    {
        public override int Depth
        {
            get => depth;
            set
            {
                depth = value;
                ((UIPanelContainer)container).canvas.sortingOrder = depth;
            }
        }

        public override void Open(PanelManagerCore mgr, params object[] args)
        {
            base.Open(mgr, args);
        }

        public void OnBackBtnClick()
        {
            Close(false);
        }

        public override void SetPosition(Vector3 pos)
        {
            transform.position = pos;
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