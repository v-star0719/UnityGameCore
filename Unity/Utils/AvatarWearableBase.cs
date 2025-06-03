using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.Unity
{
    //主要就是用来获取 SkinnedMeshRenderer
    public class AvatarWearableBase : MonoBehaviour
    {
        [Serializable]
        public class MeshSkinData
        {
            public GameObject meshRenderer;
            public string targetBone;
        }
        public List<SkinnedMeshRenderer> skins = new();
        public List<MeshSkinData> meshSkins = new();

        public void OnDestroy()
        {
            foreach (var m in meshSkins)
            {
                if (m.meshRenderer != null)
                {
                    Destroy(m.meshRenderer);  
                }
            }
        }
    }
}