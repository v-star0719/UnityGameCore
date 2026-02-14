using System.Collections.Generic;
using GameCore.Edit;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public class AssetImporterConfigWindowBase<T> : EditorWindowBase where T : AssetImporterConfigBase
    {
        private List<T> settings = new();
        private Vector2 scrollPos;
        private TreeView treeView;

        //当修改配置里的parent属性后，会清空这里的缓存
        private static T _rootConfig;
        private static string multiRootTip = "Multiple root configs have been found. There should be only one root node, which is the node with a parent of null. There should be exactly one configuration with a null parent property.";
        private static bool findMultiRoot = false;

        public static T RootConfig
        {
            get
            {
                if(_rootConfig == null)
                {
                    var list = new List<T>();
                    foreach(var guid in AssetDatabase.FindAssets($"t:{typeof(T).Name}"))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var tis = AssetDatabase.LoadAssetAtPath<T>(path);
                        list.Add(tis);
                    }

                    foreach(var tis in list)
                    {
                        tis.ChildNodes.Clear();
                    }
                    findMultiRoot = false;

                    foreach(var tis in list)
                    {
                        if(tis.ParentNode == null)
                        {
                            if(_rootConfig != null)
                            {
                                //发现了多个根节点，这可能会出现非预期的问题
                                Debug.LogWarning(multiRootTip);
                                findMultiRoot = true;
                            }
                            else
                            {
                                _rootConfig = tis;
                            }
                        }
                        else
                        {
                            tis.ParentNode.ChildNodes.Add(tis);
                        }
                    }
                }
                return _rootConfig;
            }
            set => _rootConfig = value;
        }

        #region Entrance
        protected override void OnEnable()
        {
            base.OnEnable();
            treeView = new TreeView(120, 60, 40, 40);
            treeView.onNodeClick = OnNodeClick;
            InitTree();
        }
        #endregion


        protected virtual void InitTree()
        {
            CollectSettings();
            T rootNode = null;
            foreach(var s in settings)
            {
                s.ChildNodes.Clear();
            }

            findMultiRoot = false;
            foreach(var s in settings)
            {
                if(s.ParentNode == null)
                {
                    if(rootNode == null)
                    {
                        rootNode = s;
                    }
                    else
                    {
                        Debug.LogWarning(multiRootTip);
                        findMultiRoot = true;
                    }
                }
                else
                {
                    s.ParentNode.ChildNodes.Add(s);
                }
            }
            treeView.rootNode = rootNode;
        }

        protected void OnGUI()
        {
            treeView.OnGUI(position);
            using(GUIUtil.LayoutHorizontal())
            {
                if(GUI.Button(new Rect(0, 0, 60, 25), "Create"))
                {
                    CreateSetting();
                }
                if(GUI.Button(new Rect(80, 0, 60, 25), "Refresh"))
                {
                    InitTree();
                }
            }
            if(findMultiRoot)
            {
                using(GUIUtil.Color(Color.red))
                {
                    GUI.Box(new Rect(0, 28, position.width * 0.5f - 60, position.height - 28), multiRootTip);
                }
            }
        }

        protected virtual void OnNodeClick(ITreeNode node)
        {
            var s = node as T;
            Selection.activeObject = s;
            EditorGUIUtility.PingObject(s);
        }

        private void CollectSettings()
        {
            var configType = typeof(T);
            settings.Clear();
            foreach(var guid in AssetDatabase.FindAssets($"t:{configType.Name}", null))
            {
                settings.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));
            }
        }

        private void CreateSetting()
        {
            var t = typeof(T);
            var path = EditorUtility.SaveFilePanelInProject($"Save {t.Name}", t.Name, "asset", $"Save {t.Name}");
            if(!string.IsNullOrEmpty(path))
            {
                var obj = ScriptableObject.CreateInstance(typeof(T));
                AssetDatabase.CreateAsset(obj, path);
                AssetDatabase.Refresh();
                settings.Add(obj as T);
                settings.Sort((a, b) => a.Name.CompareTo(b.Name));
            }
        }
    }
}