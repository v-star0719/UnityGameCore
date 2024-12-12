using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public class BuilderEditorWnd : EditorWindow
    {
        private BuilderBase builder;
        public void Init(BuilderBase bld)
        {
            builder = bld;
        }

        public void OnGUI()
        {
            builder.OnGUI();
        }
    }
}