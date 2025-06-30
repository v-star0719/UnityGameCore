using UnityEngine;

namespace GameCore.Unity.Tweener
{
    [RequireComponent(typeof(RectTransform))]
    public class TweenSize : Tweener
    {
        public Vector2 from = Vector2.zero;
        public Vector2 to = Vector2.zero;
        private RectTransform __rectTrans;

        public RectTransform RectTrans
        {
            get
            {
                if (__rectTrans == null)
                {
                    __rectTrans = GetComponent<RectTransform>();
                }
                return __rectTrans;
            }
        }

        protected override void OnUpdate(float factor)
        {
            RectTrans.sizeDelta = Vector3.Lerp(from, to, factor);
        }
    }
}