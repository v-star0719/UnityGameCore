using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using GameCore.Edit;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public class BuilderWindow : EditorWindowBase
    {
        private List<BuilderSettings> settings = new List<BuilderSettings>();
        private Vector2 scrollPos;
        private Type[] types;
        private List<Type> builderSettingTypes = new();
        private string[] builderSettingNames;
        private int builderSettingIndex;
        private Dictionary<int, string> outputDirs = new();

        [MenuItem("Tools/Builder")]
        public static void Open()
        {
            GetWindow<BuilderWindow>();
        }

        protected override void OnEnable()
        {
            CollectSettings();
            base.OnEnable();
        }

        public void OnGUI()
        {
            using (GUIUtil.LayoutHorizontal())
            {
                builderSettingIndex = EditorGUILayout.Popup(builderSettingIndex, builderSettingNames, GUILayout.ExpandWidth(false));
                if(GUILayout.Button("Create", GUILayout.Width(120)))
                {
                    CreateSetting();
                }
                if(GUILayout.Button("Refresh", GUILayout.Width(120)))
                {
                    CollectSettings();
                }
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach(var bs in settings)
            {
                GUILayout.BeginHorizontal("box");
                {
                    EditorGUILayout.ObjectField(bs, bs.GetType(), false, GUILayout.Width(350));
                    var dir = GetOutputDir(bs);
                    EditorStyles.label.richText = true;
                    if(GUILayout.Button($"<u>{dir}</u>", EditorStyles.label))
                    {
                        var newDir = EditorUtility.SaveFolderPanel("Select a folder", dir, "Builds");
                        if (!string.IsNullOrEmpty(newDir) && newDir != dir)
                        {
                            SetOutputDir(bs, newDir);
                        }
                    }
                    EditorStyles.label.richText = false;

                    if(GUILayout.Button("Build", GUILayout.ExpandWidth(false)))
                    {
                        try
                        {
                            var type = typeof(BuilderBase).Assembly.GetType(bs.Builder);
                            if (type == null)
                            {
                                Debug.LogError($"builder type is not found: {bs.Builder}");
                            }
                            else
                            {
                                var builder1 = Activator.CreateInstance(type, (object)bs) as BuilderBase;
                                builder1.Build(dir);
                            }
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
        }

        private void CollectSettings()
        {
            var bst = typeof(BuilderSettings);
            settings.Clear();
            foreach(var guid in AssetDatabase.FindAssets($"t:{bst.Name}", null))
            {
                settings.Add(AssetDatabase.LoadAssetAtPath<BuilderSettings>(AssetDatabase.GUIDToAssetPath(guid)));
            }

            builderSettingTypes = new List<Type>();
            foreach(var t in typeof(BuilderSettings).Assembly.GetTypes())
            {
                if(t.IsAssignableFrom(bst))
                {
                    builderSettingTypes.Add(t);
                }
            }
            builderSettingTypes.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            builderSettingNames = new string[builderSettingTypes.Count];
            for (int i = 0; i < builderSettingTypes.Count; i++)
            {
                builderSettingNames[i] = builderSettingTypes[i].Name;
            }
        }

        private void CreateSetting()
        {
            var t = builderSettingTypes[builderSettingIndex];
            var path = EditorUtility.SaveFilePanelInProject($"Save {t.Name}", t.Name, "asset", $"Save {t.Name}");
            if(!string.IsNullOrEmpty(path))
            {
                var obj = ScriptableObject.CreateInstance(typeof(BuilderSettings));
                AssetDatabase.CreateAsset(obj, path);
                AssetDatabase.Refresh();
                settings.Add(obj as BuilderSettings);
                settings.Sort((a, b) => a.name.CompareTo(b.name));
            }
        }

        private string GetOutputDir(BuilderSettings sb)
        {
            var key = sb.GetInstanceID();
            if (outputDirs.TryGetValue(key, out var dir))
            {
                return dir;
            }

            dir = EditorPrefs.GetString(key.ToString(), "");
            if (string.IsNullOrEmpty(dir))
            {
                dir = BuilderBase.GetDefaultOutputDir();
            }
            outputDirs[key] = dir;
            return dir;
        }

        private void SetOutputDir(BuilderSettings bs, string dir)
        {
            if(!outputDirs.TryGetValue(bs.GetInstanceID(), out var curDir) || curDir != dir)
            {
                outputDirs[bs.GetInstanceID()] = dir;
                EditorPrefs.SetString(bs.GetInstanceID().ToString(), dir);
            }
        }
    }
}