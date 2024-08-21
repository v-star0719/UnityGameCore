using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Common
{
//边是要有方向的。如果边和多边形相交，交点到外面的点的方向，分离的方向。
    public class PolygonEdge
    {
        public int id; //边的编号，用顶点组合。比如12是1->2的边
        public PolygonVertex from;
        public PolygonVertex to;

        public List<PolygonVertex> intersects = new List<PolygonVertex>(); //相交点
        public int intersectsNum; //交点数量
        public bool hasChecked; //已经检测过是否相交的边不处理
        public float? intersectDepthValue;
        public Vector2 intersectDepth;

        public void ClearIntersectData()
        {
            this.intersectsNum = 0;
            this.hasChecked = false;
            this.intersectDepth = Vector2.zero;
            this.intersectDepthValue = null;
        }

        public void ReverseCopyIntersectsData(List<PolygonVertex> src, int num)
        {
            this.hasChecked = true;
            this.intersectsNum = num;
            var srcIndex = num;
            var d = Vector3.Distance(from.pos, to.pos);
            for (int i = 0; i < num; i++)
            {
                this.intersects[i].pos = src[srcIndex].pos;
                this.intersects[i].intersectDepthValue = d - src[srcIndex].intersectDepthValue;
                srcIndex = srcIndex - 1;
            }

            CalculateInnerIntersectDepthData();
        }

        public void CalculateInnerIntersectDepthData()
        {
            if (intersectDepthValue != null)
            {
                return;
            }

            if (this.from.isOut && !this.to.isOut)
            {
                //凹多边形会出现一个点在里面，一个点在外面，但是还是没有交点。因为点在里面外面的判断已经不对了。
                if (this.intersectsNum > 0)
                {
                    this.intersectDepth = this.intersects[1].pos - this.from.pos;
                    this.intersectDepthValue = this.intersects[1].intersectDepthValue;
                    return;
                }
                else
                {
                    Debug.LogError("error, there is no intersect for( polygon");
                }
            }

            this.intersectDepthValue = 0;
            this.intersectDepth = Vector2.zero;
        }

        public Vector2 GetInnerIntersectDepthVec()
        {
            this.CalculateInnerIntersectDepthData();
            return intersectDepth;
        }

        public float GetIntersectDepthValue()
        {
            CalculateInnerIntersectDepthData();
            return this.intersectDepthValue!.Value;
        }
    }
}