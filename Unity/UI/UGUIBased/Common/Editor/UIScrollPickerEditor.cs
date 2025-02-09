using System.Collections;
using System.Collections.Generic;
using GameCore.Unity.UGUIEx;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIScrollPickerBase), true)]
public class UIScrollPickerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var picker = target as UIScrollPickerBase;
        GUILayout.Label($"CurrentSelect: {picker.CurSelectedIndex}");
    }
}
