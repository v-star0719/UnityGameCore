using System.Collections.Generic;
using GameCore.Unity;
using UnityEngine;

namespace Kernel.Unity
{
    public class AvatarWearableControllerBase : MonoBehaviour
    {
        public enum FollowMode
        {
            UseTargetBone, //直接使用目标骨骼
            FollowTargetBoneAsChild,//跟随目标骨骼：作为目标骨骼的子节点
            FollowTargetBone, //跟随目标骨骼：直接设置位置旋转缩放
        }
        protected class BonePair
        {
            public Transform myBone;
            public Transform bodyBone;
        }

        public SkinnedMeshRenderer body;
        public FollowMode followMode = FollowMode.UseTargetBone;

        protected List<AvatarWearableBase> wearables = new();
        protected Dictionary<SkinnedMeshRenderer, List<BonePair>> bonePairsMap = new();

        public virtual void AddWearable(AvatarWearableBase wearable)
        {
            wearables.Add(wearable);
            GameObjectUtils.SetLayer(wearable.transform, gameObject.layer);
            wearable.transform.SetParent(transform, false);
            foreach(var skin in wearable.skins)
            {
                CombineSkinnedMesh(skin);
            }

            foreach(var skin in wearable.meshSkins)
            {
                CombineMesh(skin.meshRenderer, skin.targetBone);
            }
        }

        public virtual void RemoveWearable(AvatarWearableBase wearable)
        {
            wearables.Remove(wearable);
            foreach(var sr in new List<SkinnedMeshRenderer>(bonePairsMap.Keys))
            {
                if(sr.transform.IsChildOf(wearable.transform))
                {
                    bonePairsMap.Remove(sr);
                }
            }
            Destroy(wearable.gameObject);
        }

        public virtual void ChangeBody(SkinnedMeshRenderer newBody)
        {
            body = newBody;
            foreach(var wearable in wearables)
            {
                foreach(var skin in wearable.skins)
                {
                    CombineSkinnedMesh(skin);
                }
            }
        }

        public virtual void CombineSkinnedMesh(SkinnedMeshRenderer renderer)
        {
            if(followMode == FollowMode.UseTargetBone)
            {
                List<Transform> bones = new List<Transform>();
                foreach(var clothBone in renderer.bones)
                {
                    var bodyBone = FindBoneOnBody(clothBone.name);
                    if(bodyBone == null)
                    {
                        bodyBone = CreateFakeBoneOnBody(clothBone);
                        if(bodyBone == null)
                        {
                            continue;
                        }
                    }
                    bones.Add(bodyBone);
                }
                renderer.bones = bones.ToArray();
            }
            else
            {
                List<BonePair> bonePairs = null;
                if(followMode == FollowMode.FollowTargetBone)
                {
                    if(bonePairsMap.TryGetValue(renderer, out bonePairs))
                    {
                        bonePairs.Clear();
                    }
                    else
                    {
                        bonePairs = new List<BonePair>();
                        bonePairsMap.Add(renderer, bonePairs);
                    }
                }

                foreach(var clothBone in renderer.bones)
                {
                    //骨骼动态效果时，有些骨骼不希望拆开分散，破坏骨骼链
                    if (clothBone.tag == "StayWithParent")
                    {
                        continue;
                    }

                    var bodyBone = FindBoneOnBody(clothBone.name);
                    if(bodyBone == null)
                    {
                        bodyBone = CreateFakeBoneOnBody(clothBone);
                        if(bodyBone == null)
                        {
                            continue;
                        }
                    }
                    if(followMode == FollowMode.FollowTargetBoneAsChild)
                    {
                        TransformUtils.SetParent(clothBone, bodyBone);
                    }
                    else if(followMode == FollowMode.FollowTargetBone)
                    {
                        bonePairs.Add(new() { bodyBone = bodyBone, myBone = clothBone });
                    }
                }
            }
        }

        public void CombineMesh(GameObject mesh, string boneName)
        {
            var bone = FindBoneOnBody(boneName);
            if(bone == null)
            {
                Debug.LogError($"bone was not found: {boneName}");
                bone = transform;
            }
            TransformUtils.SetParent(mesh.transform, bone);
        }

        protected Transform FindBoneOnBody(string boneName)
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

        protected Transform CreateFakeBoneOnBody(Transform bone)
        {
            var parentBone = FindBoneOnBody(bone.parent.name);
            if(parentBone == null)
            {
                Debug.LogWarning($"Missing bone:{bone.parent.name}，try to create fake bone");
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

        protected virtual void LateUpdate()
        {
            foreach(var kv in bonePairsMap)
            {
                foreach(var bp in kv.Value)
                {
                    bp.myBone.position = bp.bodyBone.position;
                    bp.myBone.rotation = bp.bodyBone.rotation;
                }
            }
        }
    }
}