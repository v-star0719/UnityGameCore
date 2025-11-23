using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    //N阶贝塞尔曲线
    [CreateAssetMenu(fileName = "BSpline", menuName = "VStar/Curve/BSpline", order = 2)]
    public class BSplineData : CurveData
    {
        public List<float> uList = new List<float>();
        public int polynomial = 3;
        public bool isClamped = true;

        public override Vector3 GetPos(float ratio)
        {
            if (!isClamped)
            {
                ratio = Mathf.Lerp(uList[polynomial], uList[uList.Count - polynomial], ratio);
            }
            return Curves.MathUtils.BSpline(points, polynomial, ratio, uList);
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
            var polynomial = (int) args[0];
            var uList = args[1] as List<float>;
            var step = 1f / smooth;
            Vector3 prePoint = Vector3.zero;
            for (int j = 0; j <= smooth; j++)
            {
                var curPoint = matrix * Curves.MathUtils.BSpline(points, polynomial, j * step, uList);
                if (j > 0)
                {
                    Gizmos.DrawLine(prePoint, curPoint);
                }
                prePoint = curPoint;
            }
        }
    }
}