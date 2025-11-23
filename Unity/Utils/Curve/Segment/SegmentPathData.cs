using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    //N阶贝塞尔曲线
    [CreateAssetMenu(fileName = "SegmentPath", menuName = "VStar/Curve/SegmentPath", order = 2)]
    public class SegmentPathData : CurveData
    {
        public override Vector3 GetPos(float ratio)
        {
            if (ratio >= 1)
            {
                return points[points.Count - 1];
            }
            float f = ratio * points.Count;
            int i = (int)f;
            f = f - i;
            return Vector3.Lerp(points[i], points[i + 1], f);
        }

        public override List<Vector3> ToSegments()
        {
            var step = 1f / smooth;
            var rt = new List<Vector3>(20);
            for (int j = 0; j <= smooth; j++)
            {
                rt.Add(GetPos(j * step));
            }
            return rt;
        }

        public override void DrawGizmos(Color color, Matrix4x4 matrix, List<Vector3> points, int smooth, params object[] args)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
    }
}