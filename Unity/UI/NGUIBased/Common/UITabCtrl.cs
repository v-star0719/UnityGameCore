//标签页控件
//可以在标签上指定对应的页面
namespace UIControl
{
    using UnityEngine;
    using System.Collections.Generic;

    //每个标签都包含UITabCellCtrl.cs组件，标签的切换操作由UITabCtrl完成
    //没有默认选择机制，请使用方手动选择。
    public class UITabCtrl : MonoBehaviour
    {
        private List<UITabCell> cellList = new List<UITabCell>();
        public List<EventDelegate> onSelect = new List<EventDelegate>();
        public bool initOnAwake = true;

        private int curSelect = -1;
        private bool isInited;

        public int CurSelect => curSelect;

        public UITabPage CurPage => cellList[curSelect].page;

        private void Awake()
        {
            if (initOnAwake)
            {
                Init();
            }
        }

        public void Init()
        {
            if(isInited)
            {
                return;
            }

            isInited = true;
            cellList.AddRange(GetComponentsInChildren<UITabCell>());
            for (var i = 0; i < cellList.Count; i++)
            {
                var cell = cellList[i];
                cell.Init(i, this);
                cell.Select(false);
            }
        }

        public void SelectTab(int index)
        {
            Init();
            if(curSelect != index)
            {
                if (curSelect >= 0)
                {
                    cellList[curSelect].Select(false);
                }

                if (index >= 0 && index < cellList.Count)
                {
                    cellList[index].Select(true);
                }
                curSelect = index;
                EventDelegate.Execute(onSelect);
            }
        }

        public void OnCellClicked(UITabCell cell)
        {
            SoundManager.Instance.Play("Audio_ClickNormal");
            SelectTab(cell.Index);
        }

        public void Clear()
        {
            for(var i = 0; i < cellList.Count; i++)
            {
                cellList[i].Clear();
            }
        }

        public UITabPage GetPage(int index)
        {
            return cellList[index].page;
        }
    }
}
