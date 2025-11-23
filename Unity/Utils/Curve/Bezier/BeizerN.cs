using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    [ExecuteInEditMode]
    class BeizerN : MonoBehaviour
    {
        public int d;
        public bool logBtn;
        public float s;
        public float e;
        public List<Vector3> posList = new List<Vector3>();

        void Update()
        {
            posList.Clear();
            //posList.Add(Vector3.zero);
            for (int i = 0; i < transform.childCount; i++)
            {
                posList.Add(transform.GetChild(i).localPosition);
            }

            //posList[0] = 2 * posList[1] - posList[2];
            //posList.Add(2 * posList[posList.Count - 1] - posList[posList.Count - 2]);
        }

        void OnDrawGizmos()
        {
            Vector3 lastPos = Vector3.zero;
            for (float i = s; i <= e; i += 0.01f)
            {
                Vector3 pos = transform.TransformPoint(GetPos(i));
                if (i > s)
                {
                    Gizmos.DrawLine(lastPos, pos);
                }
                lastPos = pos;
                logBtn = false;
            }
        }

        Vector3 GetPos(float f)
        {
            
            //通用公式
            //var count = transform.childCount;
            //var n = count - 1;
            //Vector3 pos = Vector3.zero;
            //for (int i = 0; i < count; i++)
            //{
            //    pos += Combination(n, i) * Mathf.Pow(1 - f, n - i) * Mathf.Pow(f, i) *
            //           transform.GetChild(i).localPosition;
            //}
            //return pos;

            //两两差值
            //var count = transform.childCount;
            //var points = new Vector3[count];
            //for (int i = 0; i < count; i++)
            //{
            //    points[i] = transform.GetChild(i).localPosition;
            //}
            //for (int i = 0; i <= count - 2; i++)
            //{
            //    for (int j = 0; j < count - i - 1; j++)
            //    {
            //        points[j] = Vector3.Lerp(points[j], points[j + 1], f);
            //    }
            //}
            //return points[0];

            var count = posList.Count;
            Vector3 pos = Vector3.zero;
            var uArray = new List<float>
            {
                0,0,0,0,0,6,7,8,9,10,
                11,11,11,11,11
            };
            //var uArray = new float[count + d];
            //Debug.Log("=====================");
            //for (int i = 0; i < uArray.Length; i++)
            //{
            //    uArray[i] = i;
            //}
            for (int i = 0; i < count; i++)
            {
                if (logBtn)
                {
                    Debug.Log("============================= " + i);
                }
                pos += MathUtils.Cox_deBoor(i, d, f, uArray) * posList[i]; //Combination(n, i) * 
            }

            return pos;
        }

        long Combination(int n, int c)
        {
            long a = 1;
            long b = 1;
            for (int i = c; i > 0; i--)
            {
                a *= n;
                b *= c;
                n--;
                c--;
            }
            return a / b;
        }

        
    }

}
