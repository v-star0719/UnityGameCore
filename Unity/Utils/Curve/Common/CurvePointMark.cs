using UnityEngine;

namespace Curves
{
    public class CurvePointMark : MonoBehaviour
    {
        public PrimitiveType shape = PrimitiveType.Sphere;
        public float size;
        // Start is called before the first frame update
        void Start()
        {
        }

        public void OnDrawGizmos()
        {
            switch (shape)
            {
                case PrimitiveType.Cube:
                    Gizmos.DrawCube(transform.position, new Vector3(size, size, size));
                    break;
                case PrimitiveType.Sphere:
                    Gizmos.DrawSphere(transform.position, size);
                    break;
            }
        }
    }
}
