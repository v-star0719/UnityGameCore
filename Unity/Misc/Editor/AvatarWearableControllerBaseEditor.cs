using GameCore.Unity.Edit;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity.Misc
{
    [CustomEditor(typeof(AvatarWearableControllerBase), true)]
    public class AvatarWearableControlBaseEditor : UnityEditor.Editor
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
