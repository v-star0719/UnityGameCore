using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel.Edit;
using Kernel.Unity;
using UnityEditor;
using UnityEngine;

namespace Kernel.Unity
{
    public class MiscToolsWnd : EditorWindow
    {
        [MenuItem("Tools/Misc/MiniTools")]
        public static void Open()
        {
            GetWindow<MiscToolsWnd>("MiscToolsWnd");
        }

        public void OnGUI()
        {
            if (GUIUtil.Button("FixGameObjectName(CamelCase)"))
            {
                FixGameObjectName.Fix();
            }

            if (GUIUtil.Button("GetChildrenCountDeeply"))
            {
                int childCount = TransformUtils.GetChildCount(Selection.activeTransform);
                EditorUtility.DisplayDialog("", childCount.ToString(), "ok");
            }

            if (GUIUtil.Button("GetChildrenCount"))
            {
                int childCount = Selection.activeTransform != null ? Selection.activeTransform.childCount : 0;
                EditorUtility.DisplayDialog("", childCount.ToString(), "ok");
            }

            if (GUIUtil.Button("BoxColliderFitToMesh"))
            {
                BoxColliderFitToMesh();
            }
        }

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