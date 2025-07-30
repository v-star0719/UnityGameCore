using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Unity
{
    public class HideIfCameraOnBack : MonoBehaviour
    {
        public GameObject obj;
        // Update is called once per frame
        void Update()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                return;
            }

            var dir = cam.transform.position - transform.position;
            var d = Vector3.Dot(dir, transform.forward);
            obj.SetActive(d > 0);
        }
    }
}
