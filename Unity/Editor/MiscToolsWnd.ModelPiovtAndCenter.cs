using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public partial class MiscToolsWnd
    {

        //��ʱ��ģ�͵����Ĳ��Ǻ�֧�㲻���غϵģ�������������������ǵļ�ࡣ
        //Obj�����ϲ����������Ϊ֧�㡣���ĵ��µ�ģ�͵�λ�����������ĺ�֧��ļ��
        
        private Vector3 objectPivotAndCenterOffset;

        private void GUIPivotAndCenterOffset()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            GUILayout.Label("ģ��֧������ĵ�ľ���", GUILayout.ExpandWidth(false));
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