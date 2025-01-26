using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    [ExecuteInEditMode]
    public class UIScrollPickerHorizontal : UIScrollPickerBase
    {
        public override float GetDistToViewportCenter(Transform item)
        {
            return Mathf.Abs(ContentPosToViewportPos(item.localPosition).x);
        }

        public override float GetViewportSize()
        {
            return viewportTrans.rect.size.x;
        }

        public override void OnDrawGizmos()
        {
            if (viewportTrans != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(pickAreaSize, viewportTrans.rect.size.y, 0.1f));
            }
        }

        protected override void DoMove(Vector3 delta)
        {
            contentTrans.localPosition += new Vector3(delta.x, 0, 0);
        }

        protected override Vector3 GetItemPos(int index)
        {
            return new Vector3((itemSize + itemGap) * index, 0, 0);
        }

        protected override bool IsInSelectArea(UIScrollPickerItem item)
        {
            return Mathf.Abs(item.transform.localPosition.x + contentTrans.localPosition.x) < pickAreaSize * 0.5f;
        }
        
        protected override bool IsOutOfViewport()
        {
            var leftEdgeViewPos = contentTrans.localPosition.x;
            if(leftEdgeViewPos > 0)
            {
                return true;
            }
            var rightEdgeViewPos = items[^1].transform.localPosition.x + contentTrans.localPosition.x;
            if(rightEdgeViewPos < 0)
            {
                return true;
            }
            return false;
        }
    }
}

