using System.Collections;
using UnityEngine;

namespace GameCore.Unity
{
    public class TextUtilsBaseNgui : TextUtilsBase
    {
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