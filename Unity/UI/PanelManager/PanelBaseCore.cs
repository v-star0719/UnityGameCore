using System;
using UnityEngine;

namespace GameCore.Unity
{
    public class PanelBaseCore : MonoBehaviour
    {
        [Serializable]
        public class Settings
        {
            public bool isFullPanel; //全屏界面会遮挡主底部的UI
            public bool blurBg = true;
            public bool playPopUpAnimation = true;
            public bool clickBgClose = true;

            public bool playOpenSound = true;
            public bool playCloseSound = true;
        }

        protected int depth;
        protected PanelManagerCore manager;
        protected PanelContainerCore container;

        public Settings settings;

        public bool IsClosed { get; private set; }
        public string PanelName { get; set; }

        public bool isFullPanel; //全屏界面会遮挡主底部的UI
        public bool blurBg = true;
        public bool popUpAnimation = true;
        public bool clickBgClose = true;
        public bool playOpenSound = true;
        public bool playCloseSound = true;

        public virtual int Depth
        {
            get => depth;
            set => depth = value;
        }

        public PanelContainerCore Container
        {
            get => container;
            set => container = value;
        }

        public virtual void Open(PanelManagerCore mgr, params object[] args)
        {
            manager = mgr;
            try
            {
                OnOpen(args);
                if (playOpenSound)
                {
                    PlayOpenSound();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public virtual void Close(bool isCloseByMgr = false)
        {
            if (isCloseByMgr)
            {
                OnClose();
                IsClosed = true;
                GameObject.Destroy(container.gameObject);
                manager.onPanelClose?.Invoke(this);
                if (playCloseSound)
                {
                    PlayCloseSound();
                }
            }
            else
            {
                manager.ClosePanel(PanelName);
            }
        }

        protected virtual void OnOpen(params object[] args)
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void PlayOpenSound()
        {
        }

        protected virtual void PlayCloseSound()
        {
        }

        public void Show()
        {
            if (Container != null)
            {
                Container.gameObject.SetActive(true);
            }
            else
            {
                gameObject.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (Container != null)
            {
                Container.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void SetPosition(Vector3 pos)
        {
            container.transform.position = pos;
        }

        public virtual void OnBgClick()
        {
            if (clickBgClose)
            {
                Close(false);
            }
        }
    }
}