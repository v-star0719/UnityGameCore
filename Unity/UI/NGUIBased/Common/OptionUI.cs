using System;
using UIControl;
using UnityEngine;

namespace UI
{
    public class OptionUI : MonoBehaviour
    {
        public UILabel buttonLbl;
        public UITabCtrl tabCtrl;//使用第一个标签作为预设

        private UITweener[] tweeners;//正向是展开
        private string[] options;
        private Action<int> onSelect;
        private bool isShowed;

        public void Init(string[] options, Action<int> onSelect)
        {
            this.options = options;
            this.onSelect = onSelect;
            tweeners = GetComponentsInChildren<UITweener>();

            var prefab = tabCtrl.transform.GetChild(0).gameObject;
            for (int i = 1; i < options.Length; i++)
            {
                var go = Instantiate(prefab);
                go.transform.SetParent(tabCtrl.transform, false);
                go.GetComponentInChildren<UILabel>().text = options[i];
            }
            prefab.GetComponentInChildren<UILabel>().text = options[0];
            tabCtrl.transform.localScale = Vector3.one;//先恢复，计算位置
            tabCtrl.GetComponent<UITable>().Reposition();
            tabCtrl.transform.localScale = new Vector3(1, 0, 1);//再变回
        }

        public void Select(int s)
        {
            tabCtrl.SelectTab(s);
            buttonLbl.text = options[s];
        }

        public void OnTabSelect()
        {
            buttonLbl.text = options[tabCtrl.CurSelect];
            onSelect(tabCtrl.CurSelect);
            if (isShowed)
            {
                OnButtonClick();
            }
        }

        public void OnButtonClick()
        {
            if (isShowed)
            {
                isShowed = false;
                NGUIUtils.PlayTweens(tweeners, false);
            }
            else
            {
                isShowed = true;
                NGUIUtils.PlayTweens(tweeners, true);
            }
        }

        public void OnBgClick()
        {
            OnButtonClick();
        }
    }
}
