using UnityEngine;

namespace Kernel.Edit
{
    public class SceneMark : MonoBehaviour
    {
        public Color clr = Color.red;
        public Vector3 size = Vector3.one;
        public float radius;
        public Vector3 offset;
        [Tooltip("box或者sphere")]
        public PrimitiveType type;
        public bool wireFrame;
        public bool boxColliderSize;
        public bool showForward;
        public float forwardLineLength = 0.3f;

        public void OnDrawGizmos()
        {
            Gizmos.color = clr;
            Gizmos.matrix = transform.localToWorldMatrix;
            switch (type)
            {
                case PrimitiveType.Cube:
                    if (boxColliderSize)
                    {
                        var bc = GetComponent<BoxCollider>();
                        if (bc != null)
                        {
                            size = bc.size;
                        }
                    }

                    if (wireFrame)
                    {
                        Gizmos.DrawWireCube(offset, size);
                    }
                    else
                    {
                        Gizmos.DrawCube(offset, size);
                    }
                    break;
                case PrimitiveType.Sphere:
                    if (wireFrame)
                    {
                        Gizmos.DrawWireSphere(offset, radius);
                    }
                    else
                    {
                        Gizmos.DrawSphere(offset, radius);
                    }
                    break;
            }

            if (showForward)
            {
                Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0, forwardLineLength));
            }
        }
    }
}
