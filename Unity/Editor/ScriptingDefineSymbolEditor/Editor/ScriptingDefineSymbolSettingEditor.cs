using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorInternal;
using UnityEngine;

namespace GameCore.Unity
{
    [CustomEditor(typeof(ScriptingDefineSymbolSetting))]
    public class ScriptingDefineSymbolSettingEditor : Editor
    {
        private ScriptingDefineSymbolSetting setting;
        private ReorderableList list;

        private void OnEnable()
        {
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("symbols"));
            list.drawHeaderCallback = DrawHeader;
            list.drawElementCallback = DrawElement;
            list.elementHeightCallback = ElementHeight;
            list.onAddCallback = OnAdd;
            list.onRemoveCallback = OnRemove;
            list.onReorderCallback = OnReorder;

            //local function.
            void DrawHeader(Rect rect)
            {
                EditorGUI.LabelField(rect, "List");
            }
            void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                var toggleWith = 24;
                var indexWith = 12;
                var rightMargin = 4;
                var valueWith = (rect.width - toggleWith - indexWith - rightMargin) * 0.6f;
                var descWith = (rect.width - toggleWith - indexWith - rightMargin) * 0.4f;

                rect.height = EditorGUIUtility.singleLineHeight;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

                rect.width = indexWith;
                EditorGUI.LabelField(rect, index.ToString());
                rect.x += indexWith;
                rect.width = valueWith;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("value"), GUIContent.none);
                rect.x += valueWith + 8;
                rect.width = toggleWith - 8;//4像素是和左边输入框的间隙
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("enabled"), GUIContent.none);
                rect.x += toggleWith;
                rect.width = descWith;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("desc"), GUIContent.none);
            }
            float ElementHeight(int index)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            void OnAdd(ReorderableList list)
            {
                list.serializedProperty.InsertArrayElementAtIndex(list.index);
            }
            void OnRemove(ReorderableList list)
            {
                list.serializedProperty.DeleteArrayElementAtIndex(list.index);
            }
            void OnReorder(ReorderableList list)
            {
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            setting = target as ScriptingDefineSymbolSetting;

            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            //for (var i = 0; i < setting.symbols.Count; i++)
            //{
            //    var symbol = setting.symbols[i];
            //    using (new GUILayout.HorizontalScope())
            //    {
            //        symbol.value = GUILayout.TextField(symbol.value);
            //        symbol.enabled = GUILayout.Toggle(symbol.enabled, GUIContent.none, GUILayout.ExpandWidth(false));
            //    }
            //}

            if(GUILayout.Button("Load From Player Settings"))
            {
                LoadFromPlayerSettings();
            }

            if(GUILayout.Button("Apply"))
            {
                Apply();
            }
        }

        private void Apply()
        {
            List<string> sl = new List<string>();
            foreach (var t in setting.symbols)
            {
                if (t.enabled)
                {
                    sl.Add(t.value);
                }
            }
            PlayerSettings.SetScriptingDefineSymbols(EditorUtils.CurrentNamedBuildTarget, sl.ToArray());
        }

        private void LoadFromPlayerSettings()
        {
            var t = EditorUtils.CurrentNamedBuildTarget;
            PlayerSettings.GetScriptingDefineSymbols(t, out var defines);
            foreach (var s in defines)
            {
                setting.symbols.Add(new ScriptingDefineSymbolItem()
                {
                    value = s,
                    enabled = true
                });
            }
        }
    }
}