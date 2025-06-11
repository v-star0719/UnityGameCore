using System;
using System.Collections.Generic;
using GameCore.Core;
using UnityEngine;

namespace GameCore.Unity
{
    public class PanelBaseCore : MonoBehaviour
    {
        public enum BgType
        {
            None,
            Gray,
            Blur,
            Empty,
        }

        [Serializable]
        public class Settings
        {
            public bool isFullPanel; //全屏界面会遮挡主底部的UI
            public bool blurBg = true; //显示模糊被背景，否则隐藏背景
            public bool playPopUpAnimation = true;
            public bool clickBgClose = true;
            public BgType bgType = BgType.Blur;
            public bool closeByCancelAxis = true;
            public bool closeBySubmitAxis = false;

            public bool playOpenSound = true;
            public bool playCloseSound = true;
        }

        protected int depth;
        protected PanelManagerCore manager;
        protected PanelContainerCore container;

        public Settings settings;

        public bool IsClosed { get; private set; }
        public string PanelName { get; set; }
        public float OpenTiming { get; private set; }
        public float Age => Time.time - OpenTiming;

        private Dictionary<Enum, Action<object>> events = new();

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
            OpenTiming = Time.time;
            try
            {
                OnOpen(args);
                if (settings.playOpenSound)
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
                if (settings.playCloseSound)
                {
                    PlayCloseSound();
                }

                foreach (var ev in events)
                {
                    EventManager.Inst.UnsubscribeEvent(ev.Key, ev.Value);
                }
                events.Clear();
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

        public virtual bool CanClose()
        {
            return true;
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

        public virtual void SetPosition(Vector3 pos)
        {
            container.transform.position = pos;
        }

        public virtual void OnBgClick()
        {
            if (settings.clickBgClose)
            {
                Close(false);
            }
        }

        //回车键键或者手柄的确认键
        public virtual void OnSubmitAxis()
        {
            if (settings.closeBySubmitAxis)
            {
                Close(false);
            }
        }

        //ESC键或者手柄的取消键
        public virtual void OnCancelAxis()
        {
            if (settings.closeByCancelAxis)
            {
                Close(false);
            }
        }

        public void SubscribeEvent(Enum e, Action<object> action)
        {
            EventManager.Inst.SubscribeEvent(e, action);
            events.Add(e, action);
        }
    }
}