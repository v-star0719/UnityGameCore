using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Kernel.Unity
{
    public static class TransformUtils
    {
        //root节点也会回调一次
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

        public static Transform DeeplyFind(Transform trans, string childName)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                var t = trans.GetChild(i);
                if (t.name == childName)
                {
                    return t;
                }
                else
                {
                    t = DeeplyFind(t, childName);
                    if (t != null)
                    {
                        return t;
                    }
                }
            }

            return null;
        }

        public static string GetTransformPath(Transform node, Transform root)
        {
            string rt = node.name;
            node = node.parent;
            while (node != root)
            {
                rt = $"{node.name}/{rt}";
                node = node.parent;
            }
            return rt;
        }

        public static int GetChildCount(Transform t)
        {
            if (t == null)
            {
                return 0;
            }

            int n = t.childCount;
            for (int i = 0; i < t.childCount; i++)
            {
                n += GetChildCount(t.GetChild(i));
            }

            return n;
        }

        public static void ReplaceStringInName(Transform transform, string find, string replace)
        {
            var newName = transform.name.Replace(find, replace);
            if (newName != transform.name)
            {
                Debug.LogFormat("{0} --> {1}", transform.name, newName);
                transform.name = newName;
            }

            int n = transform.childCount;
            for (int i = 0; i < n; i++)
            {
                var t = transform.GetChild(i);
                ReplaceStringInName(t, find, replace);
            }
        }

        ///现在用Transform.SetParent就行了。
        public static void SetParent(Transform trans, Transform parent)
        {
            trans.parent = parent;
            trans.localPosition = Vector3.zero;
            trans.rotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
    }
}
