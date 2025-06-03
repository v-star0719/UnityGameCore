namespace GameCore.Unity
{
    //给子弹施加的速度偏移，实现各种神奇的弹道。
    //偏移方向在椎体表面上，从锥顶射出。
    //偏移方向的速度+主方向的速度=速度。方向和速度大小都满足这个等式。
    [System.Serializable]
    public class VelocityOffsetData
    {
        public VelocityOffsetType offsetType;
        public float delay;
        public float duration = 1;
        public float offsetStart = 1;//偏移速度的起始大小。如果是1，这主方向的速度大小为0。
        public float offsetEnd = 0;//偏移速度的结束大小
        public float offsetAngle;//椎体角度。-1表示随机。
        public float offsetPolar;//横截面极坐标值。-1表示随机。以椎体朝向为Z+轴，横截面是XY平面。偏移方向在XY平面上做投影，得到一条射线，就是这条射线和x轴的角度。
    }

    public enum VelocityOffsetType
    {
        PerpendicularToTargetDir,
        PerpendicularToVelocity,
    }
}
