using System;
using System.IO;

namespace GameCore.Core
{
    public class BitUtils
    {
        public static bool Contains(int n, int place)
        {
            return (n & (1 << place)) != 0;
        }

        public static void Add(ref int n, int place)
        {
            n |= (1 << place);
        }

        public static void Del(ref int n, int place)
        {
            n &= ~(1 << place);
        }

        public static int GetNumberOf1(int n)
        {
            int c = 0;
            while (n != 0)
            {
                n = n & (n - 1);
                c++;
            }
            return c;
        }
    }
}