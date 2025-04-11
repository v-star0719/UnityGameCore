using UnityEngine;

namespace GameCore.Edit
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

        public Vector3 GetRandomPosInRect()
        {
            var p = new Vector3(Random.Range(0, size.x) - size.x * 0.5f,
                Random.Range(0, size.y) - size.y * 0.5f,
                Random.Range(0, size.z) - size.z * 0.5f);
            return transform.TransformPoint(p);
        }

        public Vector3 GetRandomPointInRound()
        {
            var r = Random.Range(0, radius);
            var angle = Random.Range(0, Mathf.PI * 2);
            var p = new Vector3(r * Mathf.Cos(angle), 0, r * Mathf.Sin(angle));
            return transform.TransformPoint(p);
        }
    }
}
