using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    [CustomEditor(typeof(BuilderSettings))]
    public class BuilderSettingsEditorEditor : Editor
    {
        private static string[] builders;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var settings = target as BuilderSettings;

            EditorGUILayout.ObjectField(serializedObject.FindProperty("parent"));
            if (settings.CheckParentLoop())
            {
                var clr = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("parent loop reference");
                GUI.color = clr;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("desc"));
            if (settings.Parent != null)
            {
                EditorGUIUtil.DrawSeparator();
            }

            this.GUIPropertyFieldWithOverride_Popup("builder", GetBuilders(), settings.Parent, () => settings.Builder);
            this.GUIPropertyFieldWithOverride_String("channel", settings.Parent, () => settings.Channel);
            this.GUIPropertyFieldWithOverride_String("assetBundlePackage", settings.Parent, () => settings.AssetBundlePackage);
            this.GUIPropertyFieldWithOverride_Bool("isRelease", settings.Parent, () => settings.IsRelease);
            this.GUIPropertyFieldWithOverride_Bool("isDevelopment", settings.Parent, () => settings.IsDevelopment);
            this.GUIPropertyFieldWithOverride_StringMultiLine("scriptingDefineSymbols", settings.Parent, () => settings.ScriptingDefineSymbols);
            this.GUIPropertyFieldWithOverride_StringMultiLine("extraScriptingDefineSymbols", settings.Parent, () => settings.ExtraScriptingDefineSymbols);

            serializedObject.ApplyModifiedProperties();
        }

        private static string[] GetBuilders()
        {
            if (builders == null)
            {
                List<string> list = new();
                foreach (var t in Assembly.GetAssembly(typeof(BuilderWindow)).DefinedTypes)
                {
                    if (t.IsSubclassOf(typeof(BuilderBase)))
                    {
                        list.Add(t.Name);
                    }
                }

                builders = list.ToArray();
            }

            return builders;
        }
    }
}