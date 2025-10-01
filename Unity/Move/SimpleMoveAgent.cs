using UnityEngine;

namespace GameCore.Unity
{
    public class SimpleMoveAgent : MonoBehaviour
    {
        public float radius = 0.5f;
        public float speed = 3f;
        public float angularSpeed = 180;
        public float stoppingDistance = 0.5f;
        public float stoppingAngle = 10f;//一般背朝目标不算到

        public Vector3 Destination { get; private set; }

        public bool IsArrived => IsDistanceArrived && IsAngleArrived;
        public bool IsStopped { get; set; }
        public bool IsPaused { get; set; }
        private bool IsDistanceArrived;
        private bool IsAngleArrived;

        public void Reset()
        {
            IsStopped = false;
            IsDistanceArrived = false;
            IsAngleArrived = false;
            IsPaused = false;
            if (stoppingDistance < 0)
            {
                stoppingDistance = 0;//小于0会走过头
            }
        }

        public void SetDestination(Vector3 des)
        {
            Destination = des;
            var dir = Destination - transform.position;
            IsDistanceArrived = dir.sqrMagnitude <= stoppingDistance * stoppingDistance;
            IsAngleArrived = Vector3.Angle(dir, transform.forward) <= stoppingAngle;
        }

        public void Tick(float deltaTime)
        {
            if (IsArrived || IsStopped || IsPaused)
            {
                return;
            }

            var dir = Destination - transform.position;
            if (!IsDistanceArrived)
            {
                var dist = dir.magnitude;
                if(dist <= stoppingDistance)
                {
                    //如果当前已经在停止距离内，不移动
                    IsDistanceArrived = true;
                }
                else
                {
                    float delta = speed * deltaTime;
                    if(dist - delta <= stoppingDistance)
                    {
                        IsDistanceArrived = true;
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
            }

            if (!IsAngleArrived)
            {
                if(dir.sqrMagnitude < 0.0001f)
                {
                    IsAngleArrived = true;//当位置和目标点重回后，就不能朝向目标点了
                }
                else
                {
                    var angle = Mathf.Abs(Vector2.Angle(transform.forward, dir));
                    var targetRotation = Quaternion.LookRotation(dir);
                    var deltaAngle = angularSpeed * deltaTime;
                    if(deltaAngle >= angle)
                    {
                        transform.rotation = targetRotation;
                        IsAngleArrived = true;
                    }
                    else
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, deltaAngle / angle);
                        IsAngleArrived = (angle - deltaAngle) <= stoppingAngle;
                    }
                }
            }
        }
    }
}
