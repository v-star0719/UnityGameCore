using UnityEngine;

namespace GameCore.Unity.Tweener
{
    public class TweenAnchorPosition : Tweener
    {
        public Vector3 from;
        public Vector3 to;
        public RectTransform rectTrans;

        private RectTransform RectTrans
        {
            get
            {
                if (rectTrans == null)
                {
                    rectTrans = GetComponent<RectTransform>();
                }
                return rectTrans;
            }
        }

        protected override void OnUpdate(float factor)
        {
            var p = Vector3.Lerp(from, to, factor);
            RectTrans.anchoredPosition = p;
        }
    }
}