using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

//标签页控件
//可以在标签上指定对应的页面
namespace GameCore.Unity.UGUIEx
{
    //每个标签都包含UITabCellCtrl.cs组件，标签的切换操作由UITabCtrl完成
    //没有默认选择机制，请使用方手动选择。
    public class UITabCtrl : MonoBehaviour
    {
        private List<UITabCell> cellList = new List<UITabCell>();
        public UnityEvent onSelect;
        public bool canDeselect;
        public bool initOnAwake = true;

        private int curSelect = -11111;
        private bool isInited;

        public int CurSelect => curSelect;

        public UITabPage CurPage => curSelect >= 0 && curSelect < cellList.Count ? cellList[curSelect].page : null;

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
                onSelect.Invoke();
            }
            else if(canDeselect)
            {
                SelectTab(-1);
            }
        }

        public void OnCellClicked(UITabCell cell)
        {
            SoundManager.Instance.PlayNormalSound("Audio_ClickNormal");
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

        public UITabCell GetCell(int index)
        {
            return cellList[index];
        }

        public T GetPage<T>() where T : UITabPage
        {
            foreach (var cell in cellList)
            {
                if (cell.page is T page)
                {
                    return page;
                }
            }
            return null;
        }
    }
}
