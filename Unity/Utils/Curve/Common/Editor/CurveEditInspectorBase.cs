using UnityEditor;
using UnityEngine;

namespace Curves
{
    public class CurveEditInspectorBase<T> : Editor where T : CurveData
    {
        protected CurveEditBase<T> edit;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            edit = target as CurveEditBase<T>;
            EditorGUILayout.ObjectField(serializedObject.FindProperty("data"), new GUIContent("data"));
            GUI.enabled = false;
            EditorGUILayout.ObjectField("workingData", edit.workingData, typeof(BezierCombineCurveData), false);
            GUI.enabled = true;

            var p = serializedObject.FindProperty("markSize");
            p.floatValue = EditorGUILayout.FloatField("markSize", p.floatValue);

            p = serializedObject.FindProperty("smooth");
            var v = EditorGUILayout.IntField("smooth", p.intValue);
            p.intValue = v < 1 ? 1 : v;

            AfterGUIFields();

            GUILayout.BeginVertical(GUI.skin.box);
            {
                var points = edit.posList;
                for (int i = 0; i < points.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(i.ToString(), GUILayout.ExpandWidth(false));
                    GUILayout.Label(points[i].ToString());
                    GUILayout.EndHorizontal();
                }

                if (points.Count == 0)
                {
                    GUILayout.Label("no point, please add");
                }
            }
            GUILayout.EndVertical();

            AfterGUIList();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
                {
                    edit.Clear();
                }

                if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
                {
                    edit.Save();
                }
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void AfterGUIFields()
        {
        }

        protected virtual void AfterGUIList()
        {
        }
    }
}