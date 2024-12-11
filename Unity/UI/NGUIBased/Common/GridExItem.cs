using UnityEngine;

#if NGUI
namespace UI
{
    public class GridExItem : MonoBehaviour
    {
        public GameObject selectedMark;
        public UITweener[] selectedMarkTweeners;

        [HideInInspector]
        public GridEx grid;
        public IGridExData baseData;
        [HideInInspector]
        public int index;
        
        public virtual void SetData(IGridExData data)
        {
            this.baseData = data;
        }

        public virtual void Refresh()
        {

        }

        public virtual bool CanSelect()
        {
            return true;
        }

        //NGUI会发出OnSelect事件，不能用这个名字。
        public virtual void OnSelectX(bool isClick)
        {
        }

        //列表项实现这个方法来显示这个item被选中的效果。true = 选中，false = 没被选中
        public virtual void SetSelect(bool sel)
        {
            if (selectedMark == null)
            {
                return;
            }

            selectedMark.SetActive(sel);
            if (selectedMarkTweeners != null)
            {
                NGUIUtils.PlayTweens(selectedMarkTweeners, true);
            }
        }

        public virtual void OnClick()
        {
            grid.OnItemSelect(this, true);
        }
    }

    public class GridExItemT<T> : GridExItem where T : class, IGridExData
    {
        public T data;
        public override void SetData(IGridExData d)
        {
            base.SetData(d);
            this.data = d as T;
        }
    }
}

#endif