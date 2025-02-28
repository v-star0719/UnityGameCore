using System.Collections;
using System.Collections.Generic;
using GameCore.Edit;
using GameCore.Unity;
using Kernel.Unity;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    [CustomEditor(typeof(AvatarWearableControllerBase), true)]
    public class AvatarWearableControlBaseEditor : Editor
    {
        private SkinnedMeshRenderer skin;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            skin = EditorGUILayout.ObjectField("Wearable", skin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
            using (GUIUtil.Enabled(skin != null))
            {
                if(GUILayout.Button("Combine"))
                {
                    (target as AvatarWearableControllerBase).CombineSkinnedMesh(skin);
                }
            }
        }
    }
}
