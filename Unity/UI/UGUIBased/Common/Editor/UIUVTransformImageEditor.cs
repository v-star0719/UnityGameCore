using GameCore.Unity.UGUIEx;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UIUVTransformImage))]
[CanEditMultipleObjects]
public class UIUVTransformImageEditor : ImageEditor
{
    private SerializedProperty uvRotation;
    private SerializedProperty flipHorizontal;
    private SerializedProperty flipVertical;

    protected override void OnEnable()
    {
        base.OnEnable();
        uvRotation = serializedObject.FindProperty("uvRotation");
        flipHorizontal = serializedObject.FindProperty("flipHorizontal");
        flipVertical = serializedObject.FindProperty("flipVertical");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("UV Transform Settings (Unity 6)", EditorStyles.boldLabel);

        serializedObject.Update();
        EditorGUILayout.PropertyField(uvRotation);
        EditorGUILayout.PropertyField(flipHorizontal);
        EditorGUILayout.PropertyField(flipVertical);
        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            foreach(var target in targets)
            {
                ((UIUVTransformImage)target).MarkVerticesDirtyNextFrame();
            }
        }
    }
}