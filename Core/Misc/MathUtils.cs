using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore.Core.Misc
{
    public partial class MathUtils
    {
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
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
            angle *= Mathf.Deg2Rad;
            center.x += r * Mathf.Cos(angle);
            center.y += r * Mathf.Sin(angle);
            return center;
        }

        public static Vector2 GetPointOnRound(Vector2 center, float r, float angle)
        {
            angle *= Mathf.Deg2Rad;
            center.x += r * Mathf.Cos(angle);
            center.y += r * Mathf.Sin(angle);
            return center;
        }

        public static Vector3 GetRandomPointOnRoundXZ(Vector3 center, float r)
        {
            float angle = RandomUtils.Random.Next(0, 360) * Mathf.Deg2Rad;
            center.x += r * Mathf.Cos(angle);
            center.z += r * Mathf.Sin(angle);
            return center;
        }

        public static Vector3 GetRandomPointInRoundXZ(Vector3 center, float r)
        {
            float angle = RandomUtils.Random.Next(0, 360) * Mathf.Deg2Rad;
            r = (float)RandomUtils.Random.NextDouble() * r;
            center.x += r * Mathf.Cos(angle);
            center.z += r * Mathf.Sin(angle);
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

        public static Vector3 RotateX(Vector3 vector, float angle)
        {
            float angleRad = Mathf.Deg2Rad * angle;
            float cosTheta = Mathf.Cos(angleRad);
            float sinTheta = Mathf.Sin(angleRad);
            return new Vector3(
                vector.x,
                vector.y * cosTheta - vector.z * sinTheta,
                vector.y * sinTheta + vector.z * cosTheta
            );
        }

        public static Vector3 RotateY(Vector3 vector, float angle)
        {
            float angleRad = Mathf.Deg2Rad * angle;
            float cosTheta = Mathf.Cos(angleRad);
            float sinTheta = Mathf.Sin(angleRad);
            return new Vector3(
                vector.x * cosTheta + vector.z * sinTheta,
                vector.y,
                -vector.x * sinTheta + vector.z * cosTheta
            );
        }

        public static Vector3 RotateZ(Vector3 vector, float angle)
        {
            float angleRad = Mathf.Deg2Rad * angle;
            float cosTheta = Mathf.Cos(angleRad);
            float sinTheta = Mathf.Sin(angleRad);
            return new Vector3(
                vector.x * cosTheta - vector.y * sinTheta,
                vector.x * sinTheta + vector.y * cosTheta,
                vector.z
            );
        }

        //计算从屏幕中心发出的射线，和屏幕的交点
        public static Vector2 CalculateRayScreenIntersection(Vector2 screenSize, Vector2 dir)
        {
            var halfScreenW = screenSize.x * 0.5f;
            //水平线
            if(dir.y == 0)
            {
                return new Vector2(0, dir.x > 0 ? halfScreenW : -halfScreenW);
            }

            //非水平线
            //------
            //|\  /|
            //| \/ |
            //| /\ |
            //|/  \|
            //------ 横屏不好画
            //计算在上下两边上的交点，如果交点超出屏幕了，说明应该在左右两边相交
            //这个算法只需要计算两次除法。如果是用斜率判断要算至少2次。
            var x = 0f;
            var y = 0f;
            var halfScreenH = screenSize.y * 0.5f;
            y = dir.y > 0 ? halfScreenH : -halfScreenH;
            x = dir.x / dir.y * y;
            if(dir.x > 0 && x > halfScreenW)
            {
                x = halfScreenW;
                y = dir.y / dir.x * x;
            }
            else if(dir.x < 0 && x < -halfScreenW)
            {
                x = -halfScreenW;
                y = dir.y / dir.x * x;
            }

            return new Vector2(x, y);
        }
        
        
        ///将一个矩形分成若干格子，在这些格子中随机一个
        ///返回的格子原点是矩形中心
        public static Vector2 GetRandomCellInRect(float rectWith, float rectHeight, float cellWith, float cellHeight)
        {
            var hCount = (int)Mathf.Max(1, rectWith / cellWith);
            var vCount = (int)Mathf.Max(1, rectHeight / cellHeight);
            var r = RandomUtils.Random.Next(0, hCount * vCount);
            var y = r / hCount;
            var x = r - y * hCount;
            //认为(0,0)格子在左下角度
            var rt = new Vector2
            {
                x = (x + 0.5f) * cellWith - hCount * cellWith * 0.5f,
                y = (y + 0.5f) * cellHeight - vCount * cellHeight * 0.5f
            };
            return rt;
        }

        ///将一个矩形分成若干格子，在这些格子中随机一个
        ///返回的格子原点是矩形中心
        ///count: 小于0是一个百分比数值，-100~0对应100%~0
        public static List<Vector2> GetRandomCellsInRect(float rectWith, float rectHeight, float cellWith, float cellHeight, int count)
        {
            var hCount = (int)Mathf.Max(1, rectWith / cellWith);
            var vCount = (int)Mathf.Max(1, rectHeight / cellHeight);

            //cells记录格子索引，每随机一个，就把用掉的格子放到最后去，从前面可用的格子继续随机
            var cells = new int[hCount * vCount];
            for(int i = 0; i < cells.Length; i++)
            {
                cells[i] = i;
            }

            //count修正
            if (count < 0)
            {
                count = Mathf.CeilToInt(-count * 0.01f * cells.Length);
            }

            var rt = new List<Vector2>();
            var max = cells.Length;
            for (var i = 0; i < count && max > 0; i++, max--)
            {
                var idx = RandomUtils.Random.Next(0, max);
                var r = cells[idx];
                var y = r / hCount;
                var x = r - y * hCount;
                //认为(0,0)格子在左下角度
                var v2 = new Vector2
                {
                    x = (x + 0.5f) * cellWith - hCount * cellWith * 0.5f,
                    y = (y + 0.5f) * cellHeight - vCount * cellHeight * 0.5f
                };
                rt.Add(v2);
                (cells[max - 1], cells[idx]) = (cells[idx], cells[max - 1]);
            }
            return rt;
        }

        ///保持宽高比放在容器里。
        ///constrain：true=限制在容器内，较长的一边撑满。false=不限制在容易内，较短的一边撑满，较长的一边超出。
        public static Vector2 PutRectInRect(float containerWith, float containerHeight, float width, float height, bool constrain)
        {
            var ratio = width / height;
            var containerRatio = containerWith / containerHeight;
            if(ratio < containerRatio)
            {
                //更窄
                return constrain ? new Vector2(containerHeight * ratio, containerHeight) : new Vector2(containerWith, containerWith / ratio);
            }
            else
            {
                //更宽
                return constrain ? new Vector2(containerWith, containerWith / ratio) : new Vector2(containerHeight * ratio, containerHeight);
            }
        }

        public static Vector2 PutRectInRect(Vector2 containerSize, Vector2 size, bool constrain)
        {
            return PutRectInRect(containerSize.x, containerSize.y, size.x, size.y, constrain);
        }

        public static Vector3 GetMeetPosMoveTowardEachOther(Vector3 aPos, Vector3 bPos, float aSpeed, float bSpeed)
        {
            var f = aSpeed / (aSpeed + bSpeed);
            return Vector3.Lerp(aPos, bPos, f);
        }

        /// <summary>
        /// 计算点c关于直线的对称点
        /// </summary>
        /// <param name="p">需要计算对称点的点</param>
        /// <param name="a">直线经过的点</param>
        /// <param name="f">直线的方向向量</param>
        /// <returns>点c关于直线的对称点</returns>
        public static Vector3 GetSymmetricPoint(Vector3 p, Vector3 a, Vector3 f)
        {
            // 确保方向向量不为零
            if(f.sqrMagnitude < Mathf.Epsilon)
            {
                Debug.LogWarning("方向向量不能为零向量");
                return Vector3.zero;
            }

            // 计算向量ac
            Vector3 ac = p - a;

            // 计算投影参数t：(ac · f) / |f|²
            float t = Vector3.Dot(ac, f) / f.sqrMagnitude;

            // 计算点c在直线上的投影点p
            Vector3 pj = a + t * f;

            // 计算对称点：对称点 = 2p - c
            Vector3 symmetricPoint = 2 * pj - p;

            return symmetricPoint;
        }

        /// <summary>
        /// 在矩形范围内随机生成多个圆形位置，保证它们之间的距离不小于两倍半径
        /// 点位过多时不建议使用，比较次数太多。(n - 1) * n / 2。10=>45，20=>190, 3i=>435。这还是没有出现未命中的情况
        /// </summary>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        /// <param name="count">爆炸点数量</param>
        /// <param name="radius">每个爆炸点的半径</param>
        /// <param name="shrinkRadius">缩小半径继续尝试 <param>
        /// <returns>爆炸点位置列表</returns>
        public static void GetRandomRoundsInRect(List<Vector2> positions, float width, float height, int count, float radius, bool shrinkRadius)
        {
            // 最小距离：两个爆炸点中心的距离至少为 2 * radius
            float minDistance = radius * 2f;
            minDistance *= minDistance;

            // 最大尝试次数，避免死循环
            int maxAttempts = 3;//前面的点尝试次数少，后面的会增加
            int attempts = 0;
            float shrinkRadiusStep = radius * 0.1f;

            while(positions.Count < count)
            {
                // 在矩形范围内随机生成一个点
                float x = UnityEngine.Random.Range(-width / 2f, width / 2f);
                float y = UnityEngine.Random.Range(-height / 2f, height / 2f);

                // 检查是否与已有爆炸点重合或距离过近
                bool isValid = true;
                foreach(var pos in positions)
                {
                    var xt = x - pos.x;
                    var yt = y - pos.y;
                    if(xt * xt + yt * yt < minDistance * minDistance)
                    {
                        isValid = false;
                        break;
                    }
                }

                if(isValid)
                {
                    positions.Add(new Vector2(x, y));
                    attempts = 0;
                    maxAttempts++;
                }
                else
                {
                    attempts++;
                    if(attempts >= maxAttempts)
                    {
                        if(shrinkRadius)
                        {
                            radius -= shrinkRadiusStep;
                            minDistance = radius * 2;
                            minDistance *= minDistance;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}