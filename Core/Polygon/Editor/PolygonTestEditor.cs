using UnityEditor;
using UnityEngine;

namespace Assets.GameCore.Core.Polygon.Editor
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