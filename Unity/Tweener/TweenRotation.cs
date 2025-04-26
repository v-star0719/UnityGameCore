using UnityEngine;

namespace GameCore.Unity.Tweener
{
    public class TweenRotation : Tweener
    {
        public Vector3 from = Vector3.zero;
        public Vector3 to = Vector3.zero;

        protected override void OnUpdate(float factor)
        {
            var p = Vector3.Lerp(from, to, factor);
            transform.localEulerAngles = p;
        }
    }
}