using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    [CreateAssetMenu(fileName = "CatmullRomPath", menuName = "VStar/Curve/CatmullRomPath", order = 3)]
    public class CatmullRomPathData : CurveData
    {
        public override Vector3 GetPos(float ratio)
        {
            return MathUtils.CatmullRom(points, ratio);
        }

		public override void DrawGizmos(Color color, Matrix4x4 matrix, List<Vector3> points, int smooth, params object[] args)
		{
            if (points.Count <= 3)
            {
                return;
            }

			Vector3 prevPt = matrix * points[1];
            var step = 1f / smooth;
			for (int i=0; i<= smooth; i++)
			{
				Vector3 currPt = matrix * MathUtils.CatmullRom(points, i * step);
				Gizmos.DrawLine(currPt, prevPt);
				prevPt = currPt;
			}
		}

        public override List<Vector3> ToSegments()
        {
            List<Vector3> rt = new List<Vector3>();
            var step = 1f / (points.Count - 1) / smooth;
            for (float i = step; i <= 1; i += step)
            {
                rt.Add(MathUtils.CatmullRom(points, i));
            }
			return rt;
        }

		//在前后生成两个控制点
		public static void GenerateControlPoints(List<Vector3> pathPoints)
		{
            if (pathPoints.Count < 3)
            {
                return;
            }

            pathPoints.Insert(0, Vector3.zero);
            pathPoints.Add(Vector3.zero);
            var n = pathPoints.Count;
            pathPoints[0] = pathPoints[1] + (pathPoints[1] - pathPoints[2]);
            pathPoints[n - 1] = pathPoints[n - 2] + (pathPoints[n - 2] - pathPoints[n - 3]);
        }
    }
}