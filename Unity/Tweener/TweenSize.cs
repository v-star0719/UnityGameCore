using UnityEngine;

namespace GameCore.Unity.Tweener
{
    [RequireComponent(typeof(RectTransform))]
    public class TweenSize : Tweener
    {
        public Vector2 from = Vector2.zero;
        public Vector2 to = Vector2.zero;
        public RectTransform target;

        public RectTransform Target
        {
            get
            {
                if (target == null)
                {
                    target = GetComponent<RectTransform>();
                }
                return target;
            }
        }

        protected override void OnUpdate(float factor)
        {
            Target.sizeDelta = Vector3.Lerp(from, to, factor);
        }
    }
}