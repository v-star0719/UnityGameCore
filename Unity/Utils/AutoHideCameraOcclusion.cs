using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Unity
{
    //自动隐藏遮挡物
    public class AutoHideCameraOcclusion : MonoBehaviour
    {
        public Transform target;
        public Transform myCamera;

        private List<MeshRenderer> hiddenMeshRenderers = new List<MeshRenderer>();

        public void LateUpdate()
        {
            if (myCamera == null || target == null)
            {
                return;
            }

            var hit = Physics.RaycastAll(new Ray(myCamera.position, target.position - myCamera.position), 1000);
            List<MeshRenderer> hitMeshRenderers = new List<MeshRenderer>();
            foreach (var h in hit)
            {
                hitMeshRenderers.Add(h.transform.GetComponent<MeshRenderer>());
            }

            //新增的
            foreach (var r in hitMeshRenderers)
            {
                if(!hiddenMeshRenderers.Contains(r))
                {
                    hiddenMeshRenderers.Add(r);
                    r.enabled = false;
                }
            }

            //不存在的
            for (var i = hiddenMeshRenderers.Count - 1; i >= 0; i--)
            {
                var r = hiddenMeshRenderers[i];
                if (!hitMeshRenderers.Contains(r))
                {
                    hiddenMeshRenderers.RemoveAt(i);
                    r.enabled = true;
                }
            }
        }
    }
}