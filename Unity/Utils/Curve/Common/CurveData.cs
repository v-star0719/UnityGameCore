using System;
using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    public class CurveData : ScriptableObject
    {
        public List<Vector3> points = new List<Vector3>();
        public int smooth = 1; //把路径分成多少段，越大越平滑

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ratio">0 -> 1</param>
        /// <returns></returns>
        public virtual Vector3 GetPos(float ratio)
        {
            throw new NotImplementedException();
        }

        public virtual List<Vector3> ToSegments()
        {
            throw new NotImplementedException();
        }

        public virtual void DrawGizmos(Color color, Matrix4x4 matrix)
        {
            DrawGizmos(color, matrix, points, smooth);
        }

        public virtual void DrawGizmos(Color color, Matrix4x4 matrix, List<Vector3> points, int smooth, params object[] args)
        {
        }
    }
}
