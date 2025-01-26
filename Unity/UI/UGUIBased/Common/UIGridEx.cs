using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UISpriteCollection;

namespace GameCore.Unity.UGUIEx
{
    public struct ScrollViewDragDirs
    {
        public bool l;
        public bool r;
        public bool u;
        public bool d;
    }

    public interface IGridExData
    {
        int GridId { get; }
    }

    //对简单列表的功能封装
    //（1）自动创建列表，对列表项进行复用。列表项目前不会自动删除，多了的会隐藏
    //（2）支持定位到某一项
    //（3）支持列表选中，刷新后保持当前的选中位置。需要列表数据具有 GridId 属性，用来锁定某一项。
    public class UIGridEx : MonoBehaviour
    {
        public UIGridExItem itemPrefab;
        //public UIScrollView scrollView;
        public bool selectByDefault = true;//初始化以及当前选中的数据丢失时，默认选中一个。如果未false，只能点击进行选中。

        public Action<ScrollViewDragDirs> onDraggableChanged; //左右上下四个方向是不是可以拖动发生变化
        public Action arrangeFunc; //自定义排列，不用UIGrid或者UITable
        public UIGridExItem SelectedItem { get; private set; }
        public Action<UIGridExItem> onItemSelect;
        public bool autoScrollToHoverObj;

        private List<UIGridExItem> items;
        private List<UIGridExItem> recycledItems; //后创建的放在前面，先创建的放在后面，保证取出来的顺序和在gameObject下的排练顺序一样。每次取最后的
        private List<IGridExData> datas;
        private float itemScale = 1;
        private float itemTotalCount;
        private float dragCheckTimer;
        //private ScrollViewDragDirs dragDirs; //左右上下四个方向是不是可以拖动

        //private float dragError = 5f;
        private float dragCheckInterval = 0.2f;
        private SpringPanel __springPanel;
        private float autoScrollToHoverObjTimer = 0;

        private SpringPanel SpringPanel
        {
            get
            {
                if (__springPanel == null)
                {
                    __springPanel = GetComponent<SpringPanel>();
                }
                if(__springPanel == null)
                {
                    __springPanel = gameObject.AddComponent<SpringPanel>();
                }
                return __springPanel;
            }
        }

        public int DataCount => datas.Count;

        private void Start()
        {
            enabled = onDraggableChanged != null || autoScrollToHoverObj;
        }

        protected virtual void Init()
        {
            if (itemPrefab == null)
            {
                Debug.LogError("itemPrefab has not been set");
                return;
            }
            items = new List<UIGridExItem>();
            recycledItems = new List<UIGridExItem>();
            itemTotalCount = 0;
            itemScale = itemPrefab.transform.localScale.x;
            itemPrefab.gameObject.SetActive(false);

            enabled = onDraggableChanged != null || autoScrollToHoverObj;
            //if (onDraggableChanged != null)
            //{
            //    dragDirs = new ScrollViewDragDirs();
            //}
        }

        private void Set(List<IGridExData> datas)
        {
            this.datas = datas;
            var dataCount = datas.Count;

            RecycleAllItems();

            for (int i = 0; i < dataCount; i++)
            {
                var item = GetAvailableItem();
                item.gameObject.SetActive(true);
                item.index = i;
                item.SetData(datas[i]);
                item.SetSelect(false);
                items.Add(item);
            }

            arrangeFunc?.Invoke();
            //Debug.Log($"used: {items.Count}  recycled: {recycledItems.Count} total: {itemTotalCount}");
        }

        //在短时间内依次显示每个item的动画
        // 每个item的展示间隔单位s，默认是0.03s
        public void PlayAnima(float interval = 0.03f, float delay = 0f)
        {
            //NGUIUtils.PlayListAnima(items, interval, delay);
        }

        public void StopAnima()
        {
            //NGUIUtils.StopListAnima(items);
        }

        public void RecycleAllItems()
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                recycledItems.Add(item);
                item.gameObject.SetActive(false);
                items.RemoveAt(i);
            }
            items.Clear();
            //Debug.Log($"recycled: {recycledItems.Count}");
        }

        public UIGridExItem GetAvailableItem()
        {
            UIGridExItem item = null;
            var index = recycledItems.Count - 1;
            if (index < 0)
            {
                var go = GameObject.Instantiate(itemPrefab, itemPrefab.transform.parent);
                itemTotalCount += 1;
                go.name = itemTotalCount.ToString();
                go.transform.localScale = new Vector3(itemScale, itemScale, itemScale);
                item = go.GetComponent<UIGridExItem>();
                item.grid = this;
                var button = item.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(item.OnClick);
                }
                //Debug.Log($"GetAvailableItem. no recycled items, create new. total {itemTotalCount}");
            }
            else
            {
                item = recycledItems[index];
                recycledItems.RemoveAt(index);
                //Debug.Log($"GetAvailableItem. get recycled items, remain  {recycledItems.Count}");
            }
            return item;
        }

        //如果当前没有选中，选中第一个。如果当前有选中，则根据数据id查找index，如果找到了选中这个index位置;没有找到选中原index位置
        private void SetInner(List<IGridExData> datas, bool keepSelectCurrent, bool focusToCurrent, bool noAnim)
        {
            if (items == null)
            {
                Init();
            }

            var index = -1;
            if (SelectedItem != null && keepSelectCurrent)
            {
                var selectDataId = SelectedItem.baseData.GridId;
                var newDataIndex = GetDataIndexById(selectDataId);
                if (newDataIndex >= 0)
                {
                    index = newDataIndex;
                }
                else if(selectByDefault)
                {
                    index = SelectedItem.index;
                }
            }
            else if (selectByDefault)
            {
                index = 0;
            }

            Set(datas);
            SelectItem(index, false, false, false);

            if (keepSelectCurrent)
            {
                RestrictInScrollView();
                if (focusToCurrent)
                {
                    if (IsItemVisible(items[index]))
                    {
                        RestrictInScrollView();
                    }
                    else
                    {
                        FocusTo(index, false);
                    }
                }
            }
            else
            {
                ResetScrollView();
                if (!noAnim)
                {
                    PlayAnima();
                }
            }
        }

        //todo default select
        public void Set<T>(List<T> datas, bool keepSelectCurrent = false, bool focusToCurrent = false, bool noAnim = true) where T : IGridExData
        {
            List<IGridExData> list = new List<IGridExData>();
            foreach (var d in datas)
            {
                list.Add(d);
            }
            SetInner(list, keepSelectCurrent, focusToCurrent, noAnim);
        }

        public int GetItemIndexById(int id)
        {
            return GetDataIndexById(id);
        }

        public List<T> GetItems<T>() where T : UIGridExItem
        {
            List<T> rt = new List<T>();
            foreach (var item in items)
            {
                rt.Add(item as T);
            }
            return rt;
        }

        //@param flashTo：true = 闪现过去
        public void SelectItem(int index, bool focusTo, bool triggerClick, bool flashTo)
        {
            if (index >= datas.Count)
            {
                index = datas.Count - 1;
            }

            if (index >= 0)
            {
                OnItemSelect(items[index], triggerClick);
                if (focusTo)
                {
                    FocusTo(index, flashTo);
                }
            }
            else
            {
                OnItemSelect(null, triggerClick);
            }
        }

        //由于item的位置随时可能改变，所以实时根据id获取他的位置
        public void SelectItemByItem(UIGridExItem item, bool focusTo, bool flashTo)
        {
            var index = GetItemIndexById(item.baseData.GridId);
            SelectItem(index, focusTo, false, flashTo);
        }

        //由于item的位置随时可能改变，所以实时根据id获取他的位置
        public void SelectItemById(int id, bool focusTo, bool flashTo)
        {
            var index = GetItemIndexById(id);
            SelectItem(index, focusTo, false, flashTo);
        }

        public void ClearCurrentSelect()
        {
            if (SelectedItem != null)
            {
                SelectedItem.SetSelect(false);
            }
            SelectedItem = null;
        }

        public void OnItemSelect(UIGridExItem item, bool isClickMsg)
        {
            if (item != null && !item.CanSelect())
            {
                return;
            }

            if (SelectedItem != null )
            {
                SelectedItem.SetSelect(false);
            }

            SelectedItem = item;
            if (item != null)
            {
                item.SetSelect(true);
                item.OnSelectX(isClickMsg);
            }
            onItemSelect?.Invoke(item);
        }

        public void Refresh()
        {
            foreach (var t in items)
            {
                t.Refresh();
            }
        }

        public void ResetScrollView()
        {
            //if (scrollView != null)
            //{
            //    scrollView.ResetPosition();
            //}
        }

        public void RestrictInScrollView()
        {
            //if (scrollView != null)
            //{
            //    scrollView.InvalidateBounds();
            //    scrollView.RestrictWithinBounds(false);
            //}
        }

        //滚动到一个项哪里
        //这个index是从0开始的
        public void FocusTo(int index, bool flashTo)
        {
            ScrollToItem(index, flashTo);
        }

        //@return bool true = 要滚动， false = 不要滚动
        public bool ScrollToItem(int index, bool flashTo)
        {
            var item = items[index];
            if (item == null)
            {
                return false;
            }

            //if (scrollView == null)
            //{
            //    return false;
            //}

            float t, b, l, r; 
            GetDistToClipBorder(item, out t, out b, out l, out r);
            if (CheckInClipArea(t, b, l, r))
            {
                return false;
            }

            var tempPos = new Vector3(0, 0, 0);
            tempPos.x = 0;
            tempPos.y = 0;
            tempPos.z = 0;
            if (l > 0)
            {
                tempPos.x = tempPos.x + l;
            }
            else if (r < 0)
            {
                tempPos.x = tempPos.x + r;
            }
            else if (t < 0)
            {
                tempPos.y = tempPos.y + t;
            }
            else if (b > 0)
            {
                tempPos.y = tempPos.y + b;
            }

            //if (flashTo)
            //{
            //    SpringPanel.Stop(scrollView.gameObject);
            //    scrollView.MoveRelative(tempPos);
            //}
            //else
            //{
            //    SpringPanel.Begin(scrollView.gameObject, tempPos + scrollView.transform.localPosition, 10);
            //}

            return true;
        }

        public void ScrollToPos(float x, float y)
        {
            //var go = scrollView.gameObject;
            //SpringPanel.Stop(go);
            //var offset = scrollView.transform.localPosition;
            //offset.x = offset.x + x;
            //offset.y = offset.y + y;
            //SpringPanel.Begin(go, offset, 10);
        }

        public void ScrollToPosAbsolutely(float x, float y)
        {
            //SpringPanel.Begin(scrollView.gameObject, new Vector3(x, y, 0), 10);
        }

        public void CenterToItem(int index, bool flashTo)
        {
            var item = GetItem(index);
            if (item == null)
            {
                return;
            }

            //var panel = scrollView.panel;
            //var clip = panel.finalClipRegion;
            //var pos = scrollView.transform.InverseTransformPoint(item.transform.position);
            //pos.x = clip.x - pos.x;
            //pos.y = clip.y - pos.y;
            //if (flashTo)
            //{
            //    SpringPanel.Stop(scrollView.gameObject);
            //    scrollView.MoveRelative(pos);
            //}
            //else
            //{
            //    SpringPanel.Begin(scrollView.gameObject, pos + scrollView.transform.localPosition, 10);
            //}
        }

        //@param item GridItem
        public bool IsItemVisible(UIGridExItem item)
        {
            //if (scrollView == null || item == null)
            //{
            //    return true;
            //}

            float t, b, l, r;
            GetDistToClipBorder(item, out t, out b, out l, out r);
            return CheckInClipArea(t, b, l, r);
            //return panel.IsVisible(widget); //只有部分看到也是可见的，不好使
        }

        //item四个tblr边到分别到裁剪区域的tblr四个边的距离向量。如果t >= 0, b <= 0, l <= 0, r >= 0，则说明在裁剪区域内
        public void GetDistToClipBorder(UIGridExItem item, out float t, out float b, out float l, out float r)
        {
            t = b = l = r = 0;
            //if (scrollView == null || item == null)
            //{
            //    return;
            //}

            //var panel = scrollView.panel;
            //var clip = panel.finalClipRegion;
            ////@type UIWidget
            //var widget = item.gameObject.GetComponentInChildren<UIWidget>();
            //var pos = scrollView.transform.InverseTransformPoint(item.transform.position);
            //if (IsHorizontal())
            //{
            //    //水平方向
            //    r = clip.x + clip.z * 0.5f - (pos.x + widget.width * 0.5f);
            //    l = clip.x - clip.z * 0.5f - (pos.x - widget.width * 0.5f);
            //}
            //else
            //{
            //    //竖直方向
            //    t = clip.y + clip.w * 0.5f - (pos.y + widget.height * 0.5f);
            //    b = clip.y - clip.w * 0.5f - (pos.y - widget.height * 0.5f);
            //}
            ////Debug.Log($"t = {t}, b = {b}, l = {l}, r = {r}")
        }

        public virtual bool IsHorizontal()
        {
            //return scrollView == null || scrollView.movement == UIScrollView.Movement.Horizontal;
            return true;
        }

        public UIGridExItem GetItem(int index)
        {
            return items[index];
        }

        public T GetItem<T>(int index) where T : UIGridExItem
        {
            return GetItem(index) as T;
        }

        public UIGridExItem GetItemById(int id)
        {
            return GetItem(GetItemIndexById(id));
        }

        public T GetItemById<T>(int id) where T : UIGridExItem
        {
            return GetItemById(id) as T;
        }

        public UIGridExItem GetItemByGo(GameObject go)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].gameObject == go)
                {
                    return items[i];
                }
            }
            return null;
        }
        //@检查 left, right, up, down 四个方向是否可以滑动
        public void CheckDraggable()
        {
            //var bounds = scrollView.bounds;
            //var boundsMax = bounds.max;
            //var boundsMin = bounds.min;
            //var finalClipRegion = scrollView.panel.finalClipRegion;

            //var canLeft = boundsMax.x > finalClipRegion.x + finalClipRegion.z * 0.5 + dragError;
            //var canRight = boundsMin.x < finalClipRegion.x - finalClipRegion.z * 0.5 - dragError;
            //var canUp = boundsMax.y > finalClipRegion.y + finalClipRegion.w * 0.5 + dragError;
            //var canDown = boundsMin.y < finalClipRegion.y - finalClipRegion.w * 0.5 - dragError;
            //////Debug.Log($"l={canLeft}, r={canRight}, u={canUp}, d={canDown}")

            //var changed = false;
            //if (canLeft != dragDirs.l)
            //{
            //    dragDirs.l = canLeft;
            //    changed = true;
            //}
            //if (canRight != dragDirs.r)
            //{
            //    dragDirs.r = canRight;
            //    changed = true;
            //}
            //if (canUp != dragDirs.u)
            //{
            //    dragDirs.u = canUp;
            //    changed = true;
            //}
            //if (canDown != dragDirs.d)
            //{
            //    dragDirs.d = canDown;
            //    changed = true;
            //}

            //if (changed)
            //{
            //    ////Debug.Log($"l={canLeft}, r={canRight}, u={canUp}, d={canDown}")
            //    onDraggableChanged(dragDirs);
            //}
        }

        public void Update()
        {
            if (onDraggableChanged != null)
            {
                dragCheckTimer = dragCheckTimer + Time.deltaTime;
                if(dragCheckTimer > dragCheckInterval)
                {
                    dragCheckTimer = 0;
                    CheckDraggable();
                }
            }

            AutoScrollToHoverObj();
        }

        public void OnDestroy()
        {
            onDraggableChanged = null;
            if (items != null)
            {
                items.Clear();
                recycledItems.Clear();
            }
            SelectedItem = null;
            //scrollView = null;
        }

        public int GetDataIndexById(int id)
        {
            for (var i = 0; i < datas.Count; i++)
            {
                if (datas[i].GridId == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public T GetSelectItem<T>() where T : UIGridExItem
        {
            return SelectedItem as T;
        }

        //item四个边到四个裁剪边的距离
        public bool CheckInClipArea(float t, float b, float l, float r)
        {
            return t >= 0 && b <= 0 && l <= 0 && r >= 0;
        }

        public void AutoScrollToHoverObj()
        {
            //if(scrollView == null)
            //{
            //    return;
            //}
            autoScrollToHoverObjTimer += Time.deltaTime;
            if (autoScrollToHoverObjTimer > 0.3f)
            {
                autoScrollToHoverObjTimer = 0;

                var spri = SpringPanel;
                if(spri.enabled)
                {
                    return;
                }

                var obj = EventSystem.current.currentSelectedGameObject;
                if (obj != null)
                {
                    foreach (var item in items)
                    {
                        if (item.gameObject == obj)
                        {
                            if (!IsItemVisible(item))
                            {
                                ScrollToItem(item.index, false);
                            }
                            return;
                        }
                    }
                }
            }
        }

        public void SetNavigation()
        {
            for (int i = 0; i < items.Count; i++)
            {
                //var kn = item.GetComponent<UIKeyNavigation>();
                //if (kn != null)
                //{
                //    if (IsHorizontal())
                //    {
                //        if (preKn != null)
                //        {
                //            preKn.onRight = kn.gameObject;
                //            kn.onLeft = preKn.gameObject;
                //        }
                //    }
                //    else
                //    {
                //        if (preKn != null)
                //        {
                //            preKn.onDown = kn.gameObject;
                //            kn.onUp = preKn.gameObject;
                //        }
                //    }

                //    if (kn.startsSelected)
                //    {
                //        kn.startsSelected = i == 0;
                //    }
                //    preKn = kn;
                //}
            }
        }
    }
}
