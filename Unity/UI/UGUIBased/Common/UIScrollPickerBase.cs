using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

//
//(1)目前没有做循环滚动，循环很麻烦
//(2)水平形式：从左到右依次对应数组的01234...索引
//   竖直形式：从上到下依次对应数组的01234...索引
//为方便处理，第一个item的坐标是(0,0,0)，和contentTrans重合。
//滚动的时候，移动的是contentTrans
//坐标系y有两套，viewport坐标系（原点在视口中心），content坐标系（原点在content节点）
//视口不会动，是content在动。比如要把content上(1,0)点移动到中间，只需要content移动(-1,0)就行了
namespace GameCore.Unity.UGUIEx
{
    public abstract class UIScrollPickerBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform viewportTrans;
        public RectTransform contentTrans;
        public float pickAreaSize;//滚动到一个区域内认为是选中了，这个参数指定这个区域的大小

        public int itemSize;
        public int itemGap;
        public bool hideInactive;

        protected List<UIScrollPickerItem> items = new();
        public int CurSelectedIndex { get; private set; }

        public float springDuration = 0.3f;
        private bool isDragging = false;

        private Vector3 springStartPos;
        private Vector3 springEndPos;
        private float springTimer = 0f;
        private bool isSpring = false;

        public float dampintDuration = 0.3f;
        private Vector2 lastDragDelta;
        private float dampingTimer;
        private bool isDamping;

        private bool needRebuild = false;

        public UnityEvent<int> onSelect;//索引，是否是点击的项

        // Use this for initialization
        private void Start()
        {

        }

        private void Update()
        {
            if (contentTrans == null || viewportTrans == null)
            {
                return;
            }

            if (needRebuild || !Application.isPlaying)
            {
                Rebuild();
            }
            Update_Damping();
            Update_Spring();
        }

        private void Update_Spring()
        {
            if (isDragging)
            {
                return;
            }

            if (!isSpring)
            {
                return;
            }

            springTimer += Time.deltaTime;
            float f = Mathf.Clamp01(springTimer / springDuration);
            contentTrans.localPosition = Vector3.Lerp(springStartPos, springEndPos, f);
            AfterMove();

            if (springTimer >= springDuration)
            {
                isSpring = false;
            }
        }

        private void Update_Damping()
        {
            if (!isDamping)
            {
                return;
            }
            dampingTimer += Time.deltaTime;//拖动后按住不动也进行衰减处理，避免按住不动后松开还滑动很远

            if(isDragging)
            {
                return;
            }

            var f = 1 - dampingTimer / dampintDuration;
            if (f <= 0)
            {
                isDamping = false;
                f = 0;
            }
            Move(lastDragDelta * (f * f * f));
            if(!isDamping)
            {
                SpringToNearestItem();
            }
        }

        public void AddItem(UIScrollPickerItem item)
        {
            needRebuild = true;
            items.Add(item);
        }

        public void RemoveItem(UIScrollPickerItem item)
        {
            needRebuild = true;
            items.Remove(item);
        }

        public void Rebuild()
        {
            needRebuild = false;
            items.Clear();
            var index = 0;
            for (int i = 0; i < contentTrans.childCount; i++)
            {
                var trans = contentTrans.GetChild(i);
                if (hideInactive && !trans.gameObject.activeSelf)
                {
                    continue;
                }

                var item = trans.GetComponent<UIScrollPickerItem>();
                if (item == null)
                {
                    item = trans.gameObject.AddComponent<UIScrollPickerItem>();
                }
                item.index = index;
                items.Add(item);
                trans.localPosition = GetItemPos(index);
                item.ShowTransit();
                index++;
            }
        }

        public void Select(int select, bool force = false, bool cut = false)
        {
            if (select == CurSelectedIndex)
            {
                ChangeSelect(select);
                return;
            }
            SpringToContentPos(items[select].transform.localPosition, cut);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (contentTrans == null || viewportTrans == null)
            {
                return;
            }
            isDragging = true;
            isDamping = true;
            isSpring = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(contentTrans == null || viewportTrans == null)
            {
                return;
            }
            lastDragDelta = eventData.delta;
            dampingTimer = 0;
            Move(eventData.delta);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(contentTrans == null || viewportTrans == null)
            {
                return;
            }
            isDragging = false;
            if (IsOutOfViewport())
            {
                isDamping = false;
                SpringToNearestItem();
            }
        }

        public virtual void OnDrawGizmos()
        {
        }

        public virtual Vector3 ContentPosToViewportPos(Vector3 p)
        {
            return p + contentTrans.localPosition;
        }

        public virtual float GetDistToViewportCenter(Transform item)
        {
            return 0;
        }

        public virtual float GetViewportSize()
        {
            return 0;
        }

        ///已知content上一个点的viewport坐标，滚动到这个点上
        public void SpringToSomewhereByViewportPos(Vector3 viewportPos, bool cut = false)
        {
            if (cut)
            {
                contentTrans.localPosition -= viewportPos;
                AfterMove();
            }
            else
            {
                //原点在视口中心，视口中心是不动的，移动的是content的。
                //viewportPos刚好就是视口中心到这个点的方向。把content反向移动那么多，就会到视口中心了。
                springStartPos = contentTrans.localPosition;
                springEndPos = contentTrans.localPosition - viewportPos;
                springTimer = 0f;
                isSpring = true;
                dampingTimer = dampintDuration;
            }
        }

        ///已知content上一个点的content坐标，滚动到这个点上
        public void SpringToContentPos(Vector3 contentPos, bool cut = false)
        {
            SpringToSomewhereByViewportPos(ContentPosToViewportPos(contentPos), cut);
        }

        protected virtual void AfterMove()
        {
            for(int i = 0; i < items.Count; i++)
            {
                UIScrollPickerItem item = items[i];
                item.ShowTransit();
                if(IsInSelectArea(item))
                {
                    if(CurSelectedIndex != item.index)
                    {
                        item.OnSelect();
                        ChangeSelect(item.index);
                    }
                }
            }
        }

        protected virtual void ChangeSelect(int index)
        {
            CurSelectedIndex = index;
            if(onSelect != null)
            {
                onSelect.Invoke(CurSelectedIndex);
            }
        }

        protected virtual void DoMove(Vector3 delta)
        {
        }

        protected virtual Vector3 GetItemPos(int index)
        {
            return Vector3.zero;
        }

        protected virtual bool IsInSelectArea(UIScrollPickerItem item)
        {
            return false;
        }

        protected virtual bool IsOutOfViewport()
        {
            return false;
        }

        protected virtual void Move(Vector3 delta)
        {
            DoMove(delta);
            AfterMove();
        }

        protected virtual void SpringToNearestItem()
        {
            float minDistToViewportCenter = float.MaxValue;
            Transform item = null;
            for(int i = 0; i < contentTrans.childCount; i++)
            {
                var trans = contentTrans.GetChild(i);
                var dist = GetDistToViewportCenter(trans);
                if(dist < minDistToViewportCenter)
                {
                    item = trans;
                    minDistToViewportCenter = dist;
                }
            }
            SpringToContentPos(item.transform.localPosition);
        }
    }
}
