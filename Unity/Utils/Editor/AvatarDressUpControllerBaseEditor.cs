using System.Collections;
using System.Collections.Generic;
using GameCore.Edit;
using GameCore.Unity;
using Kernel.Unity;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    [CustomEditor(typeof(AvatarDressUpControllerBase))]
    public class AvatarDressUpControlBaseEditorEditor : Editor
    {
        private SkinnedMeshRenderer skin;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            skin = EditorGUILayout.ObjectField("dressUp", skin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
            using (GUIUtil.Enabled(skin != null))
            {
                if(GUILayout.Button("Combine"))
                {
                    (target as AvatarDressUpControllerBase).CombineSkinnedMesh(skin);
                }
            }
        }
    }
}
