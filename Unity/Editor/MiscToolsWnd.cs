using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Edit;
using GameCore.Unity;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Unity
{
    public partial class MiscToolsWnd : EditorWindowBase
    {
        public string playerPrefersKey;
        public int playerPrefersInt;
        public int playerPrefersString;
        private EditDataStringField batchNameFormat;
        private Color linnerToSrgbSource;
        private Color linnerToSrgbTarget;

        [MenuItem("Tools/Misc/MiniTools")]
        public static void Open()
        {
            GetWindow<MiscToolsWnd>("MiscToolsWnd");
        }

        protected override void InitEditDataFields()
        {
            base.InitEditDataFields();
            batchNameFormat = new("MiscToolsWnd.batchNameFormat", "{0}", this);
        }

        public void OnGUI()
        {
            if (GUIUtil.Button("FixGameObjectName(CamelCase)"))
            {
                FixGameObjectName.Fix();
            }

            if (GUIUtil.Button("GetChildrenCountDeeply"))
            {
                int childCount = TransformUtils.GetChildCount(Selection.activeTransform);
                EditorUtility.DisplayDialog("", childCount.ToString(), "ok");
            }

            if (GUIUtil.Button("GetChildrenCount"))
            {
                int childCount = Selection.activeTransform != null ? Selection.activeTransform.childCount : 0;
                EditorUtility.DisplayDialog("", childCount.ToString(), "ok");
            }

            if (GUIUtil.Button("BoxColliderFitToMesh"))
            {
                BoxColliderFitToMesh();
            }

            if (GUIUtil.Button("MissingScript"))
            {
                MissingScript.CheckSelectGameObjects();
            }

            if (GUIUtil.Button("CheckUncompressedTextures"))
            {
                CheckUncompressedTextures();
            }

            using (GUIUtil.LayoutHorizontal())
            {
                if(GUIUtil.Button("SetDirtyAndSave"))
                {
                    foreach (var guid in Selection.assetGUIDs)
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
                        EditorUtility.SetDirty(asset);
                        AssetDatabase.SaveAssetIfDirty(asset);
                    }
                    ShowNotification(new GUIContent($"Finish {Selection.assetGUIDs.Length}"), 2);
                }
            }

            using (Edit.GUIUtil.LayoutHorizontal(EditorGUIUtil.StyleBox))
            {
                GUILayout.Label("SetPlayerPrefers", GUILayout.ExpandWidth(false));
                GUILayout.Label("key");
                bool b = false;
                playerPrefersKey = EditorGUIUtil.TextFieldCompact("key", playerPrefersKey, ref b, GUILayout.Width(100));
                playerPrefersInt = EditorGUIUtil.IntFieldCompact("int", playerPrefersInt, ref b);
                if (GUIUtil.Button("ok"))
                {
                    PlayerPrefs.SetInt(playerPrefersKey, playerPrefersInt);
                }
            }

            using (Edit.GUIUtil.LayoutHorizontal(EditorGUIUtil.StyleBox))
            {
                GUILayout.Label("BatchRename", GUILayout.ExpandWidth(false));
                batchNameFormat.Value = EditorGUILayout.TextField(batchNameFormat.Value);
                if (GUILayout.Button($"Do (Select:{Selection.objects.Length}", GUILayout.ExpandWidth(false)))
                {
                    for (var i = 0; i < Selection.objects.Length; i++)
                    {
                        var obj = Selection.objects[i];
                        obj.name = string.Format(batchNameFormat.Value, i);
                    }
                }
            }

            using (Edit.GUIUtil.LayoutHorizontal(EditorGUIUtil.StyleBox))
            {
                GUILayout.Label("LinnerToSrgb", GUILayout.ExpandWidth(false));
                var clr = EditorGUILayout.ColorField(linnerToSrgbSource, GUILayout.Width(100));
                GUILayout.Label("===>", GUILayout.ExpandWidth(false));
                EditorGUILayout.ColorField(linnerToSrgbTarget, GUILayout.Width(100));
                if (clr != linnerToSrgbSource)
                {
                    linnerToSrgbSource = clr;
                    linnerToSrgbTarget = new Color(L2S(clr.r), L2S(clr.g), L2S(clr.b));
                }
            }

            using (Edit.GUIUtil.LayoutHorizontal(EditorGUIUtil.StyleBox))
            {
                if(GUILayout.Button($"复制对象路径", GUILayout.ExpandWidth(false)))
                {
                    EditorGUIUtility.systemCopyBuffer = TransformUtils.GetTransformPath(Selection.activeTransform);
                }
            }

            GUIPivotAndCenterOffset();
        }

        private static string[] compressedFormats = new[]{"DXT", "BC", "ETC", "EAC", "ASTC"};
        public void CheckUncompressedTextures()
        {
            var folder = EditorUtils.GetSelectedFolder();
            EditorUtils.IterateAssetsInFolder(folder, path =>
            {
                var format = TextureFormat.ASTC_10x10;
                var tex2d = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                Texture tex = null;
                if (tex2d != null)
                {
                    format = tex2d.format;
                    tex = tex2d;
                }
                else
                {
                    var tex3d = AssetDatabase.LoadAssetAtPath<Texture3D>(path);
                    if (tex3d != null)
                    {
                        format = tex3d.format;
                        tex = tex3d;
                    }
                }

                var f = format.ToString();
                foreach (var s in compressedFormats)
                {
                    if (f.Contains(s))
                    {
                        return;
                    }
                }
                Debug.Log($"{path} is no compressed: {format}", tex);
            });
        }

        public static float L2S(float f)
        {
            return f <= 0.0031308f ? 12.92f * f : 1.055f * (f * (1 / 2.4f)) - 0.055f;
        }

    }
}