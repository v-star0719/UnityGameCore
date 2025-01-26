using GameCore.Unity.UGUIEx;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIAnchorToObject))]
public class UIAnchorToObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        GUIAnchorData("Left", serializedObject.FindProperty("leftAnchor"), true);
        GUIAnchorData("Right", serializedObject.FindProperty("rightAnchor"), true);
        GUIAnchorData("Top", serializedObject.FindProperty("topAnchor"), false);
        GUIAnchorData("Bottom", serializedObject.FindProperty("bottomAnchor"), false);
        serializedObject.ApplyModifiedProperties();
    }

    private void GUIAnchorData(string name, SerializedProperty prop, bool isHorz)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(name, GUILayout.Width(50));
        var labelWidth = EditorGUIUtility.labelWidth;

        var p = prop.FindPropertyRelative("target");
        p.objectReferenceValue = EditorGUILayout.ObjectField(p.objectReferenceValue, typeof(RectTransform), true);
        //
        p = prop.FindPropertyRelative("side");
        if (isHorz)
        {
            p.intValue = (int)(UIAnchorToObject.RectSideHorizontal)EditorGUILayout.EnumPopup((UIAnchorToObject.RectSideHorizontal)p.intValue);
        }
        else
        {
            p.intValue = (int)(UIAnchorToObject.RectSideVertical)EditorGUILayout.EnumPopup((UIAnchorToObject.RectSideVertical)p.intValue);
        }
        //
        EditorGUIUtility.labelWidth = 40;
        p = prop.FindPropertyRelative("offset");
        p.floatValue = EditorGUILayout.FloatField("offset", p.floatValue, GUILayout.ExpandWidth(false));

        EditorGUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = labelWidth;
    }
}
