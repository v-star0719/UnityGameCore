using UnityEngine;

namespace GameCore.Unity
{
    //todo 角度有点问题，再优化吧
    //相比较于贝塞尔曲线，这个可以很容易的实现很多复杂的效果，比如飞远后，转n圈后再进攻目标。
    //偏移做完后，最终是直线飞向目标
    public class TrajectoryMove : MonoBehaviour
    {
        private Transform target;
        private float speed;

        public VelocityOffsetData[] offsets = new VelocityOffsetData[0];

        public bool IsArrived { get; private set; }

        private float curOffsetValue;
        private int curOffsetIndex;
        private float offsetTimer;
        private VelocityOffsetData curOffsetData;
        private bool isOffsetValueApplied = false;
        private Vector3 curVelocity;
        private float curOffsetAngle;
        private float curOffsetPolar;

        /// <summary>
        ///
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        public void StartFly(Vector3 startPos, Transform target, float speed)
        {
            if (offsets.Length == 0)
            {
                Debug.LogError("not offset data");
                return;
            }

            this.target = target;
            this.speed = speed;

            IsArrived = false;
            transform.position = startPos;
            transform.LookAt(target.position);

            Vector3 dir = target.position - transform.position;

            curOffsetIndex = 0;
            offsetTimer = 0;
            isOffsetValueApplied = false;
            curVelocity = dir.normalized * speed;
            curOffsetData = offsets[0];
            curOffsetValue = curOffsetData.offsetStart;
            enabled = true;
        }

        public void Update()
        {
            if (target == null)
            {
                enabled = false;
                return;
            }

            UpdateOffsetValue();
            UpdateVelocity();

            Vector3 delta = curVelocity * Time.deltaTime;
            var dir = target.position - transform.position;
            if(delta.sqrMagnitude >= dir .sqrMagnitude)
            {
                enabled = false;
                IsArrived = true;
                delta = dir;
            }

            transform.LookAt(transform.position + delta);
            transform.position += delta;
        }

        private void UpdateOffsetValue()
        {
            if (curOffsetIndex >= offsets.Length)
            {
                return;
            }

            offsetTimer += Time.deltaTime;

            if (offsetTimer < curOffsetData.delay)
            {
                return;
            }

            if (!isOffsetValueApplied)
            {
                curOffsetValue = curOffsetData.offsetStart;
                curOffsetAngle = (curOffsetData.offsetAngle < 0 ? Random.Range(0, 360) : curOffsetData.offsetAngle) * Mathf.Deg2Rad;
                curOffsetPolar = (curOffsetData.offsetPolar < 0 ? Random.Range(0, 360) : curOffsetData.offsetPolar) * Mathf.Deg2Rad;
                isOffsetValueApplied = true;
            }

            float t = offsetTimer - curOffsetData.delay;
            if (curOffsetData.duration < 0.01f)
            {
                curOffsetValue = curOffsetData.offsetStart;
            }
            else
            {
                curOffsetValue = Mathf.Lerp(curOffsetData.offsetStart, curOffsetData.offsetEnd,
                    t / curOffsetData.duration);
            }

            if (t >= curOffsetData.duration)
            {
                //切换到下一段
                curOffsetIndex++;
                if (curOffsetIndex < offsets.Length)
                {
                    isOffsetValueApplied = false;
                    curOffsetData = offsets[curOffsetIndex];
                }
                else
                {
                    curOffsetData = null;
                    curOffsetValue = 0;
                }
            }
        }

        private void UpdateVelocity()
        {
            if (curOffsetData == null)
            {
                curVelocity = (target.position - transform.position).normalized * speed;
                return;
            }

            //确定主方向
            Vector3 offsetDir = Vector3.zero;
            Vector3 mainDir;
            if (curOffsetData.offsetType == VelocityOffsetType.PerpendicularToVelocity)
            {
                //始终垂直当前速度方向
                mainDir = curVelocity;
            }
            else
            {
                mainDir = target.position - transform.position;
            }
            mainDir.Normalize();

            //先在世界坐标系下计算好这条线，然后Z+轴旋转到和主方向重合，这条线也跟着旋转一下，就是需要的射线了。
            //将椎顶放在原点，朝向Z+轴，在Z=1处做一个横截面，算出射线在横截面上的点。
            //这个点和原点构成的向量就是待旋转的射线了。
            //  /|
            // / |
            //--------
            // \ |
            //  \|
            if (Mathf.Abs(curOffsetValue) > 0.0001)
            {
                offsetDir.x = Mathf.Cos(curOffsetPolar);
                offsetDir.y = Mathf.Sin(curOffsetPolar);
                offsetDir.z = Mathf.Tan(curOffsetAngle);
                var qua = Quaternion.FromToRotation(Vector3.up, mainDir);
                offsetDir = qua * offsetDir;
            }
            offsetDir.Normalize();
            Debug.DrawLine(transform.position, transform.position + offsetDir * 2);

            //移动速度
            //近似处理。速度大小可能大于speed
            curVelocity = (1 - curOffsetValue) * speed * mainDir + curOffsetValue * speed * offsetDir;
        }
    }
}
