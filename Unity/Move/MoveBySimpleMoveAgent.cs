using UnityEngine;

namespace Kernel.Unity
{
    public class MoveBySimpleMoveAgent : MoveBase
    {
        public SimpleMoveAgent agent;

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

        public override Vector3 Destination { get; protected set; }

        public override bool IsArrived => agent.IsArrived;
        public override bool IsStopped { get; set; }

        public MoveBySimpleMoveAgent(SimpleMoveAgent agent) : base(agent.transform)
        {
            this.agent = agent;
            transform = agent.transform;
        }

        public override void Reset()
        {
            IsStopped = true;
            agent.Reset();
        }

        public override void SetDestination(Vector3 des)
        {
            agent.SetDestination(des);
        }

        public override void Tick(float deltaTime)
        {
            agent.Tick(deltaTime);
        }
    }
}
