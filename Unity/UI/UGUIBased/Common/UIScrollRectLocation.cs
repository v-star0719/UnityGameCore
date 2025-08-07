using System.Collections;
using Fight;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class UIScrollRectLocation : MonoBehaviour
    {
        public Vector2 stayViewportPos = new Vector2(0.5f, 0.5f);//停留在视口哪个地方。0~1。
        public float duration = 0.2f;
        private ScrollRect _scrollRect;
        private RectTransform target;
        private float timer = 0f;
        private Vector2 targetPos;
        private Vector2 currentPos;
        private int waitInitFrame;
        private bool hasInitPos;

        public ScrollRect ScrollRect
        {
            get
            {
                if (_scrollRect == null)
                {
                    _scrollRect = GetComponent<ScrollRect>();
                }
                return _scrollRect;
            }
        }

        public void ScrollTo(RectTransform target)
        {
            this.target = target;
            timer = 0;
            waitInitFrame = 1;
            hasInitPos = false;
            enabled = true;
        }

        private void Start()
        {

        }

        private void Update()
        {
            if (target == null)
            {
                Stop();
                return;
            }

            if (waitInitFrame > 0)
            {
                waitInitFrame--;
                return;
            }

            if (!hasInitPos)
            {
                hasInitPos = true;
                InitScrollPos();
            }
            timer += Time.deltaTime;
            var f = Mathf.Min(timer / duration, 1);
            ScrollRect.normalizedPosition = Vector2.Lerp(currentPos, targetPos, f);
            if (f >= 1)
            {
                Stop();
            }
        }

        private void InitScrollPos()
        {
            var contentRect = ScrollRect.content.rect;
            var pos = ScrollRect.content.InverseTransformPoint(target.position);//这个pos是content节点下的局部坐标
            var viewRect = (ScrollRect.viewport ?? ScrollRect.GetComponent<RectTransform>()).rect;

            //初始状态下，content的顶边和viewport的顶边重合，content的左边和viewport的左边重合
            //滚动就是移动content的位置，这样的坐标系下移动定位就很好处理了

            var maxScrollDist = new Vector2(contentRect.size.x - viewRect.size.x, contentRect.size.y - viewRect.size.y);
            if(ScrollRect.horizontal && maxScrollDist.x <= 0 || ScrollRect.vertical && maxScrollDist.y <= 0)
            {
                currentPos = targetPos;
                return;//不需要滚动，视口足够容纳
            }

            var contentTrans = ScrollRect.content;
            var viewPos = new Vector2(stayViewportPos.x * viewRect.size.x, stayViewportPos.y * viewRect.size.y);
            //就是把坐标原点从content节点，转为content左下角。content的位置是pivot指定的。
            var itemPos = new Vector2(pos.x, pos.y) + new Vector2(contentRect.size.x * contentTrans.pivot.x, contentRect.size.y * contentTrans.pivot.y);
            var dir = itemPos - viewPos;//滚动的时候normalizedPosition从0增长到1，要用正数

            currentPos = ScrollRect.normalizedPosition;
            if(ScrollRect.horizontal)
            {
                targetPos = new Vector2(Mathf.Clamp01(dir.x / maxScrollDist.x), currentPos.y);
            }
            if(ScrollRect.vertical)
            {
                targetPos = new Vector2(currentPos.x, Mathf.Clamp01(dir.y / maxScrollDist.y));
            }
            //Debug.Log($"contentRect={contentRect}, viewRect={viewRect}, pos={targetPos}");
        }

        private void Stop()
        {
            enabled = false;
        }
    }
}