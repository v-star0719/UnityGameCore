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

        //因为游戏内可能也要判断是否是prefab编辑模式，所以没有放进编辑器程序集
        // 判断当前GameObject是否在Prefab编辑模式下
        public static bool IsEditingInPrefabMode(GameObject gameObject)
        {
#if UNITY_EDITOR
            // 如果不在编辑器模式，直接返回false
            if(!Application.isEditor)
                return false;

            // 检查是否为Prefab Asset（未打开的Prefab文件）
            if(UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
                return true;

            // 检查是否为打开的Prefab实例（正在编辑的Prefab）
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            return prefabStage != null && prefabStage.IsPartOfPrefabContents(gameObject);
#else
            return false;
#endif
        }
    }
}

