using UnityEngine;

namespace GameCore.Unity
{
    public class TouchUtils
    {
        private static float Rad90 = Mathf.PI * 0.5f;

        //t: 0-1
        public static float EaseOut(float t)
        {
            return 1 - Mathf.Sin(t * Rad90);
        }
        
        //t: 0-1
        public static float EaseIn(float t)
        {
            return (Mathf.Sin(t * Rad90));
        }
    }
}
