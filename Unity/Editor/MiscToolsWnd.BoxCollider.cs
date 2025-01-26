using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public partial class MiscToolsWnd
    {
        public static void BoxColliderFitToMesh()
        {
            if (Selection.transforms.Length == 0)
            {
                return;
            }

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                var trans = Selection.transforms[i];
                BoxCollider collider = trans.GetComponentInChildren<BoxCollider>();
                if (collider == null)
                {
                    Debug.LogError(trans + "no collider");
                    return;
                }

                MeshRenderer renderer = trans.GetComponentInChildren<MeshRenderer>();
                if (renderer == null)
                {
                    Debug.LogError(trans + "no mesh renderer");
                    return;
                }

                collider.center = renderer.bounds.center - trans.position;
                collider.size = renderer.bounds.size;
            }
        }
    }
}