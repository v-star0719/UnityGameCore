using System;
using UnityEngine;
using UnityEngine.Playables;

namespace GameCore.Unity.UGUIEx
{
    public class UIOptionUI : MonoBehaviour
    {
        public UnityEngine.UI.Text buttonText;
        public UITabCtrl tabCtrl;//使用第一个标签作为预设
        public Transform bottomMark;
        public PlayableDirectorEx director;//正向是展开，如果没有的话，直接显示隐藏
        private UITweener[] tweeners;
        private string[] options;
        private Action<int> onSelect;
        private bool isShowed;

        public void Init(string[] options, Action<int> onSelect)
        {
            this.options = options;
            this.onSelect = onSelect;

            var prefab = tabCtrl.transform.GetChild(0).gameObject;
            for (int i = 1; i < options.Length; i++)
            {
                var go = Instantiate(prefab);
                go.transform.SetParent(tabCtrl.transform, false);
                go.GetComponentInChildren<UILabel>().text = options[i];
            }
            prefab.GetComponentInChildren<UILabel>().text = options[0];
            tabCtrl.transform.localScale = Vector3.one;//先恢复，计算位置
            bottomMark.SetAsLastSibling();
            tabCtrl.GetComponent<UITable>().Reposition();
            tabCtrl.transform.localScale = new Vector3(1, 0, 1);//再变回
        }

        public void Select(int s)
        {
            tabCtrl.SelectTab(s);
            buttonText.text = options[s];
        }

        public void OnTabSelect()
        {
            buttonText.text = options[tabCtrl.CurSelect];
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
                director?.PlayReverse();
            }
            else
            {
                isShowed = true;
                director?.PlayForward();
            }
        }

        public void OnBgClick()
        {
            OnButtonClick();
        }
    }
}
