using UnityEngine;

namespace UIControl
{
    public class UITabPage : MonoBehaviour
    {
        private int visiable = -1;//让默认值是-1，这样初始化就可以用调用Hide隐藏界面
        public bool IsVisible => visiable == 1;

        public virtual void Init()
        {
            OnInit();
        }

        public virtual void Refresh()
        {
            OnRefresh();
        }

        public virtual void Show()
        {
            if (visiable == 1)
            {
                return;
            }

            visiable = 1;
            gameObject.SetActive(true);
            OnShow();
        }

        public virtual void Hide()
        {
            if (visiable == 0)
            {
                return;
            }

            visiable = 0;
            gameObject.SetActive(false);
            OnHide();
        }

        public virtual void Clear()
        {
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnRefresh()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnClear()
        {
        }
    }
}
