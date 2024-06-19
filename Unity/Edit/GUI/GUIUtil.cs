using System;
using UnityEngine;

namespace Kernel.Edit
{
	public partial class GUIUtil
	{
#if !DISABLE_UNITY
		public static IDisposable BackgroundColor(Color color)
		{
			return new GUIBackgroundColor(color);
		}

		public static IDisposable Color(Color color)
		{
			return new GUIColor(color);
		}

		public static IDisposable ContentColor(Color color)
		{
			return new GUIContentColor(color);
		}

		public static IDisposable Enabled(bool enabled)
		{
			return new GUIEnabled(enabled);
		}
#endif

#if UNITY_EDITOR
		public static bool RightClicked(Rect rect)
		{
			return Event.current.type == UnityEngine.EventType.ContextClick && rect.Contains(Event.current.mousePosition);
		}

		public static IDisposable Settings(params IDisposable[] args)
		{
			return new GUISettings(args);
		}

	    public static bool HelpBtn()
	    {
	        return GUILayout.Button("?", GUILayout.ExpandWidth(false));
	    }

        public static bool Btn_TextLeft(string text, int width)
        {
            bool b = false;
            var align = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;

            GUILayoutOption option;
            if (width < 0) option = null;
            else if (width == 0) option = GUILayout.ExpandWidth(false);
            else option = GUILayout.Width(width);

            if (GUILayout.Button(text, option))
            {
                b = true;
            }

            GUI.skin.button.alignment = align;
            return b;
        }

        public static bool Button(string text)
        {
            return GUILayout.Button(text, GUILayout.ExpandWidth(false));
        }

        public static void Box_TextLeft(string text, params GUILayoutOption[] options)
        {
            var align = GUI.skin.box.alignment;
            GUI.skin.box.alignment = TextAnchor.MiddleLeft;
            GUILayout.Box(text, options);
            GUI.skin.box.alignment = align;
        }

		public static IDisposable LayoutHorizontal()
		{
			return new GUILayoutHorizontal();
		}

		public static IDisposable LayoutHorizontal(GUIStyle style)
		{
			return new GUILayoutHorizontal(style);
		}

		public static IDisposable LayoutHorizontal(params GUILayoutOption[] options)
		{
			return new GUILayoutHorizontal(options);
		}

		public static IDisposable LayoutHorizontal(GUIStyle style, params GUILayoutOption[] options)
		{
			return new GUILayoutHorizontal(style, options);
		}

		public static IDisposable LayoutVertical(params GUILayoutOption[] options)
		{
			return new GUILayoutVertical(options);
		}

		public static IDisposable LayoutVertical(GUIStyle style, params GUILayoutOption[] options)
		{
			return new GUILayoutVertical(style, options);
		}

		public static IDisposable LayoutVertical(GUIStyle style)
		{
			return new GUILayoutVertical(style);
		}

		public static IDisposable Scroll(ref Vector2 scroll)
		{
			return new GUIScroll(ref scroll);
		}

		public static IDisposable Scroll(ref Vector2 scroll, params GUILayoutOption[] options)
		{
			return new GUIScroll(ref scroll, options);
		}

	    public static IDisposable Scroll(ref Vector2 scroll, GUIStyle style, params GUILayoutOption[] options)
	    {
		    return new GUIScroll(ref scroll, style, options);
	    }

		public static IDisposable TextAlign(GUIStyle style, TextAnchor anchor)
		{
			return new GUITextAlign(style, anchor);
		}

		private class EnumMaskItem
		{
			public int Value;
			public string Text;
			public bool IsSelected;
		}
#endif
	}
}