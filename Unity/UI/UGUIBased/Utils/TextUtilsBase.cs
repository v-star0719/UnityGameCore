using UnityEngine;

namespace GameCore.Unity.UI.UGUIEx
{
    public class TextUtilsBase
    {
        //
        public static string[] romans = new[] { "-", "¢ñ", "¢ò", "¢ó", "¢ô", "¢õ", "¢ö", "¢÷", "¢ø", "¢ù", "¢ú", "¢û", "¢ü" };

        public static string NumberToRoman(int n)
        {
            return n < romans.Length ? romans[n] : "-";
        }

        public static string WrapColor(string str, Color clr)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(clr)}>{str}</color>";
        }

        public static string WrapGreen(string str)
        {
            return WrapColor(str, Color.green);
        }

        public static string WrapRed(string str)
        {
            return WrapColor(str, Color.red);
        }
    }
}