using System.Collections;
using UnityEngine;

namespace GameCore.Unity
{
    //在一定范围内随机分布
    [ExecuteAlways]
    public class RandomDistribution : MonoBehaviour
    {
        public Vector3 range;
        public bool reposition;

        void Start()
        {

        }

        void Update()
        {
            if (reposition)
            {
                reposition = false;
                Reposition();
            }
        }

        private void Reposition()
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                var trans = transform.GetChild(i);
                trans.localPosition = new Vector3(
                    Random.Range(0, range.x) - range.x * 0.5f,
                    Random.Range(0, range.y) - range.y * 0.5f,
                    Random.Range(0, range.z) - range.z * 0.5f
                );
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, range);
        }
    }
}