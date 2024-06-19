using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Kernel.Unity
{
    //名字改成首字母大写的
    public class FixGameObjectName
    {
        public static void Fix()
        {
            if (Selection.activeTransform == null)
            {
                Debug.LogError("no GameObject selected");
                return;
            }
            TransformUtils.TraverseTransformTree(Selection.activeTransform, trans =>
            {
                if (char.IsLower(trans.name, 0))
                {
                    var array = trans.name.ToCharArray();
                    array[0] = char.ToUpper(array[0]);
                    trans.name = new string(array);
                }
            });
        }
    }
}