using UnityEngine;

namespace GameCore.Unity
{
    public class GameObjectUtils
    {
        public static void SetLayer(Transform trans, int layer)
        {
            if(trans.gameObject.layer == layer)
            {
                return;
            }
            trans.gameObject.layer = layer;
            foreach(Transform t in trans)
            {
                SetLayer(t, layer);
            }
        }

        public static void TraverseTransformTree(Transform root, System.Action<Transform> onVisit)
        {
            if(onVisit != null)
            {
                onVisit(root);
            }
            foreach(Transform child in root)
            {
                TraverseTransformTree(child, onVisit);
            }
        }

        //��Ϊ��Ϸ�ڿ���ҲҪ�ж��Ƿ���prefab�༭ģʽ������û�зŽ��༭������
        // �жϵ�ǰGameObject�Ƿ���Prefab�༭ģʽ��
        public static bool IsEditingInPrefabMode(GameObject gameObject)
        {
#if UNITY_EDITOR
            // ������ڱ༭��ģʽ��ֱ�ӷ���false
            if(!Application.isEditor)
                return false;

            // ����Ƿ�ΪPrefab Asset��δ�򿪵�Prefab�ļ���
            if(UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
                return true;

            // ����Ƿ�Ϊ�򿪵�Prefabʵ�������ڱ༭��Prefab��
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            return prefabStage != null && prefabStage.IsPartOfPrefabContents(gameObject);
#else
            return false;
#endif
        }
    }
}

