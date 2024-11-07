using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kernel.Unity
{
    public class BuilderWindow : EditorWindowBase
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
        }

        protected override void OnEnable()
        {
            CollectBuilders();
            base.OnEnable();
        }

        protected override void InitEditDataFields()
        {
            base.InitEditDataFields();
            foreach (var builder in builders)
            {
                builder.InitEditDataFields(this);
            }
        }

        public void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var builder in builders)
            {
                GUILayout.BeginHorizontal("box");
                {
                    //EditorGUILayout.ObjectField(builder, typeof(BuilderBase), false);
                    GUILayout.Label(builder.Desc);
                    if (GUILayout.Button("✎", GUILayout.Width(30)))
                    {
                        GetWindow<BuilderEditorWnd>().Init(builder);
                    }

                    if (GUILayout.Button("Build", GUILayout.Width(100)))
                    {
                        try
                        {
                            builder.Build();
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

            builders.Clear();
            List<Type> list = new();
            foreach(var t in Assembly.GetAssembly(typeof(BuilderWindow)).DefinedTypes)
            {
                if(t.IsSubclassOf(typeof(BuilderBase)))
                {
                    list.Add(t);
                    var builder = Activator.CreateInstance(t, this) as BuilderBase;
                    builders.Add(builder);
                }
            }
            types = list.ToArray();
        }
    }
}