using System;
using System.Collections.Generic;
using Kernel.Core.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kernel.Core
{
    public class MathUtils
    {
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

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

            var angle = Mathf.Atan(y / x); //值是-0.5pi ~ 0.5pi
            if (x < 0)
            {
                angle += Mathf.PI;
            }
            else if (angle < 0)
            {
                angle += Mathf.PI * 2; //让返回值处于1.5PI~2pi
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

        //   p1  i      p2
        //    ------------
        //     - |     -
        //      -   -   
        //       -
        //       a
        public static float PointToSegmentDistance(Vector3 a, Vector3 segP1, Vector3 segP2, out Vector3 pos,
            out bool on)
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
            pos = segP1 + vP1P2.normalized * dP1I; //交点
            return Mathf.Sqrt(vP1A.sqrMagnitude - dP1I * dP1I);
        }


        //用三角形面积求高
        //   1    c     2
        //    -----------
        //     -      -
        // a    -   -   b
        //       -
        //       P
        public static float PointToSegmentDistance(Vector3 p, Vector3 segP1, Vector3 segP2)
        {
            var a = Vector3.Distance(p, segP1);
            if (a < 0.0001f)
            {
                return a;
            }

            var b = Vector3.Distance(p, segP2);
            if (b < 0.0001f)
            {
                return b;
            }

            var c = Vector3.Distance(segP1, segP2);
            if (c < 0.0001f)
            {
                return a;
            }

            if (a * a >= b * b + c * c)
            {
                return b;
            }

            if (b * b >= a * a + c * c)
            {
                return a;
            }

            var l = (a + b + c) * 0.5f;
            var s = Mathf.Sqrt(l * (l - a) * (l - b) * (l - c));
            return 2 * s / c;
        }

        private const float EPS = 0.0001f;

        public static int SegmentIntersectCircle(Vector2 ptStart, Vector2 ptEnd, Vector2 ptCenter, float radius,
            Vector2[] result)
        {
            var fDis = Vector2.Distance(ptStart, ptEnd);

            var d = Vector2.zero;
            d.x = (ptEnd.x - ptStart.x) / fDis;
            d.y = (ptEnd.y - ptStart.y) / fDis;

            var E = Vector2.zero;
            E.x = ptCenter.x - ptStart.x;
            E.y = ptCenter.y - ptStart.y;

            var a = E.x * d.x + E.y * d.y;
            var a2 = a * a;

            var e2 = E.x * E.x + E.y * E.y;

            var r2 = radius * radius;

            if ((r2 - e2 + a2) < 0)
            {
                return 0;
            }
            else
            {
                int n = 0;
                var f = Mathf.Sqrt(r2 - e2 + a2);
                var t = a - f;

                if ((t - 0.0) > -EPS && (t - fDis) < EPS)
                {
                    result[n++] = new Vector2(ptStart.x + t * d.x, ptStart.y + t * d.y);
                }

                t = a + f;
                if ((t - 0.0) > -EPS && (t - fDis) < EPS)
                {
                    result[n++] = new Vector2(ptStart.x + t * d.x, ptStart.y + t * d.y);
                }

                return n;
            }
        }


        //      D         C        F
        // S////o//////////////////o//////E
        //      -         |        -
        //      -         |        -
        //        -       |       -
        //                O
        public static int SegmentIntersectSphere(Vector3 ptStart, Vector3 ptEnd, Vector3 center, float radius,
            out float disance, Vector3[] inserts)
        {
            Vector3 vSO = center - ptStart;
            Vector3 vEO = center - ptEnd;
            Vector3 vSE = ptEnd - ptStart;
            float dSE = 0;
            float dSC = 0;

            var dSO = vSO.magnitude;
            var dEO = vEO.magnitude;
            if (dSO < radius || dEO < radius)
            {
                //两个点都在球内，计算到直线的距离
                dSE = vSE.magnitude;
                dSC = Vector3.Dot(vSO, vSE) / dSE;
                disance = Mathf.Sqrt(dSO * dSO - dSC * dSC);
                return 0;
            }

            //两个点都不在球内，计算到直线的距离
            vSE = ptEnd - ptStart;
            dSE = vSE.magnitude;
            dSC = Vector3.Dot(vSO, vSE) / dSE;
            disance = Mathf.Sqrt(dSO * dSO - dSC * dSC);

            //直线和圆不相交
            if (disance > radius)
            {
                return 0;
            }

            //直线和圆相交。由于两个点都不在求内，所以只需要判断两个点是否位于OC的两侧即可
            var b = Vector3.Dot(vEO, vSE) * dSC <= 0;
            if (inserts == null || !b)
            {
                return 0;
            }

            var dDC = Mathf.Sqrt(radius * radius - disance * disance);
            var dSD = dSC - dDC;

            //todo 这里应该有两个交点
            inserts[0] = dSD / dSE * vSE + ptStart;
            return 1;
        }

        public static Vector2 RotateVector(Vector2 v, float rotation)
        {
            var m11 = Mathf.Cos(rotation);
            var m12 = Mathf.Sin(rotation);
            var m21 = -m12;
            var m22 = m11;
            return new Vector2(v.x * m11 + v.y * m21, v.x * m12 + v.y * m22);
        }

        ///-@param point Vector3
        ///-@param pivot Vector3
        ///-@param angle Quaternion
        public static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }

        // 计算点到直线的距离
        public static float PointToLineDistance2(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            return PointToLineDistance1(point, linePoint1, (linePoint1 - linePoint2).normalized);
        }

        /// 计算点到直线的距离
        /// lineDirection是归一化的方向向量
        public static float PointToLineDistance1(Vector3 point, Vector3 linePoint, Vector3 lineDirection)
        {
            Vector3 pointToLinePoint = point - linePoint;
            float projectionLength = Vector3.Dot(pointToLinePoint, lineDirection);
            Vector3 projectionPoint = linePoint + projectionLength * lineDirection;
            Gizmos.DrawLine(point, projectionPoint);
            return (point - projectionPoint).magnitude;
        }

        public static bool LineIntersect(Vector2 lineA1, Vector2 lineA2, Vector2 lineB1, Vector2 lineB2, out Vector2 intersectionPoint)
        {
            GetLineParams(lineA1, lineA2, out var a1, out var b1, out var c1);
            GetLineParams(lineB1, lineB2, out var a2, out var b2, out var c2);
            var o = b1 * a2 - b2 * a1;
            if (o < 0.00001 && o > -0.00001)
            {
                intersectionPoint = Vector2.zero;
                return false; //认为不相交
            }

            o = 1 / o;

            intersectionPoint = new Vector2(
                (c2 * b1 - c1 * b2) * o,
                (c1 * a2 - c2 * a1) * o);
            return true;
        }

        //ax + by + c = 0
        public static void GetLineParams(Vector2 p1, Vector2 p2, out float a, out float b, out float c)
        {
            a = p1.y - p2.y;
            b = p2.x - p1.x;
            c = p1.y * p2.x - p1.x * p2.y;
        }

        //判断线段B的两个点是否在线段A的同一层
        public static bool SegmentSide(Vector2 sA1, Vector2 sA2, Vector2 sB1, Vector2 sB2)
        {
            var a = sA2- sA1;
            return Cross(a, sB1 - sA1) * Cross(a, sB2 - sA1) > 0;
        }

        public static bool SegmentIntersect(Vector2 sA1, Vector2 sA2, Vector2 sB1, Vector2 sB2, out Vector2 intersectionPoint)
        {
            if( SegmentSide(sA1, sA2, sB1, sB2) || SegmentSide(sB1, sB2, sA1, sA2))
            {
                intersectionPoint = Vector2.zero;
                return false; //不相交
            }
            return LineIntersect(sA1, sA2, sB1, sB2, out intersectionPoint);
        }
}
}