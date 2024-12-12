using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GameCore.Core;
using GameCore.Lang.Extension;

namespace GameCore.Core
{
    //用左下角坐标，作为格子坐标，（0,0）是第一个格子
    public partial class GridUtils
    {
        //和UnityEngine的Rect一样，左下角坐标是Rect的位置。位置(0,0)，大小(1,1)的右上角坐标是（1,1）
        public class GridRect
        {
            public RectInt rect;
            public Edge edgeLeft;
            public Edge edgeRight;
            public Edge edgeTop;
            public Edge edgeBottom;
            public GridRect(Vector2Int pos, Vector2Int size)
            {
                rect = new RectInt(pos, size);
                edgeLeft = new Edge(new Vector2Int(rect.xMin, rect.yMin), new Vector2Int(rect.xMin, rect.yMax));
                edgeRight = new Edge(new Vector2Int(rect.xMax, rect.yMin), new Vector2Int(rect.xMax, rect.yMax));
                edgeTop = new Edge(new Vector2Int(rect.xMin, rect.yMax), new Vector2Int(rect.xMax, rect.yMax));
                edgeBottom = new Edge(new Vector2Int(rect.xMin, rect.yMin), new Vector2Int(rect.xMax, rect.yMin));
            }

            public bool TryMerge(GridRect gr, out GridRect rt)
            {
                if (edgeLeft == gr.edgeRight)
                {
                    rt = new GridRect(gr.rect.min, new Vector2Int(rect.size.x + gr.rect.size.x, rect.size.y));
                    return true;
                }
                else if (edgeRight == gr.edgeLeft)
                {
                    rt = new GridRect(rect.min, new Vector2Int(rect.size.x + gr.rect.size.x, rect.size.y));
                    return true;
                }
                else if(edgeTop == gr.edgeBottom)
                {
                    rt = new GridRect(rect.min, new Vector2Int(rect.size.x, rect.size.y + gr.rect.size.y));
                    return true;
                }
                else if (edgeBottom == gr.edgeTop)
                {
                    rt = new GridRect(gr.rect.min, new Vector2Int(rect.size.x, rect.size.y + gr.rect.size.y));
                    return true;
                }

                rt = null;
                return false;
            }
        }
        public struct Edge
        {
            public Vector2Int startPos;
            public Vector2Int endPos;

            public Edge(Vector2Int start, Vector2Int end)
            {
                //将x值小的放在start，如果x值一样，将y值小的放在start
                if (start.x < end.x)
                {
                    startPos = start;
                    endPos = end;
                }
                else if(start.x > end.x)
                {
                    startPos = end;
                    endPos = start;
                }
                else if (start.y < end.y)
                {
                    startPos = start;
                    endPos = end;
                }
                else if(start.y > end.y)
                {
                    startPos = end;
                    endPos = start;
                }
                else
                {
                    startPos = start;
                    endPos = end;
                }
            }

            public static bool operator ==(Edge a, Edge b)
            {
                return a.startPos == b.startPos && a.endPos == b.endPos;
            }

            public static bool operator !=(Edge a, Edge b)
            {
                return !(a == b);
            }

            public bool Equals(Edge other)
            {
                return startPos.Equals(other.startPos) && endPos.Equals(other.endPos);
            }

            public override bool Equals(object obj)
            {
                return obj is Edge other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(startPos, endPos);
            }
        }
        public static List<RectInt> MergeGrids(List<Vector2Int> cells)
        {
            List<GridRect> rects = new List<GridRect>();
            foreach (var cell in cells)
            {
                rects.Add(new GridRect(cell, Vector2Int.one));
            }

            bool hasDoMerge = true;
            while (hasDoMerge)
            {
                hasDoMerge = false;
                for (int i = 0; i < rects.Count - 1; i++)
                {
                    for (int j = 1; j < rects.Count; j++)
                    {
                        if (rects[i].TryMerge(rects[j], out var gr))
                        {
                            hasDoMerge = true;
                            rects[i] = gr;
                            rects.FastRemove(j);
                            break;
                        }
                    }
                }
            }

            var rt = new List<RectInt>();
            foreach (var gridRect in rects)
            {
                rt.Add(gridRect.rect);
            }

            return rt;
        }
    }
}
