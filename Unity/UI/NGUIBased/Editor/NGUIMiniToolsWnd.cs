#if NGUI

using System.Collections.Generic;
using GameCore.Edit;
using GameCore.Unity;
using UnityEditor;
using UnityEngine;
using Selection = UnityEditor.Selection;

namespace Assets.Kernel.Unity.Editor
{
    public class NGUIMiniToolsWnd : EditorWindow
    {
        private int depthOffset;
        private int depth;
        private int width;
        private int height;
        private float localX;
        private float localY;
        private float localZ;
        private float localXScale;
        private float localYScale;
        private float localZScale;

        private string spriteName;

        [MenuItem("VStar/NGUIEditorUtils")]
        public static void GetWindow()
        {
            GetWindow<NGUIMiniToolsWnd>();
        }

        private void OnGUI()
        {
            using (GUIUtil.LayoutHorizontal())
            {
                if (GUILayout.Button("widget 宽高", GUILayout.ExpandWidth(false)))
                {
                    ChangeSize(width, height);
                }

                width = EditorGUILayout.IntField(width, GUILayout.Width(100));
                GUILayout.Label("X", GUILayout.ExpandWidth(false));
                height = EditorGUILayout.IntField(height, GUILayout.Width(100));
            }

            using (GUIUtil.LayoutHorizontal())
            {
                if (GUILayout.Button("widget depth", GUILayout.ExpandWidth(false)))
                {
                    ChangeDepth(depth);
                }

                depth = EditorGUILayout.IntField(depth, GUILayout.Width(100));
            }

            using (GUIUtil.LayoutHorizontal())
            {
                if (GUILayout.Button("所有子widget的depth增加"))
                {
                    ChangeChildDepthByOffset(depthOffset);
                }

                depthOffset = EditorGUILayout.IntField(depthOffset, GUILayout.Width(100));
            }


            using (GUIUtil.LayoutHorizontal())
            {
                if (GUILayout.Button("所有local坐标缩放"))
                {
                    ScaleLocalPosition(localXScale, localYScale, localZScale);
                }

                localXScale = EditorGUILayout.FloatField(localXScale, GUILayout.Width(100));
                localYScale = EditorGUILayout.FloatField(localYScale, GUILayout.Width(100));
                localZScale = EditorGUILayout.FloatField(localZScale, GUILayout.Width(100));
            }

            if (Selection.gameObjects.Length > 0)
            {
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    GUILayout.Label("选中：" + Selection.gameObjects[i].name);
                }
            }
            else
            {
                GUILayout.Label("请先选中一个NGUI对象根节点");
            }

            using (GUIUtil.LayoutHorizontal())
            {
                if (GUILayout.Button("FindSpriteNameInSelectedPrefabs"))
                {
                    FindSpriteNameInPrefabs();
                }
            }
        }

        private void ChangeSize(int width, int height)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                var w = Selection.gameObjects[i].GetComponent<UIWidget>();
                w.width = width;
                w.height = height;
            }
        }

        private void ChangeDepth(int depth)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                var w = Selection.gameObjects[i].GetComponent<UIWidget>();
                w.depth = depth;
            }
        }

        private void ChangeChildDepthByOffset(int offset)
        {
            var list = Selection.activeGameObject.GetComponentsInChildren<UIWidget>(true);
            for (int i = 0; i < list.Length; i++)
            {
                list[i].depth += offset;
            }
        }

        private void ScaleLocalPosition(float x, float y, float z)
        {
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                var pos = Selection.transforms[i].localPosition;
                pos.x *= x;
                pos.y *= y;
                pos.z *= z;
                Selection.transforms[i].localPosition = pos;
            }
        }

        private void AddToRoot(Transform child, Transform node, string nodeName)
        {
            var t = Find(node, nodeName);
            if (t != null)
            {
                child.parent = t;
            }
        }

        private Transform Find(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var tt = t.GetChild(i);
                if (tt.name == name)
                {
                    return tt;
                }
            }

            return null;
        }

        private List<Transform> GetChildren(Transform t)
        {
            var list = new List<Transform>();
            for (int i = 0; i < t.childCount; i++)
            {
                list.Add(t.GetChild(i));
            }

            return list;
        }

        public void FindSpriteNameInPrefabs()
        {
            EditorUtils.IterateAssetsInFolder(EditorUtils.GetSelectedFolder(),
                (filePath) =>
                {
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                    if (go != null)
                    {
                        var sprites = go.GetComponentsInChildren<UISprite>(true);
                        for (int j = 0; j < sprites.Length; j++)
                        {
                            if (sprites[j].spriteName == spriteName)
                            {
                                string name = sprites[j].name;
                                var t = sprites[j].transform;
                                while (t.parent)
                                {
                                    name = t.parent.name + "/" + name;
                                    t = t.parent;
                                }

                                Debug.LogError(name);
                            }
                        }
                    }
                });
        }
    }
}

#endif