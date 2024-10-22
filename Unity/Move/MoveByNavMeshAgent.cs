using UnityEngine;
using UnityEngine.AI;

namespace Kernel.Unity
{
    public class MoveByNavMeshAgent : MoveBase
    {
        public NavMeshAgent agent;

        public override float radius
        {
            get => agent.radius;
            set => agent.radius = value;
        }

        public override float speed
        {
            get => agent.speed;
            set => agent.speed = value;
        }

        public override float angularSpeed
        {
            get => agent.angularSpeed;
            set => agent.angularSpeed = value;
        }

        public override float stoppingDistance
        {
            get => agent.stoppingDistance;
            set => agent.stoppingDistance = value;
        }

        public override Vector3 Destination
        {
            get => agent.destination;
            protected set
            {
                Debug.LogError("set is not available");
            }
        }

        public override int priority
        {
            get => agent.avoidancePriority;
            set => agent.avoidancePriority = value;
        }

        public override bool IsArrived { get; protected set; }

        public override bool IsStopped
        {
            get => agent.isStopped;
            set => agent.isStopped = value;
        }

        public override bool IsPaused
        {
            get => agent.enabled;
            set => agent.enabled = value;
        }

        private float stoppingDistanceSqr;

        public MoveByNavMeshAgent(NavMeshAgent agent) : base(agent.transform)
        {
            this.agent = agent;
        }

        public override void Reset()
        {
            IsStopped = true;
            stoppingDistanceSqr = stoppingDistance * stoppingDistance;
        }

        public override void SetDestination(Vector3 des)
        {
            agent.SetDestination(des);
            IsArrived = (Destination - transform.position).sqrMagnitude <= stoppingDistanceSqr;
        }

        public override void Tick(float deltaTime)
        {
            if (IsArrived || IsStopped)
            {
                return;
            }

            var dir = Destination - transform.position;
            if (dir.sqrMagnitude <= stoppingDistanceSqr)
            {
                IsArrived = true;
            }
        }
    }
}
