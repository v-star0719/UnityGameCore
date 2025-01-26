using System.Collections;
using System.Collections.Generic;
using GameCore.Edit;
using GameCore.Unity.UGUIEx;
using KKK;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIScrollViewLocation), true)]
public class UIScrollViewLocationEditor : Editor
{
    private RectTransform testTarget;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        using (GUIUtil.LayoutHorizontal())
        {
            GUILayout.Label("testTarget", GUILayout.ExpandWidth(false));
            testTarget = EditorGUILayout.ObjectField(testTarget, typeof(RectTransform), true) as RectTransform;
            using (GUIUtil.Enabled(testTarget != null))
            {
                if(GUILayout.Button("LocateTo"))
                {
                    var picker = target as UIScrollViewLocation;
                    picker.ScrollTo(testTarget);
                }
            }
        }
    }
}
