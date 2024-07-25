using UnityEngine;

namespace UI
{
    public class NumberUtils
    {
        public const double K = 1000;
        public const double M = 1000 * 1000;
        public const double B = 1000 * 1000 * 1000;
        public static string NumberToK(double n)
        {
            if (n < K)
            {
                return $"{n:f0}";
            }
            else if (n < M)
            {
                return $"{n / K:f2}K";
            }
            else if (n < B)
            {
                return $"{n / M:f2}M";
            }
            else //if (n < 1000 * 1000 * 1000)
            {
                return $"{n / B:f2}B"; //double最多到B了
            }
        }
    }
}
