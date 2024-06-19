﻿using System;
using UnityEngine;

namespace Kernel.Edit
{
	public partial class GUIUtil
	{
		public static void CopyToSystemClipboard(string content)
		{
			GUIUtility.systemCopyBuffer = content;
		}

		private class GUIEnabled : IDisposable
		{
			private readonly bool old;

			public GUIEnabled(bool value)
			{
				old = GUI.enabled;
				GUI.enabled = value;
			}

			public void Dispose()
			{
				GUI.enabled = old;
			}
		}

		private class GUIContentColor : IDisposable
		{
			private readonly Color old;

			public GUIContentColor(Color value)
			{
				old = GUI.contentColor;
				GUI.contentColor = value;
			}

			public void Dispose()
			{
				GUI.contentColor = old;
			}
		}

		private class GUIColor : IDisposable
		{
			private readonly Color old;

			public GUIColor(Color value)
			{
				old = GUI.color;
				GUI.color = value;
			}

			public void Dispose()
			{
				GUI.color = old;
			}
		}

		private class GUIBackgroundColor : IDisposable
		{
			private readonly Color old;

			public GUIBackgroundColor(Color value)
			{
				old = GUI.backgroundColor;
				GUI.backgroundColor = value;
			}

			public void Dispose()
			{
				GUI.backgroundColor = old;
			}
		}

		private class GUISettings : IDisposable
		{
			private readonly IDisposable[] disposables;

			public GUISettings(params IDisposable[] args)
			{
				disposables = args;
			}

			public void Dispose()
			{
				for(int i = 0; i < disposables.Length; i++)
				{
					disposables[i].Dispose();
				}
			}
		}

		private class GUILayoutVertical : IDisposable
		{
			public GUILayoutVertical(GUILayoutOption[] options)
			{
				GUILayout.BeginVertical(options);
			}

			public GUILayoutVertical(GUIStyle style, params GUILayoutOption[] options)
			{
			    GUILayout.BeginVertical(style, options);
			}

			public GUILayoutVertical(GUIStyle style)
			{
			    GUILayout.BeginVertical(style);
			}

			public void Dispose()
			{
			    GUILayout.EndVertical();
			}
		}

		private class GUILayoutHorizontal : IDisposable
		{
			public GUILayoutHorizontal()
			{
			    GUILayout.BeginHorizontal();
			}

			public GUILayoutHorizontal(GUIStyle style)
			{
			    GUILayout.BeginHorizontal(style);
			}

			public GUILayoutHorizontal(params GUILayoutOption[] options)
			{
			    GUILayout.BeginHorizontal(options);
			}

			public GUILayoutHorizontal(GUIStyle style, params GUILayoutOption[] options)
			{
			    GUILayout.BeginHorizontal(style, options);
			}

			public void Dispose()
			{
			    GUILayout.EndHorizontal();
			}
		}

		private class GUIScroll : IDisposable
		{
			public GUIScroll(ref Vector2 scroll)
			{
				scroll = GUILayout.BeginScrollView(scroll);
			}

			public GUIScroll(ref Vector2 scroll, params GUILayoutOption[] options)
			{
				scroll = GUILayout.BeginScrollView(scroll, options);
			}

			public GUIScroll(ref Vector2 scroll, GUIStyle style, params GUILayoutOption[] options)
			{
				scroll = GUILayout.BeginScrollView(scroll, style, options);
			}


			public void Dispose()
			{
			    GUILayout.EndScrollView();
			}
		}

		private class GUITextAlign : IDisposable
		{
			private readonly TextAnchor old;
			private readonly GUIStyle guiStyle;

			public GUITextAlign(GUIStyle style, TextAnchor anchor)
			{
				guiStyle = style;
				old = style.alignment;
				style.alignment = anchor;
			}

			public void Dispose()
			{
				guiStyle.alignment = old;
			}
		}
	}
}