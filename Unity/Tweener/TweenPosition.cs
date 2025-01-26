using UnityEngine;

namespace GameCore.Unity.Tweener
{
    public class TweenPosition : Tweener
    {
        public Vector3 from;
        public Vector3 to;
        public Space space;

        protected override void OnUpdate(float factor)
        {
            var p = Vector3.Lerp(from, to, factor);
            if(space == Space.World)
            {
                transform.position = p;
            }
            else
            {
                transform.localPosition = p;
            }
        }
    }
}