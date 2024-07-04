using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Kernel.Unity
{
    [CustomEditor(typeof(ScriptingDefineSymbolSetting))]
    public class ScriptingDefineSymbolDataEditor : Editor
    {
        private ScriptingDefineSymbolSetting setting;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            setting = target as ScriptingDefineSymbolSetting;

            for (var i = 0; i < setting.symbols.Count; i++)
            {
                var symbol = setting.symbols[i];
                using (new GUILayout.HorizontalScope())
                {
                    symbol.value = GUILayout.TextField(symbol.value);
                    symbol.enabled = GUILayout.Toggle(symbol.enabled, GUIContent.none, GUILayout.ExpandWidth(false));
                }
            }

            if(GUILayout.Button("Load From Player Settings"))
            {
                LoadFromPlayerSettings();
            }

            if(GUILayout.Button("Apply"))
            {
                Apply();
            }
        }

        private void Apply()
        {
            List<string> sl = new List<string>();
            foreach (var t in setting.symbols)
            {
                if (t.enabled)
                {
                    sl.Add(t.value);
                }
            }
            PlayerSettings.SetScriptingDefineSymbols(CurrentNamedBuildTarget, sl.ToArray());
        }

        private void LoadFromPlayerSettings()
        {
            var t = CurrentNamedBuildTarget;
            PlayerSettings.GetScriptingDefineSymbols(t, out var defines);
            foreach (var s in defines)
            {
                setting.symbols.Add(new ScriptingDefineSymbolItem()
                {
                    value = s,
                    enabled = true
                });
            }
        }

        public static NamedBuildTarget CurrentNamedBuildTarget
        {
            get
            {
                BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
                return namedBuildTarget;
            }
        }
    }
}