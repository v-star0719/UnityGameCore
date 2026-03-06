using UnityEditor;
using UnityEngine;

namespace GameCore.Core.Polygon
{
    [CustomEditor(typeof(PolygonTest))]
    public class PolygonTestEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Test"))
            {
                //(target as PolygonTest).Test();
            }
        }
    }
}