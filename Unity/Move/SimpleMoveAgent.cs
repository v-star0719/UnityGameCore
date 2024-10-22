using UnityEngine;

namespace Kernel.Unity
{
    public class SimpleMoveAgent : MonoBehaviour
    {
        public float radius = 0.5f;
        public float speed = 3f;
        public float angularSpeed = 180;
        public float stoppingDistance = 0.5f;

        public Vector3 Destination { get; private set; }

        public bool IsArrived { get; private set; }
        public bool IsStopped { get; set; }
        public bool IsPaused { get; set; }

        public void Reset()
        {
            IsStopped = false;
            IsArrived = true;
            if (stoppingDistance < 0)
            {
                stoppingDistance = 0;//小于0会走过头
            }
        }

        public void SetDestination(Vector3 des)
        {
            Destination = des;
            IsArrived = (Destination - transform.position).sqrMagnitude <= stoppingDistance * stoppingDistance;
        }

        public void Tick(float deltaTime)
        {
            if (IsArrived || IsStopped || IsPaused)
            {
                return;
            }

            var dir = Destination - transform.position;
            var dist = dir.magnitude;
            if (dist <= stoppingDistance)
            {
                //如果当前已经在停止距离内，不移动
                IsArrived = true;
            }
            else
            {
                float delta = speed * deltaTime;
                if (dist - delta <= stoppingDistance)
                {
                    IsArrived = true;
                    delta = dist - stoppingDistance;
                }

                //撞墙检测
                //if (Physics.Raycast(transform.position, dir, out var hitInfo, radius + 0.5f, 1 << Layers.barrier))
                //{
                //    //快撞墙了
                //    if (dist - radius < delta)
                //    {
                        
                //        //撞上了，沿 被击中表面法向量在xz平面上的垂线移动
                //        var normal = new Vector3(hitInfo.normal.z, hitInfo.normal.x);
                //        normal.Normalize();
                //        delta = delta * Vector3.Dot(normal, dir) / dir.magnitude;
                //        if (delta < 0)
                //        {
                //            delta = -delta;
                //            dir = -normal;
                //        }
                //        else
                //        {
                //            dir = normal;
                //        }
                //    }
                //}
                transform.position += delta * dir.normalized;
            }
            
            var angle = Vector2.Angle(transform.forward, dir);
            var targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, angularSpeed * deltaTime / angle);
        }
    }
}
