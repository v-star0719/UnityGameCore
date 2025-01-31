using System.Collections;
using System.Collections.Generic;
using Config;
using GameCore.Unity;
using UnityEngine;

namespace Kernel.Unity
{
    public class AvatarWearableControllerBase : MonoBehaviour
    {
        public SkinnedMeshRenderer body;
        
        private List<AvatarWearableBase> wearables = new();

        public virtual void AddWearable(AvatarWearableBase wearable)
        {
            wearables.Add(wearable);
            GameObjectUtils.SetLayer(wearable.transform, gameObject.layer);
            wearable.transform.SetParent(transform, false);
            foreach (var skin in wearable.skins)
            {
                CombineSkinnedMesh(skin);
            }

            foreach (var skin in wearable.meshSkins)
            {
                CombineMesh(skin.meshRenderer, skin.targetBone);
            }
        }

        public virtual void RemoveWearable(AvatarWearableBase wearable)
        {
            wearables.Remove(wearable);
            Destroy(wearable.gameObject);
        }

        public virtual void ChangeBody(SkinnedMeshRenderer newBody)
        {
            body = newBody;
            foreach (var wearable in wearables)
            {
                foreach(var skin in wearable.skins)
                {
                    CombineSkinnedMesh(skin);
                }
            }
        }

        public void CombineSkinnedMesh(SkinnedMeshRenderer mesh)
        {
            List<Transform> bones = new List<Transform>();
            foreach (var clothBone in mesh.bones)
            {
                var bodyBone = FindBoneOnBody(clothBone.name);
                if (bodyBone == null)
                {
                    //Debug.LogWarning($"Missing bone:{clothBone.name}£¬try to create fake bone");
                    //var parent = FindBoneOnBody(clothBone.transform.parent.name);
                    //if (parent == null)
                    //{
                    //    Debug.LogError($"failed, no parent bone: {clothBone.transform.parent.name}");
                    //    return;
                    //}

                    //var go = new GameObject(clothBone.name);
                    //bodyBone = go.transform;
                    //bodyBone.parent = parent;
                    //bodyBone.transform.localPosition = clothBone.localPosition;
                    //bodyBone.transform.localScale = clothBone.localScale;
                    //bodyBone.transform.localRotation = clothBone.localRotation;
                    //Debug.LogWarning("Create fake bone success");
                    bodyBone = CreateFakeBoneOnBody(clothBone);
                    if (bodyBone==null)
                    {
                        return;
                    }
                }

                bones.Add(bodyBone);
            }
            mesh.bones = bones.ToArray();
        }

        public void CombineMesh(GameObject mesh, string boneName)
        {
            var bone = FindBoneOnBody(boneName);
            if (bone == null)
            {
                Debug.LogError($"bone was not found: {boneName}");
                bone = transform;
            }
            TransformUtils.SetParent(mesh.transform, bone);
        }

        private Transform FindBoneOnBody(string boneName)
        {
            foreach(var bodyBone in body.bones)
            {
                if(bodyBone.name == boneName)
                {
                    return bodyBone;
                }
            }
            return null;
        }

        private Transform CreateFakeBoneOnBody(Transform bone)
        {
            var parentBone = FindBoneOnBody(bone.parent.name);
            if (parentBone == null)
            {
                Debug.LogWarning($"Missing bone:{bone.parent.name}£¬try to create fake bone");
                parentBone = CreateFakeBoneOnBody(bone.parent);
            }
            if(parentBone == null)
            {
                Debug.LogError($"failed, no parent bone: {bone.parent.name}");
                return null;
            }

            var go = new GameObject(bone.name);
            var bodyBone = go.transform;
            bodyBone.parent = parentBone;
            bodyBone.transform.localPosition = bone.localPosition;
            bodyBone.transform.localScale = bone.localScale;
            bodyBone.transform.localRotation = bone.localRotation;
            Debug.LogWarning($"Create fake bone success {bone.name}");
            return bodyBone;
        }
    }
}