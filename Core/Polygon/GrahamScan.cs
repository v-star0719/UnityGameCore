using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Common
{
    //平面上一组点的凸包的
    public class GrahamScanPoint
    {
        public Vector2 pos;
        public float crossAngle = float.NaN; //以starPoint为原点，x+方向的向量的叉积

        public GrahamScanPoint(Vector2 point)
        {
            pos = point;
        }
    }

    //计算最小凸包围多边形
    public class GrahamScan
    {
        public static List<Vector2> Calculate(List<Vector2> points)
        {
            if (points.Count < 3)
            {
                return points;
            }

            var innerPoints = new List<GrahamScanPoint>();
            foreach (var point in points)
            {
                innerPoints.Add(new GrahamScanPoint(point));
            }

            //按到和起点所在x+轴的角度排列
            var rtStack = new List<GrahamScanPoint>();
            var startPoint = PickStartPoint(innerPoints);
            foreach (var p in innerPoints)
            {
                CalculateCrossAngleToStarPoint(p, startPoint); ;
            }
            //返回的结果由大到小排列，先用后面的点，从尾部删除方便
            innerPoints.Sort((a, b) => a.crossAngle.CompareTo(b.crossAngle));

            rtStack.Add(startPoint);
            rtStack.Add(innerPoints[^1]);
            rtStack.Add(innerPoints[^2]);//第三个点极坐标比前面点小，一定在内部。直接加入就行
            innerPoints.RemoveAt(innerPoints.Count - 1);
            innerPoints.RemoveAt(innerPoints.Count - 1);
            
            //最近加入的两个构成一个线段A，即将加入的点和最近加入的点构成一个线段B
            //如果线段B在线段A的左边，说明即将加入的点会完善包围多边形，把这个点加入
            //如果线段B在线段A的右边，说明即将加入的点比之前加入的点更靠外，把上次加入的点删除，新的点替代
            for (int i = innerPoints.Count - 1; i >= 0; i--)
            {
                var p = innerPoints[i];
                while (true)
                {
                    if (rtStack.Count <= 1)
                    {
                        Debug.LogError("error");
                        break;
                    }

                    var ps1 = rtStack[rtStack.Count - 1];
                    var ps0 = rtStack[rtStack.Count - 2];
                    var cross = CalculateCrossAngle(p.pos.x - ps0.pos.x, p.pos.y - ps0.pos.y,
                        ps1.pos.x - ps0.pos.x, ps1.pos.y - ps0.pos.y);
                    //Debug.LogError(ps0.sid.."//>"..p.sid, ps0.sid.."//>"..ps1.sid, cross)
                    if (cross >= 0)
                    {
                        //Debug.LogError("pop", ps1.sid, cross)
                        rtStack.RemoveAt(rtStack.Count - 1);
                    }
                    else
                    {
                        //Debug.LogError("push", p.sid)
                        rtStack.Add(p);
                        break;
                    }
                }
            }

            var rt = new List<Vector2>();
            foreach (var p in rtStack)
            {
                rt.Add(p.pos);
            }

            return rt;
        }

        //把起点挑出来。最左下角的那个
        public static GrahamScanPoint PickStartPoint(List<GrahamScanPoint> points)
        {
            GrahamScanPoint find = null;
            var findIndex = -1;
            var n = points.Count - 1;
            for (int i = 0; i < n; i++)
            {
                var p = points[i];
                if (find == null || p.pos.y < find.pos.y || (p.pos.y == find.pos.y && p.pos.x < find.pos.x))
                {
                    find = p;
                    findIndex = i;
                }
            }

            points[findIndex] = points[n];
            points.RemoveAt(n);
            return find;
        }

        public static void CalculateCrossAngleToStarPoint(GrahamScanPoint p, GrahamScanPoint startPoint)
        {
            var x = p.pos.x - startPoint.pos.x;
            var y = p.pos.y - startPoint.pos.y;
            p.crossAngle = CalculateCrossAngle(x, y, 0, 100);
        }

        //如果p1xp2 > 0，则p1在p2的顺时针方向，<0则在逆时针方向
        public static float CalculateCrossAngle(float x1, float y1, float x2, float y2)
        {
            var c = x1 * y2 - x2 * y1;
            return c / (Mathf.Sqrt(x1 * x1 + y1 * y1) * Mathf.Sqrt(x2 * x2 + y2 * y2));
        }
    }
}