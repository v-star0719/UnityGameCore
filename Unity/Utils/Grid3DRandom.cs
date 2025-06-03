using GameCore.Core;
using UnityEngine;

namespace GameCore.Unity
{
    //将子节点按网格排列在3D空间中，每个节点占用一个格子，节点放在格子的随机位置里
    [ExecuteAlways]
    public class Grid3DRandom : MonoBehaviour
    {
        public int randomSeed;
        public Vector3 cellSize;
        public Vector3 columnGap;
        public Vector3 lineGap;
        public int itemPerLine;
        public bool reposition;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (reposition)
            {
#if !UNITY_EDITOR
                reposition = false;
#endif
                Reposition();
            }
        }

        private void Reposition()
        {
            var random = new FloatRandom(randomSeed);
            var itemLimit = itemPerLine <= 0 ? int.MaxValue : itemPerLine;
            Vector3 lineStart = Vector3.zero;
            Vector3 curPos = lineStart;
            int count = transform.childCount;
            int col = 0;
            for (int i = 0; i < count; i++)
            {
                var trans = transform.GetChild(i);
                var pos = new Vector3(
                    curPos.x + random.Range(0, cellSize.x) - cellSize.x * 0.5f,
                    curPos.y + random.Range(0, cellSize.y) - cellSize.y * 0.5f,
                    curPos.z + random.Range(0, cellSize.z) - cellSize.z * 0.5f
                    );
                trans.localPosition = pos;

                col++;
                if (col == itemLimit)
                {
                    col = 0;
                    lineStart = lineStart + lineGap;
                    curPos = lineStart;
                }
                else
                {
                    curPos = curPos + columnGap;
                }
            }
        }
    }
}