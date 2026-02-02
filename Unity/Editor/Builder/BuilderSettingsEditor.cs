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
            this.GUIPropertyFieldWithOverride_Popup("builder", GetBuilders(), () => settings.Builder);
            this.GUIPropertyFieldWithOverride_String("channel", () => settings.Channel);
            this.GUIPropertyFieldWithOverride_String("assetBundlePackage", () => settings.AssetBundlePackage);
            this.GUIPropertyFieldWithOverride_Bool("isRelease", () => settings.IsRelease);
            this.GUIPropertyFieldWithOverride_Bool("isDevelopment", () => settings.IsDevelopment);
            this.GUIPropertyFieldWithOverride_String("scriptingDefineSymbols", () => settings.ScriptingDefineSymbols);
            this.GUIPropertyFieldWithOverride_String("extraScriptingDefineSymbols", () => settings.ExtraScriptingDefineSymbols);

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