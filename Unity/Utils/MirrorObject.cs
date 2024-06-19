using System.Collections;
using UnityEngine;

namespace Kernel.Unity
{
    [ExecuteInEditMode]
    public class MirrorObject : MonoBehaviour
    {
        public enum MirrorPlaneType
        {
            XZ,
            XY,
            YZ,
        }

        public MirrorPlaneType planeType;
        public bool alwaysUpdate;
        public Transform target;
        public float planePos;

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR
            if (!runInEditMode)
            {
                enabled = alwaysUpdate;
                Update();
            }
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null)
            {
                return;
            }

            var pos = target.position;
            var eulerAngles = target.eulerAngles;
            switch (planeType)
            {
                case MirrorPlaneType.XZ:
                    pos.y = planePos * 2 - pos.y;
                    eulerAngles.x = -eulerAngles.x;
                    break;
                case MirrorPlaneType.XY:
                    pos.z = planePos * 2 - pos.z;
                    eulerAngles.y = -eulerAngles.y;
                    break;
                case MirrorPlaneType.YZ:
                    pos.x = planePos * 2 - pos.x;
                    eulerAngles.z = -eulerAngles.z;
                    break;
            }

            transform.position = pos;
            transform.eulerAngles = eulerAngles;
        }
    }
}