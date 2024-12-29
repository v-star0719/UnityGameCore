using GameCore.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class UGUIUtils
    {
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
        public static void AutoPlaceUI(RectTransform trans, Vector3 worldPlacePos)
        {
            const float GAP = 100; //UI边缘和当前位置的间距，即图中箭头的长度。

            //获取UI的大小。
            var bounds = RectTransformUtils.CalculateBounding(trans, Space.Self);
            var localPlacePos = trans.InverseTransformPoint(worldPlacePos);
            var size = bounds.size;
            Debug.Log($"AutoPlaceUI: min={bounds.min}, max={bounds.max}, size={size}, placePos={localPlacePos}");
            
            //让将ui的中心和placePos重合
            localPlacePos.z = 0;
            localPlacePos = localPlacePos - bounds.center;
            
            var viewSize = GetViewSize(trans);
            var halfViewSizeW = viewSize.x * 0.5f;
            var halfViewSizeH = viewSize.y * 0.5f;
            var sizeX = size.x;
            var sizeY = size.y;
            var offsetX = 0f;
            var offsetY = 0f;

            //向上试探，放在上面时，计算UI上边和屏幕上边的距离
            var maxDist = -9999f;
            var dist = halfViewSizeH - (localPlacePos.y + sizeY + GAP);
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = 0;
                offsetY = sizeY * 0.5f + GAP;
            }

            //向下试探，放在下面时，计算UI下边和屏幕下边的距离
            dist = (localPlacePos.y - sizeY - GAP) + halfViewSizeH;
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = 0;
                offsetY = -sizeY * 0.5f - GAP;
            }

            //向右试探，放在右面时，计算UI右边和屏幕右边的距离
            dist = halfViewSizeW - (localPlacePos.x + sizeX + GAP);
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = sizeX * 0.5f + GAP;
                offsetY = 0;
            }

            //向左试探，放在左边时，计算UI左边和屏幕左边的距离
            dist = (localPlacePos.x - sizeX - GAP) + halfViewSizeW;
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = -sizeX * 0.5f - GAP;
                offsetY = 0;
            }

            localPlacePos.x += offsetX;
            localPlacePos.y += offsetY;
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
    }
}