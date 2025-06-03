using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    [ExecuteAlways]
    public partial class UIAnchorToObject : MonoBehaviour
    {
        public AnchorDataHorizontal leftAnchor;
        public AnchorDataHorizontal rightAnchor;
        public AnchorDataVertical topAnchor;
        public AnchorDataVertical bottomAnchor;
        private RectTransform rectTransform;

        public void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Update()
        {
            var refresh = false;
            if (leftAnchor.IsChanged())
            {
                refresh = true;
                leftAnchor.MarkNotChanged();
            }
            if (rightAnchor.IsChanged())
            {
                refresh = true;
                rightAnchor.MarkNotChanged();
            }
            if (topAnchor.IsChanged())
            {
                refresh = true;
                topAnchor.MarkNotChanged();
            }
            if (bottomAnchor.IsChanged())
            {
                refresh = true;
                bottomAnchor.MarkNotChanged();
            }
            if (refresh)
            {
                var left = leftAnchor.target != null ? rectTransform.InverseTransformPoint(leftAnchor.GetPos()) : (Vector3)rectTransform.rect.MiddleLeft();
                var right = rightAnchor.target != null ? rectTransform.InverseTransformPoint(rightAnchor.GetPos()) : (Vector3)rectTransform.rect.MiddleRight();
                var top = topAnchor.target != null ? rectTransform.InverseTransformPoint(topAnchor.GetPos()) : (Vector3)rectTransform.rect.TopMiddle();
                var bottom = topAnchor.target != null ? rectTransform.InverseTransformPoint(bottomAnchor.GetPos()) : (Vector3)rectTransform.rect.BottomMiddle();
                rectTransform.sizeDelta = new Vector2(Mathf.Abs(left.x - right.x), Mathf.Abs(top.y - bottom.y));
                rectTransform.localPosition += new Vector3((left.x + right.x) * 0.5f, (top.y + bottom.y) * 0.5f, 0);
            }
        }
    }

    public static class RectExt
    {
        public static Vector2 MiddleLeft(this Rect rect)
        {
            return new Vector2(rect.center.x - rect.width * 0.5f, rect.center.y);
        }

        public static Vector2 MiddleRight(this Rect rect)
        {
            return new Vector2(rect.center.x + rect.width * 0.5f, rect.center.y);
        }

        public static Vector2 TopMiddle(this Rect rect)
        {
            return new Vector2(rect.center.x, rect.center.y + rect.height * 0.5f);
        }

        public static Vector2 BottomMiddle(this Rect rect)
        {
            return new Vector2(rect.center.x, rect.center.y - rect.height * 0.5f);
        }
    }
}
