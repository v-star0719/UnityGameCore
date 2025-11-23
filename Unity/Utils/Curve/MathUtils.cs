using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    public class MathUtils
    {
        public static Vector2 Bezier2(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            var t1 = 1 - t;
            return t1 * t1 * p0 + 2 * t * t1 * p1 + t * t * p2;
        }

        public static Vector3 Bezier2(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            var t1 = 1 - t;
            return t1 * t1 * p0 + 2 * t * t1 * p1 + t * t * p2;
        }

        public static Vector3 Bezier3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var t1 = 1 - t;
            var t1_2 = t1 * t1;
            var t_2 = t * t;
            return t1_2 * t1 * p0 + 3 * t * t1_2 * p1 + 3 * t_2 * t1 * p2 + t_2 * t * p3;
        }

        public static Vector3 BezierN(List<Vector3> points, float f)
        {
            //通用公式
            var count = points.Count;
            var n = count - 1;
            Vector3 pos = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                pos += Combination(n, i) * Mathf.Pow(1 - f, n - i) * Mathf.Pow(f, i) * points[i];
            }
            return pos;
        }

        public static long Combination(int n, int c)
        {
            long a = 1;
            long b = 1;
            for (int i = c; i > 0; i--)
            {
                a *= n;
                b *= c;
                n--;
                c--;
            }
            return a / b;
        }

        public static Vector3 CatmullRom(List<Vector3> pathPoints, float t)
        {
            int numSections = pathPoints.Count - 3;
            int p = Mathf.Min((int)(t * numSections), numSections - 1);
            float u = t * (float)numSections - (float)p;

            Vector3 a = pathPoints[p];
            Vector3 b = pathPoints[p + 1];
            Vector3 c = pathPoints[p + 2];
            Vector3 d = pathPoints[p + 3];

            return 0.5f * ((-a + 3f * b - 3f * c + d) * 
                (u * u * u) + (2f * a - 5f * b + 4f * c - d) * 
                (u * u) + (-a + c) * u + 2f * b);
        }

        public static Vector3 BSpline(List<Vector3> pathPoints, int p, float t, List<float> uArray)
        {
            Vector3 pos = Vector3.zero;
            for (int i = 0; i < pathPoints.Count; i++)
            {
                pos += Cox_deBoor(i, p, t, uArray) * pathPoints[i]; //Combination(n, i) * 
            }
            return pos;
        }

        public static float Cox_deBoor(int i, int p, float u, List<float> uArray)
        {
            if (p == 0)
            {
                return uArray[i] <= u && u < uArray[i + 1] ? 1f : 0f;
            }
            else
            {
                //return (u - uArray[i]) / (uArray[i + p] - uArray[i]) * Cox_deBoor(i, p - 1, u, uArray) +
                //       (uArray[i + p + 1] - u) / (uArray[i + d + 1] - uArray[i + 1]) * Cox_deBoor(i + 1, p - 1, u, uArray);
                float a1 = u - uArray[i];
                float a2 = uArray[i + p] - uArray[i];
                if (a1 != 0 && a2 != 0)
                {
                    a1 = a1 / a2;
                    a2 = Cox_deBoor(i, p - 1, u, uArray);
                }
                else
                {
                    a1 = 0;
                    a2 = 0;
                }

                float b1 = uArray[i + p + 1] - u;
                float b2 = 0;
                if (b1 != 0)
                {
                    b2 = uArray[i + p + 1] - uArray[i + 1];
                    if (b2 == 0)
                    {
                        b1 = 1;
                    }
                    else
                    {
                        b1 = b1 / b2;
                    }
                    b2 = Cox_deBoor(i + 1, p - 1, u, uArray);
                }
                return a1 * a2 + b1 * b2;
            }
        }
    }
}