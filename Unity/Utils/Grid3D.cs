﻿using UnityEngine;
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

        [FormerlySerializedAs("horzOffset")]
        public Vector3 columnGap;
        [FormerlySerializedAs("vertOffset")]
        public Vector3 lineGap;
        public Alignment alignment;

        public int itemPerLine;
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
            var itemLimit = itemPerLine <= 0 ? transform.childCount : itemPerLine;
            Vector3 lineStart = Vector3.zero;
            var columnGap = this.columnGap;
            if (alignment == Alignment.Right)
            {
                lineStart = (itemLimit - 1) * columnGap;
                columnGap =- this.columnGap;
            }
            else if (alignment == Alignment.Center)
            {
                lineStart = (1 - itemLimit) * 0.5f * columnGap;
            }

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