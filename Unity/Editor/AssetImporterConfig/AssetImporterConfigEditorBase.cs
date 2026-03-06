using UnityEditor;
using UnityEngine;

namespace GameCore.Unity.Editor
{
    public class AssetImporterConfigEditorBase : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var config = target as AssetImporterConfigBase;
            var parent = config.ParentNode;
            EditorGUILayout.ObjectField(serializedObject.FindProperty("parent"));
            config.isParentChanged = parent == config.ParentNode;

            var settings = target as AssetImporterConfigBase;
            if(settings.CheckParentLoop())
            {
                var clr = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("parent loop reference");
                GUI.color = clr;
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("desc"));

                OnInspectorGUI_Custom();

                //matchers
                EditorGUILayout.PropertyField(serializedObject.FindProperty("regList"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fileAndDirList"));
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnInspectorGUI_Custom()
        {
        }
    }
}