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

namespace GameCore.Unity
{
    public partial class MiscToolsWnd : EditorWindowBase
    {
        public string playerPrefersKey;
        public int playerPrefersInt;
        public int playerPrefersString;
        private EditDataStringField batchNameFormat;

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

            if (GUIUtil.Button("CreateTexture2DArray"))
            {
                CreateTexture2DArray.ShowWindow();
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
            
            GUIPivotAndCenterOffset();
        }
    }
}