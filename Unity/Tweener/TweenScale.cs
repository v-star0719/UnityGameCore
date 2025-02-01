using UnityEngine;

namespace GameCore.Unity.Tweener
{
    public class TweenScale : Tweener
    {
        public Vector3 from;
        public Vector3 to;

        protected override void OnUpdate(float factor)
        {
            var p = Vector3.Lerp(from, to, factor);
            transform.localScale = p;
        }
    }
}