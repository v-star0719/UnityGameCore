using UnityEngine;

namespace GameCore.Unity.Tweener
{
    public class TweenPosition : Tweener
    {
        public Transform _target;
        public Vector3 from = Vector3.zero;
        public Vector3 to = Vector3.zero;
        public Space space;

        public Transform Target
        {
            get
            {
                if(_target == null)
                {
                    _target = transform;
                }
                return _target;
            }
        }

        protected override void OnUpdate(float factor)
        {
            var p = Vector3.Lerp(from, to, factor);
            if(space == Space.World)
            {
                Target.position = p;
            }
            else
            {
                Target.localPosition = p;
            }
        }
    }
}