using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    public class UIOptionUI : MonoBehaviour
    {
        public TMP_Text buttonText;
        public UIGridEx gridEx;//使用第一个标签作为预设
        public PlayableDirectorEx director;//正向是展开，如果没有的话，直接显示隐藏
        private string[] options;
        private Action<int> onSelect;
        private bool isShowed;

        public void Init(string[] options, Action<int> onSelect)
        {
            this.options = options;
            this.onSelect = onSelect;

            var datas = new List<UIOptionItem.Data>();
            for (int i = 0; i < options.Length; i++)
            {
                datas.Add(new UIOptionItem.Data()
                {
                    id = i,
                    text = options[i],
                });
            }
            gridEx.Set(datas);
            director.SetToBegin();
        }

        public void Select(int s)
        {
            gridEx.SelectItem(s, false, false, false);
            buttonText.text = options[s];
        }

        public void OnItemSelect(UIOptionItem uiOptionItem)
        {
            buttonText.text = uiOptionItem.data.text;
            onSelect(uiOptionItem.data.id);
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
