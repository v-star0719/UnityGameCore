using UnityEngine;

namespace GameCore.Unity.Tweener
{
    public class TweenScale : Tweener
    {
        public Transform target;
        public Vector3 from = Vector3.one;
        public Vector3 to = Vector3.one;

        public Transform Target
        {
            get
            {
                if(target == null)
                {
                    target = transform;
                }
                return target;
            }
        }

        protected override void OnUpdate(float factor)
        {
            var p = Vector3.Lerp(from, to, factor);
            Target.localScale = p;
        }
    }
}