using System.Collections;
using UnityEngine;

#if NGUI
namespace GameCore.Unity.NGUIEx
{
    public class TextUtilsBase
    {
        public static string[] romans = new[] { "-", "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ", "Ⅹ", "Ⅺ" };

        public static string NumberToRoman(int n)
        {
            return n < romans.Length ? romans[n] : "-";
        }

        public static string WrapColor(string str, Color clr)
        {
            return $"[{ColorUtility.ToHtmlStringRGB(clr)}]{str}[-]";
        }

        public static string WrapRed(string str)
        {
            return WrapColor(str, Color.red);
        }

        public static string WrapGreen(string str)
        {
            return WrapColor(str, Color.green);
        }
    }
}
#endif