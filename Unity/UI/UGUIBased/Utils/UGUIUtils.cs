using GameCore.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public static class UGUIUtils
    {
        //     ��
        //     ��
        // �� �� O �� ��
        //     ��
        //     ��
        //�ĸ�����̽Ѱ����UI����ָ��λ�õ����桢���桢��ߡ������ұߡ�ֻҪ�ܷŽ���Ļ���ɣ���һ������ͨ��ƽ�ư����Ų����Ļ�ڡ�
        //�����������Ŀռ乻����UI�ŵ����棬Ȼ������ƽ�ƾͿ��԰����Ž���Ļ�ڡ�
        //gameObject: UI�ĸ��ڵ㣬����Ҫ��һ���ڵ���������UI��λ�ã�ʹUI�ĸ��ڵ��UI�����غϡ�
        //placePos���������ꡣͨ��panelGoת��Ϊ����ڵľֲ����ꡣ
        //Ҫ��uiGo��ʼ����Ļ����
        public static void AutoPlaceUI(RectTransform trans, Vector3 worldPlacePos)
        {
            const float GAP = 100; //UI��Ե�͵�ǰλ�õļ�࣬��ͼ�м�ͷ�ĳ��ȡ�

            //��ȡUI�Ĵ�С��
            var bounds = RectTransformUtils.CalculateBounding(trans, Space.Self);
            var localPlacePos = trans.InverseTransformPoint(worldPlacePos);
            var size = bounds.size;
            Debug.Log($"AutoPlaceUI: min={bounds.min}, max={bounds.max}, size={size}, placePos={localPlacePos}");
            
            //�ý�ui�����ĺ�placePos�غ�
            localPlacePos.z = 0;
            localPlacePos = localPlacePos - bounds.center;
            
            var viewSize = GetViewSize(trans);
            var halfViewSizeW = viewSize.x * 0.5f;
            var halfViewSizeH = viewSize.y * 0.5f;
            var sizeX = size.x;
            var sizeY = size.y;
            var offsetX = 0f;
            var offsetY = 0f;

            //������̽����������ʱ������UI�ϱߺ���Ļ�ϱߵľ���
            var maxDist = -9999f;
            var dist = halfViewSizeH - (localPlacePos.y + sizeY + GAP);
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = 0;
                offsetY = sizeY * 0.5f + GAP;
            }

            //������̽����������ʱ������UI�±ߺ���Ļ�±ߵľ���
            dist = (localPlacePos.y - sizeY - GAP) + halfViewSizeH;
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = 0;
                offsetY = -sizeY * 0.5f - GAP;
            }

            //������̽����������ʱ������UI�ұߺ���Ļ�ұߵľ���
            dist = halfViewSizeW - (localPlacePos.x + sizeX + GAP);
            if(maxDist < dist)
            {
                maxDist = dist;
                offsetX = sizeX * 0.5f + GAP;
                offsetY = 0;
            }

            //������̽���������ʱ������UI��ߺ���Ļ��ߵľ���
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

        //Ҫ��uiGoû�б����š�
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

        //�����µı�֤��������Ļ�е�����
        public static Vector2 ConstrainRectInScreen(float x, float y, float halfSizeX, float halfSizeY, float halfScreenW, float halfScreenH)
        {
            var SCREEN_GAP_X = 100f; //����Ļ��Ե�ľ���
            var SCREEN_GAP_Y = 30f; //����Ļ��Ե�ľ��룬�ܿ�
            var offsetX = 0f;
            var offsetY = 0f;

            halfScreenW = halfScreenW - SCREEN_GAP_X;
            halfScreenH = halfScreenH - SCREEN_GAP_Y;

            //�ϱ��Ƿ񳬳����ϱ߳����˾Ͳ������±���
            var off = halfScreenH - (y + halfSizeY);
            if(off < 0)
            {
                offsetY = off;
            }
            else
            {
                //�±��Ƿ񳬳�
                off = (y - halfSizeY) + halfScreenH;
                if(off < 0)
                {
                    offsetY = -off;
                }
            }

            //����Ƿ񳬳�����߳����˾Ͳ������ұ���
            off = (x - halfSizeX) + halfScreenW;
            if(off < 0)
            {
                offsetX = -off;
            }
            else
            {
                //�ұ��Ƿ񳬳�
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

        public static void SetAlpha(this Image image, float alpha)
        {
            var clr = image.color;
            clr.a = alpha;
            image.color = clr;
        }
    }
}