using UnityEditor;
using UnityEngine;

namespace Curves
{
    [CustomEditor(typeof(BSplineEdit))]
    public class BSplineInspector : CurveEditInspectorBase<BSplineData>
    {
        protected override void AfterGUIFields()
        {
            var edt = edit as BSplineEdit;
            edt.polynomial = EditorGUILayout.IntField("polynomial", edt.polynomial);
        }

        protected override void AfterGUIList()
        {
            var edt = edit as BSplineEdit;
            GUILayout.BeginVertical(GUI.skin.box);
            {
                var points = edt.uList;
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
        }
    }
}