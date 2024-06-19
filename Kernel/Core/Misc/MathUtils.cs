using System.Collections.Generic;
using Kernel.Core.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kernel.Core
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
            var _t = 1 - t;
            var _t2 = _t * _t;
            var t2 = t * t;
            return _t2 * _t * p0 + 3 * t * _t2 * p1 + 3 * t2 * _t * p2 + t2 * t * p3;
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

            //两两差值
            //var count = points.Length;
            //var tpoints = new Vector3[count];
            //for (int i = 0; i < count; i++)
            //{
            //    tpoints[i] = points[i];
            //}
            //for (int i = 0; i <= count - 2; i++)
            //{
            //    for (int j = 0; j < count - i - 1; j++)
            //    {
            //        tpoints[j] = Vector3.Lerp(tpoints[j], tpoints[j + 1], f);
            //    }
            //}

            //return tpoints[0];
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

        //相对于x+轴的角度
        //Mathf.Atan2返回的值就是-pi~pi，这里仅供学习参考，直接使用Mathf.Atan2即可
        public static float AngleToXAxis(float x, float y)
        {
            if (Mathf.Abs(x) < 0.0001f)
            {
                return y > 0 ? Mathf.PI * 0.5f : Mathf.PI * 1.5f;
            }

            var angle = Mathf.Atan(y / x);//值是-0.5pi ~ 0.5pi
            if (x < 0)
            {
                angle += Mathf.PI;
            }
            else if (angle < 0)
            {
                angle += Mathf.PI * 2;//让返回值处于1.5PI~2pi
            }
            return angle;
        }

        //绕Y轴旋转时，从方向from到方向to的角度
        public static float GetRotateAngle(Vector3 from, Vector3 to)
        {
            float angle = Vector3.Angle(from, to);
            Vector3 norm = new Vector3(-from.z, 0f, from.x);
            if (Vector3.Dot(to, norm) < 0f)
            {
                angle = 360 - angle;
            }
            return angle;
        }

        public static Vector3 RotateByY(Vector3 origin, float rotationAngle)
        {
            float angle = rotationAngle * Mathf.PI;
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);
            float x = origin.x * cos - origin.z * sin;
            float z = origin.z * cos + origin.x * sin;

            origin.x = x;
            origin.z = z;
            return origin;
        }

        public static float Distance(float x, float y, float z)
        {
            return Mathf.Sqrt(x * x + y * y + z * z);
        }

        //线段AB包含线段CD，在CD之外的部分随机，也就是AC和DB。AB和CD中心一样。（不一样第一次随机就不是0.5了）
        //A-----C-----D-----B
        public static float RandomInTwoSide(float a, float b, float c, float d)
        {
            return Random.value < 0.5f ? Random.Range(a, c) : Random.Range(d, b);
        }

        public static Vector3 GetPointOnRound(Vector3 center, float r, float angle)
        {
            center.x += r * Mathf.Cos(angle * Mathf.Deg2Rad);
            center.y += r * Mathf.Sin(angle * Mathf.Deg2Rad);
            return center;
        }

        public static Vector2 GetPointOnRound(Vector2 center, float r, float angle)
        {
            center.x += r * Mathf.Cos(angle * Mathf.Deg2Rad);
            center.y += r * Mathf.Sin(angle * Mathf.Deg2Rad);
            return center;
        }

        public static Vector3 GetRandomPointInRoundXZ(Vector3 center, float r)
        {
            float angle = RandomUtils.Random.Next(0, 360);
            r = (float)RandomUtils.Random.NextDouble() * r;
            center.x += r * Mathf.Cos(angle * Mathf.Deg2Rad);
            center.z += r * Mathf.Sin(angle * Mathf.Deg2Rad);
            return center;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="r"></param>
        /// <param name="angleXY">xy平面上和x+轴的夹角</param>
        /// <param name="angleZ">确定xy平面上和x+轴的夹角后，和z+轴的夹角</param>
        /// <returns></returns>
        public static Vector3 GetPointOnSphere(Vector3 center, float r, float angleXY, float angleZ)
        {
            angleXY *= Mathf.Deg2Rad; //XY平面上，和X+轴的夹角
            angleZ *= Mathf.Deg2Rad;
            var cosZ = Mathf.Cos(angleZ);
            var sinZ = Mathf.Sin(angleZ);
            var cosYZ = Mathf.Cos(angleXY);
            var sinYZ = Mathf.Sin(angleXY);
            center.x += r * sinZ * cosYZ;
            center.y += r * sinZ * sinYZ;
            center.z += r * cosZ;
            return center;
        }

        public static Vector3 GetRandomEulerAngle()
        {
            return new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        }

        public static void MinFirst(ref int a, ref int b)
        {
            if (a > b)
            {
                a = a ^ b;
                b = a ^ b;
                a = a ^ b;
            }
        }

        //   p1  i      p2
        //    ------------
        //     - |     -
        //      -   -   
        //       -
        //       a
        public static float PointToSegmentDistance(Vector3 a, Vector3 segP1, Vector3 segP2, out Vector3 pos, out bool on)
        {
            var dP1P2 = Vector3.Distance(segP1, segP2);
            if (dP1P2 < 0.0001f)
            {
                //线段两个点离太近了
                pos = segP1;
                var rt = Vector3.Distance(segP1, a);
                on = rt <= 0.001f;
                return rt;
            }

            var vP1P2 = segP2 - segP1;
            var vP1A = a - segP1;

            var dP1I = Vector3.Dot(vP1P2, vP1A) / dP1P2;
            on = dP1I >= 0 && dP1I <= dP1P2;
            pos = segP1 + vP1P2.normalized * dP1I;//交点
            return Mathf.Sqrt(vP1A.sqrMagnitude - dP1I * dP1I);
        }
    }
}