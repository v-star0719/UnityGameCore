using System;
using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    public partial class UIAnchorToObject
    {
        [Serializable]
        public abstract class AnchorDataBase
        {
            public RectTransform target;
            public float offset;

            private Vector2 lastPos;
            private float lastOffset;

            public virtual bool IsChanged()
            {
                return target != null && (target.anchoredPosition != lastPos || lastOffset != offset);
            }

            public virtual void MarkNotChanged()
            {
                lastPos = target.anchoredPosition;
                lastOffset = offset;
            }

            public virtual Vector3 GetPos()
            {
                return target.position;
            }
        }

        [Serializable]
        public class AnchorDataVertical : AnchorDataBase
        {
            public RectSideVertical side;

            private RectSideVertical lastSide;

            public override bool IsChanged()
            {
                return lastSide != side || base.IsChanged();
            }

            public override void MarkNotChanged()
            {
                base.MarkNotChanged();
                lastSide = side;
            }

            public override Vector3 GetPos()
            {
                if(target == null)
                {
                    Debug.LogError("target is not assigned");
                    return Vector3.zero;
                }
                switch(side)
                {
                    case RectSideVertical.Top:
                        return target.TransformPoint(target.rect.TopMiddle() - new Vector2(0, offset));
                    case RectSideVertical.Bottom:
                        return target.TransformPoint(target.rect.BottomMiddle() - new Vector2(0, offset));
                    case RectSideVertical.Center:
                        return target.TransformPoint(0, offset, 0);
                }
                return target.position;
            }
        }

        [Serializable]
        public class AnchorDataHorizontal : AnchorDataBase
        {
            public RectSideHorizontal side;

            private RectSideHorizontal lastSide;

            public override bool IsChanged()
            {
                return lastSide != side || base.IsChanged();
            }

            public override void MarkNotChanged()
            {
                base.MarkNotChanged();
                lastSide = side;
            }
            public override Vector3 GetPos()
            {
                if(target == null)
                {
                    Debug.LogError("target is not assigned");
                    return Vector3.zero;
                }
                switch(side)
                {
                    case RectSideHorizontal.Left:
                        return target.TransformPoint(target.rect.MiddleLeft() + new Vector2(offset, 0));
                    case RectSideHorizontal.Right:
                        return target.TransformPoint(target.rect.MiddleRight() + new Vector2(offset, 0));
                    case RectSideHorizontal.Center:
                        return target.TransformPoint(offset, 0, 0);
                }
                return target.position;
            }
        }

        public enum RectSideHorizontal
        {
            Left,
            Center,
            Right,
        }

        public enum RectSideVertical
        {
            Top,
            Center,
            Bottom,
        }
    }
}