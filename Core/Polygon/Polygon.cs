using System;
using System.Collections.Generic;
using Kernel.Edit;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace GameCore.Common
{

    public class PolygonVertex
    {
        public Vector2 pos;
        public bool isOut; //内部用相机检测用
        public float intersectDepthValue;

        public PolygonVertex(Vector2 pos)
        {
            this.pos = pos;
        }
    }

    public class Polygon
    {
        private const int MaxVertexCount = 1000;
        public List<PolygonVertex> vertices = new List<PolygonVertex>(); //要求逆时针绕序
        public List<PolygonVertex> orgVertices = new List<PolygonVertex>();
        public Dictionary<int, PolygonEdge> edges = new Dictionary<int, PolygonEdge>();
        public Vector2 center;
        public int verticesCount;

        public void Init(List<Vector2> points, Vector2 center)
        {
            this.vertices.Clear();
            this.center = center;
            edges.Clear();
            orgVertices.Clear();

            this.verticesCount = points.Count;
            for (int i = 0; i < verticesCount; i++)
            {
                vertices.Add(new PolygonVertex(points[i]));
            }

            var from = verticesCount - 1;
            for (int i = 0; i < verticesCount; i++)
            {
                var e = new PolygonEdge();
                e.from = vertices[from];
                e.to = vertices[i];
                e.id = GetEdgeId(from, i);
                this.edges[e.id] = e;

                e = new PolygonEdge();
                e.from = vertices[i];
                e.to = vertices[from];
                e.id = GetEdgeId(i, from);
                this.edges[e.id] = e;
                from = i;
            }
        }

        //两个多边形相交
        public void PolygonIntersect(Polygon polygon)
        {
            var vertices = polygon.vertices;
            var edges = polygon.edges;

            if (vertices.Count > MaxVertexCount)
            {
                //边的id是用起点顶点索引 * 1000 + 终点索引计算的
                Debug.LogError($"number of points can't more than {MaxVertexCount}");
                return;
            }

            foreach (var v in edges.Values)
            {
                v.ClearIntersectData();
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                if (IsPointInside(vertices[i].pos))
                {
                    vertices[i].isOut = false;
                }
                else
                {
                    vertices[i].isOut = true;
                    PolygonIntersect_Helper(polygon, i, polygon.GetVertexIndexCircularly(i - 1)); //计算交点
                    PolygonIntersect_Helper(polygon, i, polygon.GetVertexIndexCircularly(i + 1)); //计算交点
                }
            }
        }

        private void PolygonIntersect_Helper(Polygon polygon, int fromVertexIndex, int toVertexIndex)
        {
            var edges = polygon.edges;
            //Debug.LogError("check "..fromPt.."//>"..toPt)

            //先看反向的是否已经存在
            var edge = edges[GetEdgeId(fromVertexIndex, toVertexIndex)];
            var reverseEdge = edges[GetEdgeId(toVertexIndex, fromVertexIndex)];
            if (reverseEdge.hasChecked)
            {
                edge.ReverseCopyIntersectsData(reverseEdge.intersects, reverseEdge.intersectsNum);
                //Debug.LogError("    "..toPt.."//>"..fromPt.."isExist")
                return;
            }

            //计算交叉点
            EdgeIntersect(edge);
            //Debug.LogError("    ".."intersects", edge.intersectsNum, edge.intersects[1].x, edge.intersects[1].y)
        }

        //计算内部一个多边形和自己相交时的分离方向
        //取相交最多的那个边的分离方向返回
        public Vector2 CalculateInnerPolygonSeparateDir(Polygon polygon)
        {
            this.PolygonIntersect(polygon);
            var max = 0f;
            PolygonEdge maxEdge = null;
            //同一个顶点取最小，多个顶点取最大
            for (int i = 0; i < polygon.verticesCount; i++)
            {
                var edge1 = polygon.edges[GetEdgeId(i, polygon.GetVertexIndexCircularly(i - 1))];
                var edge2 = polygon.edges[GetEdgeId(i, polygon.GetVertexIndexCircularly(i + 1))];
                var d1 = edge1.GetIntersectDepthValue();
                var d2 = edge2.GetIntersectDepthValue();

                //将较小值放到d1和edge1上，然后将最大值放到max上
                if (d1 > 0 || d2 > 0)
                {
                    if ((d1 > 0 && d2 > 0 && d1 > d2) || d1 == 0)
                    {
                        d1 = d2;
                        edge1 = edge2;
                    }

                    if (max < d1)
                    {
                        max = d1;
                        maxEdge = edge1;
                    }
                }
            }

            return maxEdge?.GetInnerIntersectDepthVec() ?? Vector2.zero;
        }

        //内部一个多边形和自己相交时的深度
        //-@param polygon Polygon
        public float CalculateInnerPolygonIntersectDepth(Polygon polygon)
        {
            this.PolygonIntersect(polygon);
            var max = 0f;
            //同一个顶点取最小，多个顶点取最大
            for (int i = 0; 0 < polygon.verticesCount; i++)
            {
                var d1 = polygon.edges[GetEdgeId(i, polygon.GetVertexIndexCircularly(i - 1))].GetIntersectDepthValue();
                var d2 = polygon.edges[GetEdgeId(i, polygon.GetVertexIndexCircularly(i + 1))].GetIntersectDepthValue();
                if (d1 > 0 || d2 > 0)
                {
                    if (d1 > 0 && d2 > 0)
                    {
                        d1 = d1 > d2 ? d2 : d1;
                    }
                    else if (d2 > 0)
                    {
                        d1 = d2;
                    }

                    if (max < d1)
                    {
                        max = d1;
                    }
                }
            }

            return max;
        }

        //-@param edge PolygonEdge
        public void EdgeIntersect(PolygonEdge edge)
        {
            var from = edge.from;
            var to = edge.to;
            var n = SegmentIntersect(from.pos, to.pos, edge.intersects);
            edge.intersectsNum = n;
        }

        //一条和直线和凸多边形相交，最多2个交点
        //rt PolygonVertex[] 用来返回结果，降低GC，要求大小超过2
        //return number 交点数量
        public int SegmentIntersect(Vector2 p1, Vector2 p2, List<PolygonVertex> rt)
        {
            var points = vertices;
            var from = points[verticesCount - 1];
            for (int i = 0; i < verticesCount; i++)
            {
                var to = points[i];
                if (Kernel.Core.MathUtils.SegmentIntersect(p1, p2, from.pos, to.pos, out var intersectionPoint))
                {
                    var p = new PolygonVertex(intersectionPoint);
                    p.intersectDepthValue = Vector2.Distance(intersectionPoint, p1);
                    rt.Add(p);
                }

                from = to;

                if (rt.Count >= 2)
                {
                    break; //找到两个交点后就中断
                }
            }

            if (rt.Count >= 2)
            {
                //把距离原点最近的放到前面
                if (rt[0].intersectDepthValue > rt[1].intersectDepthValue)
                {
                    (rt[0], rt[1]) = (rt[1], rt[0]);
                }
            }

            return rt.Count;
        }

        //要求逆时针绕序
        //向量a叉向量b，如果结果大于0，则位于向量ai->bi的逆时针方向
        public bool IsPointInside(Vector2 pos)
        {
            //由于是逆时针绕序
            //如果向量ai->xi始终位于向量ai->bi的逆时针方向，则位于里面
            var points = this.vertices;
            var n = points.Count;
            var p0 = points[n - 1];
            var c = 0f;
            for (int i = 0; i < n; i++)
            {
                var p1 = points[i];
                if (Kernel.Core.MathUtils.Cross(pos - p0.pos, p1.pos - p0.pos) >= -0.0001)
                {
                    //浮点数可能有误差，多往里面一点
                    return false;
                }

                p0 = p1;
            }

            return true;
        }

        public void Scale(float s)
        {
            CacheOldVertices();
            for (int i = 0; i < verticesCount; i++)
            {
                this.vertices[i].pos = (this.orgVertices[i].pos - this.center) * s + this.center;
            }
        }

        //垂直方向压缩不记得有啥用了
        public void Expand(float d, float verticalCompression = 0)
        {
            this.CacheOldVertices();
            var vertices = this.vertices;
            var orgVertices = this.orgVertices;
            for (int i = 0; i < this.verticesCount; i++)
            {
                //衔接的两条边的合方向作为扩展方向
                var from = orgVertices[this.GetVertexIndexCircularly(i - 1)];
                var center = orgVertices[i].pos;
                var to = orgVertices[this.GetVertexIndexCircularly(i + 1)];
                var dir = (center - from.pos) + (center - to.pos);

                var dt = d;
                if (verticalCompression > 0)
                {
                    var dirToPolygonCenter = center - this.center;
                    var dot = Mathf.Abs(Vector2.Dot(dirToPolygonCenter, Vector2.up));
                    if (dot > 0.866)
                    {
                        dt = dt * (1 - verticalCompression);
                    }
                }

                vertices[i].pos = dir * dt + orgVertices[i].pos;
            }
        }

        public void CacheOldVertices()
        {
            if (this.orgVertices == null)
            {
                this.orgVertices = new List<PolygonVertex>();
                for (int i = 0; i < this.verticesCount; i++)
                {
                    this.orgVertices[i] = new PolygonVertex(vertices[i].pos);
                }
            }
        }

        public int GetVertexIndexCircularly(int index)
        {
            if (index < 0)
            {
                return this.verticesCount - 1;
            }
            else if (index >= this.verticesCount)
            {
                return 0;
            }

            return index;
        }

        //用顶点索引生成边的id，方便快速查找边
        public int GetEdgeId(int from, int to)
        {
            return from * MaxVertexCount + to;
        }

        public void DrawGizmos(float vertexRadius)
        {
            if (vertices.Count == 0)
            {
                return;
            }

            Vector2 from = vertices[verticesCount - 1].pos;
            for (int i = 0; i < verticesCount; i++)
            {
                Gizmos.DrawSphere(vertices[i].pos, vertexRadius);
                Gizmos.DrawLine(from, vertices[i].pos);
                from = vertices[i].pos;
            }

            foreach (var edge in edges.Values)
            {
                if (edge.intersects.Count > 0)
                {
                    foreach (var v in edge.intersects)
                    {
                        using (GizmosUtils.GizmosColor(Color.yellow))
                        {
                            Gizmos.DrawSphere(v.pos, vertexRadius);
                        }
                    }
                }
            }
        }
    }
}