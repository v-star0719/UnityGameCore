using UnityEngine;

namespace Kernel.Unity
{
    public abstract class MoveBase
    {
        public virtual float radius { get; set; }

        public virtual float speed { get; set; }

        public virtual float angularSpeed { get; set; }

        public virtual float stoppingDistance { get; set; }

        public virtual Vector3 Destination { get; protected set; }

        public virtual bool IsArrived { get; protected set; }

        public virtual bool IsStopped { get; set; }

        public virtual int priority { get; set; }//≈ˆ◊≤”≈œ»º∂

        protected Transform transform;

        protected MoveBase(Transform trans)
        {
            transform = trans;
        }

        public virtual void Reset()
        {
            IsStopped = true;
        }

        public virtual void SetDestination(Vector3 des)
        {
        }

        public virtual void Tick(float deltaTime)
        {
        }
    }
}
