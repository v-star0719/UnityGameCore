using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    //N阶贝塞尔曲线
    [CreateAssetMenu(fileName = "BezierCurve", menuName = "VStar/Curve/BezierCurve", order = 2)]
    public class BezierCurveData : CurveData
    {
        public override Vector3 GetPos(float ratio)
        {
            return Curves.MathUtils.BezierN(points, ratio);
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
            var step = 1f / smooth;
            Vector3 prePoint = Vector3.zero;
            for (int j = 0; j <= smooth; j++)
            {
                var curPoint = matrix * Curves.MathUtils.BezierN(points, j * step);
                if (j > 0)
                {
                    Gizmos.DrawLine(prePoint, curPoint);
                }
                prePoint = curPoint;
            }
        }
    }
}