using UnityEngine;
using UnityEditor;

namespace GameCore.Unity
{
	public static partial class EditorGUIUtil
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

        private static GUIStyle _separatorStyle;
        private static GUIStyle SeparatorStyle
        {
            get
            {
                if(_separatorStyle == null)
                {
                    _separatorStyle = new GUIStyle();
                    _separatorStyle.normal.background = EditorGUIUtility.whiteTexture;
                    _separatorStyle.stretchWidth = true;
                }
                return _separatorStyle;
            }
        }
    }
}