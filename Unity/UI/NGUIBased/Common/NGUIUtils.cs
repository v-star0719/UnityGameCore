#if NGUI
namespace GameCore.Unity.NGUIEx
{
    public class NGUIUtils
    {
        //设计参考尺寸
        public static float uiScreenWidth = 1920;
        public static float uiScreenHeight = 1080;

        //实际NGUI用的屏幕尺寸
        public static float uiViewHeight = -1;
        public static float uiViewWidth = -1;

        public static void PlayTweens(UITweener[] tweeners, bool forward)
        {
            foreach (var tweener in tweeners)
            {
                PlayTween(tweener, forward);
            }
        }

        public static void PlayTween(UITweener tweener, bool forward)
        {
            if (forward)
            {
                tweener.Sample(0, true);
                tweener.tweenFactor = 0;
                tweener.PlayForward();
            }
            else
            {
                tweener.Sample(1, true);
                tweener.tweenFactor = 1;
                tweener.PlayReverse();
            }
        }
        public static void SampleTweens(UITweener[] tweeners, float factor)
        {
            foreach (var tweener in tweeners)
            {
                tweener.Sample(factor, false);
            }
        }

        //NGUI的ResetToBeginning，如果是上次正向的，就认为0是起点；如果上一次就反向的，就认为1是起点。真坑
        public static void ResetTweenToBegin(UITweener tweens)
        {
            tweens.Sample(0, false);
            tweens.tweenFactor = 0;
        }

        //NGUI的ResetToBeginning，如果是上次正向的，就认为0是起点；如果上一次就反向的，就认为1是起点。真坑
        public static void ResetTweenToEnd(UITweener tweener)
        {
            tweener.Sample(1, false);
            tweener.tweenFactor = 1;
        }

        public static void ResetTweensToBegin(UITweener[] tweeners)
        {
            foreach (var tweener in tweeners)
            {
                ResetTweenToBegin(tweener);
            }
        }

        public static void ResetTweensToEnd(UITweener[] tweeners)
        {
            foreach (var tweener in tweeners)
            {
                ResetTweenToEnd(tweener);
            }
        }

        public static void PlayListAnima_Trans(Transform root, float interval, float delay)
        {
            var n = root.childCount;
            var items = new List<Transform>(n);
            for (int i = 0; i < n; i++)
            {
                var trans = root.GetChild(i);
                if (trans.gameObject.activeSelf)
                {
                    items.Add(trans);
                }
            }
            PlayListAnima(items, interval, delay);
        }

        //在短时间内依次显示每个item的动画，采用TweenAlpha，要求控件带有ngui Widget
        //每个item的展示间隔单位s，默认是0.03s
        public static void PlayListAnima<T>(List<T> itemList, float interval = 0.05f, float delay = 0f) where T : Component
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                var widget = itemList[i].gameObject.GetComponent<UIWidget>();
                if (widget != null)
                {
                    //-@type TweenAlpha
                    var tweener = widget.gameObject.GetComponent<TweenAlpha>();
                    if (tweener == null)
                    {
                        tweener = widget.gameObject.AddComponent<TweenAlpha>();
                        tweener.from = 0;
                        tweener.to = 1;
                        tweener.duration = 0.3f;
                    }
                    tweener.delay = delay + i * interval;
                    tweener.ResetToBeginning();
                    tweener.PlayForward();
                }
                else
                {
                    Debug.LogError("ngui widget is required for Play Anima");
                    return;
                }
            }
        }

        public static void StopListAnima<T>(List<T> itemList) where T : Component
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                var widget = itemList[i].gameObject.GetComponent<UIWidget>();
                if (widget != null)
                {
                    var tweener = widget.gameObject.GetComponent<TweenAlpha>();
                    if (tweener != null)
                    {
                        tweener.Sample(1, true);
                        tweener.enabled = false;
                    }
                }
            }
        }

        //     □
        //     ↑
        // □ ← O → □
        //     ↓
        //     □
        //四个方向探寻，把UI放在指定位置的上面、下面、左边、还是右边。只要能放进屏幕即可，另一个方向通过平移把面板挪到屏幕内。
        //比如如果上面的空间够，把UI放到上面，然后左右平移就可以把面板放进屏幕内。
        //gameObject: UI的根节点，底下要有一个节点用来调整UI的位置，使UI的根节点和UI中心重合。
        //placePos是世界坐标。通过panelGo转换为面板内的局部坐标。
        //要求uiGo初始在屏幕中心
        public static void AutoPlaceUI(GameObject panelGo, GameObject uiGo, Vector3 placePos)
        {
            const float GAP = 100; //UI边缘和当前位置的间距，即图中箭头的长度。

            var bounds = NGUIMath.CalculateRelativeWidgetBounds(uiGo.transform);
            placePos = panelGo.transform.InverseTransformPoint(placePos);

            //让将ui的中心和placePos重合
            placePos.z = 0;
            placePos = placePos - bounds.center;

            //Debug.LogError("ConstrainUIInScreen", tostring(bounds.max), tostring(bounds.min), tostring(bounds.size), tostring(bounds.center));

            var viewSize = GetUIViewSize();
            var halfViewSizeH = viewSize.x * 0.5f;
            var halfViewSizeW = viewSize.y * 0.5f;
            var sizeX = bounds.size.x;
            var sizeY = bounds.size.y;
            var offsetX = 0f;
            var offsetY = 0f;

            //向上试探，放在上面时，计算UI上边和屏幕上边的距离
            var maxDist = -9999f;
            var dist = halfViewSizeH - (placePos.y + sizeY + GAP);
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = 0;
                offsetY = sizeY * 0.5f + GAP;
            }

            //向下试探，放在下面时，计算UI下边和屏幕下边的距离
            dist = (placePos.y - sizeY - GAP) + halfViewSizeH;
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = 0;
                offsetY = -sizeY * 0.5f - GAP;
            }

            //向右试探，放在右面时，计算UI右边和屏幕右边的距离
            dist = halfViewSizeW - (placePos.x + sizeX + GAP);
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = sizeX * 0.5f + GAP;
                offsetY = 0;
            }

            //向左试探，放在左边时，计算UI左边和屏幕左边的距离
            dist = (placePos.x - sizeX - GAP) + halfViewSizeW;
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = -sizeX * 0.5f - GAP;
                offsetY = 0;
            }

            placePos.x += offsetX;
            placePos.y += offsetY;
            uiGo.transform.localPosition = placePos;
        }
        
        //要求uiGo没有被缩放。
        public static void ConstrainUIInScreen(GameObject uiGo)
        {
            var bounds = NGUIMath.CalculateRelativeWidgetBounds(uiGo.transform);
            var pos = uiGo.transform.localPosition;
            var screenSize = GetUIViewSize();
            var newPos = ConstrainRectInScreen(pos.x, pos.y, bounds.size.x * 0.5f, bounds.size.y * 0.5f,
                screenSize.x * 0.5f, screenSize.y * 0.5f);
            pos.x = newPos.x;
            pos.y = newPos.y;
            uiGo.transform.localPosition = pos;
        }
        //返回新的保证界面在屏幕中的坐标
        public static Vector2 ConstrainRectInScreen(float x, float y, float halfSizeX, float halfSizeY,
            float halfScreeH, float halfScreenW)
        {
            var SCREEN_GAP_X = 100f; //和屏幕边缘的距离
            var SCREEN_GAP_Y = 30f; //和屏幕边缘的距离，避开
            var offsetX = 0f;
            var offsetY = 0f;

            halfScreenW = halfScreenW - SCREEN_GAP_X;
            halfScreeH = halfScreeH - SCREEN_GAP_Y;

            //上边是否超出，上边超出了就不处理下边了
            var off = halfScreeH - (y + halfSizeY);
            if(off < 0)
            {
                offsetY = off;
            }
            else
            {
                //下边是否超出
                off = (y - halfSizeY) + halfScreeH;
                if(off < 0)
                {
                    offsetY = -off;
                }
            }

            //左边是否超出，左边超出了就不处理右边了
            off = (x - halfSizeX) + halfScreenW;
            if(off < 0)
            {
                offsetX = -off;
            }
            else
            {
                //右边是否超出
                off = halfScreenW - (x + halfSizeX);
                if(off < 0)
                {
                    offsetX = off;
                }
            }
            return new Vector2(offsetX + x, offsetY + y);
        }

        public static Vector2 GetUIViewSize(bool recalculate = false)
        {
            if(uiViewHeight < 0 || recalculate)
            {
                var screenRatio = Screen.height * 1f / Screen.width;

                //保证分辨率的宽高都不低于w, h
                if(screenRatio > uiScreenHeight / uiScreenWidth)
                {
                    uiViewHeight = uiScreenWidth * screenRatio;
                    uiViewWidth = uiScreenWidth;
                }
                else
                {
                    uiViewHeight = uiScreenHeight;
                    uiViewWidth = uiScreenHeight / screenRatio;
                }
            }

            return new Vector2(uiViewHeight, uiViewWidth);
        }
    }
}

#endif