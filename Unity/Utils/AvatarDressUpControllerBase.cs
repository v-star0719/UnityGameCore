using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;

namespace Kernel.Unity
{
    public class AvatarDressUpControllerBase : MonoBehaviour
    {
        public SkinnedMeshRenderer body;
        public SkinnedMeshRenderer testObj;
        public bool test;

        public virtual void AddDressUp(GameObject go)
        {
            GameObjectUtils.SetLayer(go.transform, gameObject.layer);
            go.transform.SetParent(transform, false);
            CombineSkinnedMesh(go.GetComponent<AvatarDressUpBase>().skin);
        }

        public virtual void RemoveDressUp(GameObject go)
        {
            Destroy(go);
        }

        private void CombineSkinnedMesh(SkinnedMeshRenderer mesh)
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

        public void Update()
        {
            if (test)
            {
                test = false;
                TestBtn();
            }
        }

        public void TestBtn()
        {
            CombineSkinnedMesh(testObj);
        }
    }
}