using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kernel.Unity
{
    public class BuilderWindow : EditorWindow
    {
        private List<BuilderBase> builders = new List<BuilderBase>();
        private Vector2 scrollPos;
        private Type[] types;

        [MenuItem("Tools/Builder")]
        public static void Open()
        {
            GetWindow<BuilderWindow>();
        }

        public static void BuildAny(string builderName)
        {
            var t = Assembly.GetAssembly(typeof(BuilderWindow)).GetType(builderName);
            if (t == null)
            {
                Debug.LogError($"{builderName} is not found");
                return;
            }
            (Activator.CreateInstance(t) as BuilderBase)?.Build();
        }

        public void Awake()
        {
            CollectBuilders();
        }

        public void OnEnable()
        {
            List<Type> list = new();
            foreach (var typeInfo in Assembly.GetAssembly(typeof(BuilderWindow)).DefinedTypes)
            {
                if (typeInfo.IsSubclassOf(typeof(BuilderBase)))
                {
                    list.Add(typeInfo);
                }
            }
            types = list.ToArray();
        }

        public void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var builder in types)
            {
                GUILayout.BeginHorizontal();
                {
                    //EditorGUILayout.ObjectField(builder, typeof(BuilderBase), false);
                    GUILayout.Label(builder.Name);
                    if (GUILayout.Button("Build"))
                    {
                        try
                        {
                            var b = Activator.CreateInstance(builder) as BuilderBase;
                            b?.Build();
                        }
                        catch(Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            //if(GUILayout.Button("Refresh"))
            //{
            //    CollectBuilders();
            //}

            //if(GUILayout.Button("Create"))
            //{
            //    var path = EditorUtility.SaveFilePanelInProject("", "12", "asset", "");
            //    var obj = CreateInstance(types[0]);
            //    AssetDatabase.CreateAsset(obj, path);
            //}
        }

        private void CollectBuilders()
        {
            //builders.Clear();
            //foreach(var guid in AssetDatabase.FindAssets("t:BuilderBase", null))
            //{
            //    builders.Add(AssetDatabase.LoadAssetAtPath<BuilderBase>(AssetDatabase.GUIDToAssetPath(guid)));
            //}
        }
    }
}