using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    public class UIGridExItem : MonoBehaviourRectTrans
    {
        public GameObject selectedMark;

        [ReadOnly] public UIGridEx grid;
        [ReadOnly] public int index;

        public IGridExData baseData;

        public virtual void SetData(IGridExData data)
        {
            this.baseData = data;
        }

        public virtual void Refresh()
        {

        }

        public virtual bool CanSelect(bool isClickMsg)
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
        }

        public virtual void OnClick()
        {
            grid.OnItemSelect(this, true);
        }
    }

    public class UIGridExItemT<T> : UIGridExItem where T : class, IGridExData
    {
        public T data;
        public override void SetData(IGridExData d)
        {
            base.SetData(d);
            this.data = d as T;
        }
    }
}
