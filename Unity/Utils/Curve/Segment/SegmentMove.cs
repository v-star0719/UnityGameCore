using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    public class SegmentMove
    {
        public List<Vector3> points = new List<Vector3>();
        public List<float> distToStartList = new List<float>();//0点到当前点的距离
        public Vector3 Pos { get; private set; } //当前所在位置
        private int index; //从0到points.count-1，距离当前点最近的顶点。
        private float distToStart; //起点到当前点的距离

        public bool AtBeginning
        {
            get
            {
                return index == 0 && distToStart <= 0;
            }
        }

        public bool AtEnd
        {
            get
            {
                return index == distToStartList.Count - 1 && distToStart >= distToStartList[distToStartList.Count - 1];
            }
        }

        public SegmentMove(CurveData curveData) : this(curveData.ToSegments(), null)
        {
        }

        public SegmentMove(List<Vector3> points, List<float> distToStartList)
        {
            this.points = points;
            if (distToStartList == null)
            {
                CalculateDistToStart();
            }
            else
            {
                this.distToStartList = distToStartList;
            }

            var count = points.Count;
            if (count == 0)
            {
                Debug.LogWarning("segment path has no points");
                return;
            }
            ResetToBeginning();
        }

        //返回值：true = 已经移动到头了，false = 没有移动到头
        public void MoveForward(float delta)
        {
            var pointCount = points.Count;
            if (index >= points.Count - 1)
            {
                return;
            }

            distToStart += delta;
            if (distToStart >= distToStartList[pointCount - 1])
            {
                Pos = points[points.Count - 1];
                distToStart = distToStartList[pointCount - 1];
                index = pointCount - 1;
                return;
            }

            while (distToStartList[index + 1] < distToStart)
            {
                index++;
            }

            Pos = CalculatePos(index, distToStart);
        }

        //返回值：true = 已经移动到头了，false = 没有移动到头
        public void MoveBackward(float delta)
        {
            if (index <= 0)
            {
                return;
            }

            distToStart -= delta;
            if (distToStart <= 0)
            {
                Pos = points[0];
                distToStart = 0;
                index = 0;
                return;
            }

            while (distToStartList[index] > distToStart)
            {
                index--;
            }

            Pos = CalculatePos(index, distToStart);
        }

        private Vector3 CalculatePos(int index, float distToStart)
        {
            var f = (distToStart - distToStartList[index]) /
                    (distToStartList[index + 1] - distToStartList[index]);
            return Vector3.Lerp(points[index], points[index + 1], f);
        }

        public void ResetToBeginning()
        {
            Pos = points[0];
            distToStart = 0;
            index = 0;
        }

        public void ResetToEnd()
        {
            var count = points.Count;
            Pos = points[count - 1];
            distToStart = distToStartList[count - 1];
            index = 0;
        }

        public void CalculateDistToStart()
        {
            distToStartList.Clear();
            var totalDistance = 0f;
            if (points.Count > 0)
            {
                distToStartList.Add(0);
            }
            for (int i = 1; i < points.Count; i++)
            {
                totalDistance += Vector3.Distance(points[i], points[i - 1]);
                distToStartList.Add(totalDistance);
            }
        }

        public Vector3 GetPosByDistance(float d)
        {
            int i = 0;
            int j = distToStartList.Count - 1;
            while (j - i > 1)
            {
                var center = (i + j) >> 1;
                var t = distToStartList[center];
                if (t < d)
                {
                    i = center;
                }
                else
                {
                    j = center;
                }
            }

            var f = (d - distToStartList[i]) /
                    (distToStartList[j] - distToStartList[i]);
            return Vector3.Lerp(points[i], points[j], f);
        }
    }

    //[Serializable, ExecuteInEditMode]
    //public class SegmentPathDataTest : MonoBehaviour
    //{
    //    public List<Vector3> points = new List<Vector3>();
    //    public int smooth;
    //    public GameObject testGo;
    //    public float testSpeed;
    //    public bool isTestting;

    //    private SegmentMove move;

    //    void Start()
    //    {
    //        move = new SegmentMove(this, false);
    //    }


    //    void Update()
    //    {
    //        points.Clear();
    //        for (int i = 0; i < transform.childCount; i++)
    //        {
    //            points.Add(transform.GetChild(i).localPosition);
    //        }
    //        CalculateDistToStart();

    //        if (isTestting)
    //        {
    //            //move.MoveBackward(testSpeed * Time.deltaTime);
    //            //testGo.transform.localPosition = move.Pos;
    //            //if (move.AtBeginning)
    //            //{
    //            //    move.ResetToEnd();
    //            //}
    //            testGo.transform.localPosition = GetPosByDistance(testSpeed);
    //        }
    //    }
    //}
}
