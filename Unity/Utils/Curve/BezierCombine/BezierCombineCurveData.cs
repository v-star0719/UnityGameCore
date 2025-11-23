using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    //多个3阶贝塞尔曲线组合
    //顶点、控制点、控制点、顶点、控制点、控制点、……
    [CreateAssetMenu(fileName = "BezierCombineCurve", menuName = "VStar/Curve/BezierComine", order = 2)]
    public class BezierCombineCurveData : CurveData
    {
        public override Vector3 GetPos(float ratio)
        {
            if (points.Count < 4)
            {
                Debug.LogWarning("BezierCombineCurveData point count is less than 4");
                return Vector3.zero;
            }
            float f = ratio * ((points.Count - 1) / 3);
            int i = (int) f;
            f = f - i;
            i = i * 3;
            return Curves.MathUtils.Bezier3(points[i], points[i + 1], points[i + 2], points[i + 3], f);
        }

        public override List<Vector3> ToSegments()
        {
            var step = 1f / smooth;
            var rt = new List<Vector3>(20);
            for (int i = 0; i < points.Count && i + 3 < points.Count; i += 3)
            {
                for (int j = 0; j <= smooth; j++)
                {
                    rt.Add(Curves.MathUtils.Bezier3(points[i], points[i + 1], points[i + 2], points[i + 3], j * step));
                }
            }
            return rt;
        }

        public override void DrawGizmos(Color color, Matrix4x4 matrix, List<Vector3> points, int smooth, params object[] args)
        {
            for (int i = 0; i < points.Count && i + 3 < points.Count; i += 3)
            {
                DrawCurve(matrix * points[i], matrix * points[i + 1], matrix * points[i + 2], matrix * points[i + 3], smooth);
            }
        }

        void DrawCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int smooth)
        {
            Vector3 start = p0;
            float step = 1f / smooth;
            for (int i = 0; i <= smooth; i++)
            {
                var end = Curves.MathUtils.Bezier3(p0, p1, p2, p3, i * step);
                Gizmos.DrawLine(start, end);
                start = end;
            }
        }
    }
}