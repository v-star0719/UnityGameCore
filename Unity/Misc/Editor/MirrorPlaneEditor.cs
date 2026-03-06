using UnityEditor;
using UnityEngine;

namespace GameCore.Unity.Misc
{
    [CustomEditor(typeof(MirrorPlane))]
    public class MirrorPlaneEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var mp = target as MirrorPlane;
            if(GUILayout.Button("Reset"))
            {
                mp.Reset();
            }
        }
    }
}

