using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;

namespace Kernel.Unity
{
    public class AvatarDressUpControllerBase : MonoBehaviour
    {
        public SkinnedMeshRenderer body;

        public virtual void AddDressUp(AvatarDressUpBase dressUp)
        {
            GameObjectUtils.SetLayer(dressUp.transform, gameObject.layer);
            dressUp.transform.SetParent(transform, false);
            foreach (var skin in dressUp.skins)
            {
                CombineSkinnedMesh(skin);
            }
        }

        public virtual void RemoveDressUp(GameObject go)
        {
            Destroy(go);
        }
        
        public void CombineSkinnedMesh(SkinnedMeshRenderer mesh)
        {
            List<Transform> bones = new List<Transform>();
            foreach (var clothBone in mesh.bones)
            {
                bool find = false;
                foreach (var bodyBone in body.bones)
                {
                    if (bodyBone.name == clothBone.name)
                    {
                        find = true;
                        bones.Add(bodyBone);
                        break;
                    }
                }

                if (!find)
                {
                    Debug.LogError("Missing bone " + clothBone.name);
                    return;
                }
            }
            mesh.bones = bones.ToArray();
        }
    }
}