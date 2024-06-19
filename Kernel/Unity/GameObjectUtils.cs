using UnityEngine;

class GameObjectUtils
{

    public static void SetLayer(Transform trans, int layer)
    {
        if (trans.gameObject.layer == layer)
        {
            return;
        }
        trans.gameObject.layer = layer;
        foreach (Transform t in trans)
        {
            SetLayer(t, layer);
        }
    }

    public static void TraverseTransformTree(Transform root, System.Action<Transform> onVisit)
    {
        if (onVisit != null)
        {
            onVisit(root);
        }
        foreach (Transform child in root)
        {
            TraverseTransformTree(child, onVisit);
        }
    }
}
