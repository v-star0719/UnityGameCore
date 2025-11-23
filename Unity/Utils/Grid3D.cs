using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCore.Unity
{
    //将子节点按行列网格排列在3D空间中
    [ExecuteAlways]
    public class Grid3D : MonoBehaviour
    {
        public enum Alignment
        {
            Left,
            Center,
            Right,
        }

        public Vector3 horizontalSize;
        public Vector3 verticalSize;

        public Vector3 horizontalGap;
        public Vector3 verticalGap;
        public Vector2 pivot;

        public int horizontalMaxCount;
        public bool reposition;
        public bool hideInvisible;

        // Start is called before the first frame update
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
            var childCount = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (hideInvisible && !transform.GetChild(i).gameObject.activeSelf)
                {
                    continue;
                }
                childCount++;
            }
            var horizontalCount = horizontalMaxCount <= 0 ? transform.childCount : horizontalMaxCount;
            if (horizontalCount > childCount)
            {
                horizontalCount = childCount;
            }
            var verticalCount = ((childCount - 1) / horizontalCount) + 1;

            var horizontalDelta = horizontalGap + horizontalSize;
            var verticalDelta = verticalGap + verticalSize;
            var width = (horizontalCount - 1) * horizontalGap + horizontalCount * horizontalSize;
            var height = (verticalCount - 1) * verticalGap + verticalCount * verticalSize;

            Vector3 lineStart = Vector3.zero - (width - horizontalSize) * pivot.x  - (height - verticalSize) * pivot.y;
            Vector3 curPos = lineStart;
            int count = transform.childCount;
            int col = 0;
            for (int i = 0; i < count; i++)
            {
                var trans = transform.GetChild(i);
                if (hideInvisible && !trans.gameObject.activeSelf)
                {
                    continue;
                }

                trans.localPosition = curPos;

                col++;
                if (col == horizontalCount)
                {
                    col = 0;
                    lineStart += verticalDelta;
                    curPos = lineStart;
                }
                else
                {
                    curPos += horizontalDelta;
                }
            }
        }
    }
}