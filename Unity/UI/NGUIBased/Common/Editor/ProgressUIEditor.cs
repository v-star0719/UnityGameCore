using System.Collections;
using System.Collections.Generic;
using Fight;
using GameCore.Unity.NGUIEx;
using UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(ProgressUI))]
public class ProgressUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var p = target as ProgressUI;
        p.Value = EditorGUILayout.Slider("Value", p.Value, 0, 1);
    }
}
