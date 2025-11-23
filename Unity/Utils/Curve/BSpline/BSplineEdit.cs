using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    [ExecuteInEditMode]
    public class BSplineEdit : CurveEditBase<BSplineData>
    {
        public List<float> uList = new List<float>();
        public int polynomial = 3;
        
        protected override void OnUpdate()
        {
            uList.Clear();
            int n = posList.Count + polynomial + 1;
            if (n > 1)
            {
                float step = 1f / (n - 1);
                for (int i = 0; i < n; i++)
                {
                    if (i <= polynomial)
                    {
                        uList.Add(0);
                    }
                    else if (i >= posList.Count)
                    {
                        uList.Add(1);
                    }
                    else
                    {
                        uList.Add(i * step);
                    }
                }
            }
        }

        protected override void OnSave()
        {
            data.uList.Clear();
            data.uList.AddRange(uList);
            data.polynomial = polynomial;
        }

        protected override void OnDrawGizmos()
        {
            data?.DrawGizmos(Color.white, transform.localToWorldMatrix, posList, smooth, polynomial, uList);
        }
    }
}
