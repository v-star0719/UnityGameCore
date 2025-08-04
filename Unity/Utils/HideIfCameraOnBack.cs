using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Unity
{
    public class HideIfCameraOnBack : MonoBehaviour
    {
        public GameObject obj;
        public List<GameObject> objs = new ();
        public float maxCamDist = 100;

        // Update is called once per frame
        void Update()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                return;
            }

            var dir = cam.transform.position - transform.position;
            if (dir.sqrMagnitude > maxCamDist * maxCamDist)
            {
                return;
            }

            var d = Vector3.Dot(dir, transform.forward);
            foreach (var o in objs)
            {
                o.SetActive(d > 0);
            }
        }
    }
}
