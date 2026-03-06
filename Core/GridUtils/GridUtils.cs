using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Core.Grid
{
    public partial class GridUtils
    {
        //遍历包围着区块的一圈一圈格子
        //----------------
        //|   |      |   |
        //|   --------   |
        //|   |      |   |
        //|   |      |   |
        //|   --------   |
        //|   |      |   |
        //----------------
        //回调如果返回true，终止遍历
        //size = (0, 0), radius = 1时，只会遍历pos那一个格子。
        public static void IterateRange(Vector2Int pos, Vector2Int size, int radius, Func<int, int, bool> cb)
        {
            if(radius <= 0)
            {
                return;
            }

            if (size.x == 0 && size.y == 0 && radius == 1)
            {
                cb(pos.x, pos.y);
                return;
            }

            var minX = pos.x;
            var maxX = pos.x + size.x - 1;
            var minY = pos.y;
            var maxY = pos.y + size.y - 1;

            var rangeMinY = minY - radius;
            var rangeMaxY = maxY + radius;

            //左面和右面一大片
            for(int i = 1; i <= radius; i++)
            {
                for(int j = rangeMinY; j <= rangeMaxY; j++)
                {
                    if (cb(minX - i, j))
                    {
                        return;
                    }

                    if (cb(maxX + i, j))
                    {
                        return;
                    }

                }
            }

            //上面和下面一小片
            for(int i = minX; i <= maxX; i++)
            {
                for(int j = 1; j <= radius; j++)
                {
                    if (cb(i, maxY + j))
                    {
                        return;
                    }

                    if (cb(i, minY - j))
                    {
                        return;
                    }
                }
            }
        }

        //遍历边上的格子
        //    --------
        //    |      |
        //----------------
        //|   |      |   |
        //|   |      |   |
        //----------------
        //    |      |
        //    --------
        //回调如果返回true，终止遍历
        public static void IterateAdj(Vector2Int pos, Vector2Int size, int radius, Func<int, int, bool> cb)
        {
            if(radius <= 0)
            {
                return;
            }

            var minX = pos.x;
            var maxX = pos.x + size.x - 1;
            var minY = pos.y;
            var maxY = pos.y + size.y - 1;

            //横向
            for(int i = minX; i <= maxX; i++)
            {
                for (int j = 1; j <= radius; j++)
                {
                    if (cb(i, maxY + j))
                    {
                        return;
                    }

                    if (cb(i, minY - j))
                    {
                        return;
                    }
                }
            }

            //纵向
            for(int i = minY; i <= maxY; i++)
            {
                for(int j = 1; j <= radius; j++)
                {
                    if (cb(minX - j, i))
                    {
                        return;
                    }
                    if (cb(maxX + j, i))
                    {
                        return;
                    }
                }
            }
        }

        //Rect的pos在左下角
        public static bool IsRectAInRectB(Vector2Int aPos, Vector2Int aSize, Vector2Int bPos, Vector2Int bSize)
        {
            return aPos.x >= bPos.x &&
                   aPos.y >= bPos.y &&
                   aPos.x + aSize.x <= bPos.x + bSize.x &&
                   aPos.y + aSize.y <= bPos.y + bSize.y;
        }

        // 4方向偏移量（上下左右）
        private static readonly Vector2Int[] _dir4 = new[]
        {
            new Vector2Int(-1, 0), // 上
            new Vector2Int(1, 0),  // 下
            new Vector2Int(0, -1), // 左
            new Vector2Int(0, 1)   // 右
        };
        // 8方向偏移量（上下左右+左上+右上+左下+右下）
        private static readonly Vector2Int[] _dir8 = new[]
        {
            new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1),
            new Vector2Int(-1, -1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(1, 1)
        };
        /// <summary>
        /// 一圈一圈遍历格子
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="callback"> 返回true停止 </param>
        public static void TraverseCircleByCircle(Vector2Int pos, bool[,] visited, Func<int, int, bool> callback, bool is8Dir = true)
        {
            Array.Clear(visited, 0, visited.Length);

            var width = visited.GetLength(0);
            var height = visited.GetLength(1);
            var queue = new Queue<Vector2Int>();
            queue.Enqueue(pos);
            visited[pos.x, pos.y] = true;

            var dirs = is8Dir ? _dir8 : _dir4;

            while(queue.Count > 0)
            {
                // 关键：获取当前圈的格子数量（BFS层序核心，保证一圈一圈处理）
                int currentCircleCount = queue.Count;

                // 遍历当前圈的所有格子
                for(int i = 0; i < currentCircleCount; i++)
                {
                    var curPos = queue.Dequeue();

                    // 校验当前格子是否为目标：符合则立即返回（找到第一个目标，终止遍历）
                    if(callback(curPos.x, curPos.y))
                    {
                        return;
                    }

                    // 遍历当前格子的所有相邻方向，生成下一圈的格子
                    foreach(var dir in dirs)
                    {
                        int newX = curPos.x + dir.x;
                        int newY = curPos.y + dir.y;

                        // 新格子需满足：边界内 + 未被访问过
                        if(newX >= 0 && newX < width && newY >= 0 && newY < height && !visited[newX, newY])
                        {
                            visited[newX, newY] = true;
                            queue.Enqueue(new(newX, newY));
                        }
                    }
                }
                // 当前圈遍历完成，队列中剩余的是下一圈的所有格子，进入下一次循环
            }
        }
    }
}
