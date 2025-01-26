using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public partial class MiscToolsWnd
    {

        //有时候模型的中心不是和支点不是重合的，这个工具用来调整他们的间距。
        //Obj是最上层的容器，作为支点。更改底下的模型的位置来调整中心和支点的间距
        
        private Vector3 objectPivotAndCenterOffset;

        private void GUIPivotAndCenterOffset()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            GUILayout.Label("模型支点和中心点的距离", GUILayout.ExpandWidth(false));
            objectPivotAndCenterOffset = EditorGUILayout.Vector3Field(GUIContent.none, objectPivotAndCenterOffset, GUILayout.Width(100));
            if (GUILayout.Button("Set", GUILayout.ExpandWidth(false)))
            {
                SetObjectPivotAndCenterOffset(Selection.activeGameObject, objectPivotAndCenterOffset);
            }

            GUILayout.EndHorizontal();
            GUILayout.Label(Selection.activeGameObject == null ? "Please select a GameObject" : $"Select: {Selection.activeGameObject.name}");
            GUILayout.EndVertical();
        }

        private void SetObjectPivotAndCenterOffset(GameObject obj, Vector3 offset)
        {
            var meshRenders = obj.GetComponentsInChildren<MeshRenderer>();
            if (meshRenders.Length <= 0)
            {
                Debug.Log("No mesh renderer");
                return;
            }

            var bounds = meshRenders[0].bounds;
            for (int i = 1; i < meshRenders.Length; i++)
            {
                bounds.Encapsulate(meshRenders[i].bounds);
            }

            var o = obj.transform.position - bounds.center + offset;
            foreach (var r in meshRenders)
            {
                r.transform.localPosition += o;
            }
        }
    }
}