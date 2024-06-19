using System.Collections;
using System.Collections.Generic;
using Kernel.Unity;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MirrorPlane))]
public class MirrorPlaneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var mp = target as MirrorPlane;
        if (GUILayout.Button("Reset"))
        {
            mp.Reset();
        }
    }
}
