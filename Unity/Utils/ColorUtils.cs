using UnityEngine;

namespace Kernel.Unity
{
    public static class ColorUtils
    {
        public static Color ChangeBrightness(Color clr, float brightness)
        {
            Color.RGBToHSV(clr, out float h, out float s, out float v);
            v = Mathf.Clamp01(v * brightness);
            return Color.HSVToRGB(h, s, v);
        }
    }
}
