using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    [ExecuteAlways]
    public class UIScrollPickerVertical : UIScrollPickerBase
    {
        public override float GetDistToViewportCenter(Transform item)
        {
            return Mathf.Abs(ContentPosToViewportPos(item.localPosition).y);
        }

        public override float GetViewportSize()
        {
            return viewportTrans.rect.size.y;
        }

        public override void OnDrawGizmos()
        {
            if (viewportTrans != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(viewportTrans.rect.size.x, pickAreaSize, 0.1f));
            }
        }

        protected override void DoMove(Vector3 delta)
        {
            contentTrans.localPosition += new Vector3(0, delta.y, 0);
        }

        protected override Vector3 GetItemPos(int index)
        {
            return new Vector3(0, (itemSize + itemGap) * index, 0);
        }

        protected override bool IsInSelectArea(UIScrollPickerItem item)
        {
            return Mathf.Abs(item.transform.localPosition.y + contentTrans.localPosition.y) < pickAreaSize * 0.5f;
        }

        protected override bool IsOutOfViewport()
        {
            var bottomEdgeViewPos = contentTrans.localPosition.y;
            if(bottomEdgeViewPos > 0)
            {
                return true;
            }

            var topEdgeViewPos = items[^1].transform.localPosition.y + contentTrans.localPosition.y;
            if(topEdgeViewPos < 0)
            {
                return true;
            }
            return false;
        }
    }
}

