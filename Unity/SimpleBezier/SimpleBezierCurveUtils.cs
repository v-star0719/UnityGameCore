using System.Collections.Generic;
using UnityEngine;

namespace Kernel.Unity
{
    public class SimpleBezierCurveUtils
    {
        public static float[] CalculateDistances(List<BezierPointData> points, out float totalDistance)
        {
            totalDistance = 0;
            if (points.Count == 0)
            {
                return new float[0];
            }

            int n = points.Count - 1;
            var distances = new float[n];
            for (int i = 0; i < n; i++)
            {
                distances[i] = Distance(points[i], points[i + 1]);
                totalDistance += distances[i];
            }

            return distances;
        }

        public static float Distance(BezierPointData p1, BezierPointData p2)
        {
            float d = 0;
            Vector3 ps = p1.position;
            for (float i = 0; i <= 1.001f; i += 0.01f)
            {
                var pe = Kernel.Core.MathUtils.Bezier2(p1.position, p1.controller, p2.position, i);
                d += Vector3.Distance(ps, pe);
                ps = pe;
            }

            return d;
        }

        public static Vector3 Bezier(BezierPointData p1, BezierPointData p2, float t)
        {
            return Kernel.Core.MathUtils.Bezier2(p1.position, p1.controller, p2.position, t);
        }

        public static Quaternion BezierQuaternion(BezierPointData p1, BezierPointData p2, float t)
        {
            return Quaternion.Lerp(p1.quaternion, p2.quaternion, t);
        }
    }
}