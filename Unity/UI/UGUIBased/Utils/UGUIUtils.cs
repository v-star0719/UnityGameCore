using GameCore.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public static class UGUIUtils
    {
        public enum AutoPlaceUIPreferDir
        {
            None,
            Horizontal,
            Vertical
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
        public static void AutoPlaceUI(RectTransform trans, Vector3 worldPlacePos, AutoPlaceUIPreferDir preferDir)
        {
            const float GAP = 100; //UI边缘和当前位置的间距，即图中箭头的长度。

            //获取UI的大小。
            var bounds = RectTransformUtils.CalculateBounding(trans, Space.Self);
            var localPlacePos = trans.InverseTransformPoint(worldPlacePos);
            var size = bounds.size;
            //Debug.Log($"AutoPlaceUI: min={bounds.min}, max={bounds.max}, size={size}, placePos={localPlacePos}");
            
            //让将ui的中心和placePos重合
            localPlacePos.z = 0;
            localPlacePos = localPlacePos - bounds.center;
            
            var viewSize = GetViewSize(trans);
            var halfViewSizeW = viewSize.x * 0.5f;
            var halfViewSizeH = viewSize.y * 0.5f;
            var sizeX = size.x;
            var sizeY = size.y;


            //向上试探，放在上面时，计算UI上边和屏幕上边的距离
            var maxDistV = -9999f;
            var offsetXV = 0f;
            var offsetYV = 0f; var dist = halfViewSizeH - (localPlacePos.y + sizeY + GAP);
            if(maxDistV < dist)
            {
                maxDistV = dist;
                offsetXV = 0;
                offsetYV = sizeY * 0.5f + GAP;
            }

            //向下试探，放在下面时，计算UI下边和屏幕下边的距离
            dist = (localPlacePos.y - sizeY - GAP) + halfViewSizeH;
            if(maxDistV < dist)
            {
                maxDistV = dist;
                offsetXV = 0;
                offsetYV = -sizeY * 0.5f - GAP;
            }

            //垂直优先判断
            if(maxDistV > 0 && preferDir == AutoPlaceUIPreferDir.Vertical)
            {
                maxDistV = 999999;//后面会优先垂直方向放
            }

            //向右试探，放在右面时，计算UI右边和屏幕右边的距离
            var maxDistH = -9999f;
            var offsetXH = 0f;
            var offsetYH = 0f; dist = halfViewSizeW - (localPlacePos.x + sizeX + GAP);
            if(maxDistH < dist)
            {
                maxDistH = dist;
                offsetXH = sizeX * 0.5f + GAP;
                offsetYH = 0;
            }

            //向左试探，放在左边时，计算UI左边和屏幕左边的距离
            dist = (localPlacePos.x - sizeX - GAP) + halfViewSizeW;
            if(maxDistH < dist)
            {
                maxDistH = dist;
                offsetXH = -sizeX * 0.5f - GAP;
                offsetYH = 0;
            }

            //水平优先判断
            if(maxDistH > 0 && preferDir == AutoPlaceUIPreferDir.Horizontal)
            {
                maxDistH = 999999;
            }

            if (maxDistV > maxDistH)
            {
                localPlacePos.x += offsetXV;
                localPlacePos.y += offsetYV;
            }
            else
            {
                localPlacePos.x += offsetXH;
                localPlacePos.y += offsetYH;
            }
            trans.localPosition = localPlacePos;

        }

        //要求uiGo没有被缩放。
        public static void ConstrainUIInScreen(RectTransform trans)
        {
            var bounds = RectTransformUtils.CalculateBounding(trans, Space.Self);
            var pos = trans.localPosition;
            var screenSize = GetViewSize(trans);
            var newPos = ConstrainRectInScreen(pos.x, pos.y, bounds.size.x * 0.5f, bounds.size.y * 0.5f, screenSize.x * 0.5f, screenSize.y * 0.5f);
            pos.x = newPos.x;
            pos.y = newPos.y;
            trans.localPosition = pos;
        }

        //返回新的保证界面在屏幕中的坐标
        public static Vector2 ConstrainRectInScreen(float x, float y, float halfSizeX, float halfSizeY, float halfScreenW, float halfScreenH)
        {
            var SCREEN_GAP_X = 100f; //和屏幕边缘的距离
            var SCREEN_GAP_Y = 30f; //和屏幕边缘的距离，避开
            var offsetX = 0f;
            var offsetY = 0f;

            halfScreenW = halfScreenW - SCREEN_GAP_X;
            halfScreenH = halfScreenH - SCREEN_GAP_Y;

            //上边是否超出，上边超出了就不处理下边了
            var off = halfScreenH - (y + halfSizeY);
            if(off < 0)
            {
                offsetY = off;
            }
            else
            {
                //下边是否超出
                off = (y - halfSizeY) + halfScreenH;
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

        public static Vector2 GetViewSize(RectTransform trans)
        {
            return trans.GetComponentInParent<CanvasScaler>().GetComponent<RectTransform>().sizeDelta;
        }

        //放大或者缩小的宽高来适应size，完全填满size
        public static void RectFitView(RectTransform rect, Vector2 size)
        {
            var r = rect.rect;
            var rectRatio = r.width / r.height;
            var sizeRatio = size.x / size.y;

            if (rectRatio > sizeRatio)
            {
                //rect比size更宽，让高度符合size的高度。这时宽度可能超出size区域，反过来的画宽度可能不够。
                r.height = size.y;
                r.width = size.y * rectRatio;
            }
            else
            {
                //rect比size更窄，让宽度符合size的宽度，这时高度可能超出size，反过来的画高度可能不够
                r.width = size.x;
                r.height = size.x / rectRatio;
            }

            rect.sizeDelta = r.size;
        }

        public static void SetAlpha(this Image image, float alpha)
        {
            var clr = image.color;
            clr.a = alpha;
            image.color = clr;
        }
    }
}