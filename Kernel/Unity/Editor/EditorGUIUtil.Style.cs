using UnityEngine;
using UnityEditor;

namespace Kernel.Unity
{
	public partial class EditorGUIUtil
    {
        private static GUIStyle styleBox;

        public static GUIStyle StyleBox
        {
            get
            {
                if (styleBox == null)
                {
                    styleBox = new GUIStyle(EditorStyles.helpBox);
                    styleBox.margin = new RectOffset(10, 10, 10, 10);
                }

                return styleBox;
            }
        }
    }
}