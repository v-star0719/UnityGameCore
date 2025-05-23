using UnityEngine;

namespace GameCore.Unity.Tweener
{
    public class TweenScale : Tweener
    {
        public Vector3 from = Vector3.one;
        public Vector3 to = Vector3.one;

        protected override void OnUpdate(float factor)
        {
            var p = Vector3.Lerp(from, to, factor);
            transform.localScale = p;
        }
    }
}