using System;
using UnityEngine;

namespace GameCore.Core
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
    }
}
