using System.Collections;
using UnityEngine;

namespace GameCore.Unity
{
    public class TextUtilsBaseUgui : TextUtilsBase
    {
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