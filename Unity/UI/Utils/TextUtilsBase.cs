using System.Collections;
using UnityEngine;

namespace GameCore.Unity
{
    public class TextUtilsBase
    {
        public static string[] romans = new[] { "-", "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ", "Ⅹ", "Ⅺ" };

        public static string NumberToRoman(int n)
        {
            return n < romans.Length ? romans[n] : "-";
        }
    }
}