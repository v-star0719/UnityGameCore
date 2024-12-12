using GameCore.Scripts;
using UnityEngine;

#if NGUI
namespace GameCore.Unity.NGUIEx
{
    public class UITabCell : MonoBehaviour
    {
        public GameObject normalGo;   //普通状态的图片（即未选中状态）
        public GameObject selectGo; //选中状态的图片
        public UITabPage page;//对应的页面

        public int Index { get; private set; } //这个标签在标签栏里的索引
        public UITabCtrl TabCtrl { get; private set; }

        public void Init(int index, UITabCtrl tabCtrl)
        {
            Index = index;
            TabCtrl = tabCtrl;
            if (page != null)
            {
                page.Init();
            }
        }

        public void OnClick()
        {
            TabCtrl.OnCellClicked(this);
        }

        //设置选择/不选择状态
        public void Select(bool sel)
        {
            if (normalGo != null)
            {
                normalGo.SetActive(!sel);
            }

            if (selectGo != null)
            {
                selectGo.SetActive(sel);
            }

            if(page != null)
            {
                if (sel)
                {
                    page.Show();
                }
                else
                {
                    page.Hide();
                }
            }
        }

        public void Clear()
        {
            if (page != null)
            {
                page.Clear();
            }
        }
    }
}

#endif