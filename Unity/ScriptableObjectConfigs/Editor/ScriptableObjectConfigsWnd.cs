using GameCore.Unity;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public class ScriptableObjectConfigsWnd : EditorWindow
    {
        private List<Type> types;

        [MenuItem("Tools/ScriptableObjectConfigsWnd")]
        public static void Open()
        {
            GetWindow<ScriptableObjectConfigsWnd>();
        }

        private void OnEnable()
        {
            types = new List<Type>();
            foreach (var t in typeof(ScriptableObjectConfigsBase<>).Assembly.GetTypes())
            {
                if(t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(ScriptableObjectConfigsBase<>))
                {
                    types.Add(t);
                }
            }
            types.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
        }

        private void OnGUI()
        {
            foreach (var t in types)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(t.Name);
                if (GUILayout.Button("Create", GUILayout.Width(120)))
                {
                    var path = EditorUtility.SaveFilePanelInProject($"Save {t.Name}", t.Name, "asset", "Save " + t.Name);
                    var obj = ScriptableObject.CreateInstance(t);
                    AssetDatabase.CreateAsset(obj, path);
                    AssetDatabase.Refresh();
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}