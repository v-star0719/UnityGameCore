using UnityEngine;

namespace Curves
{
    //两个节点中间的两个点是控制点。节点列表为：节点，控制点，控制点，节点，控制点，控制点，节点。。。以此类推
    [ExecuteInEditMode]
    public class BezierCombineCurveEdit : CurveEditBase<BezierCombineCurveData>
    {
        protected override void OnUpdate()
        {
            //todo 对称的两个控制点，移动其中一个，另一个也跟着动
            //限制相邻两个控制点和顶点必须位于一条直线上，这样可以方便的衔接两段曲线
            //A---O----B O是顶点，A、B是控制点，将AOB限制在一条直线上。
            for (int i = 2; i < posList.Count && i + 2 < posList.Count; i += 3)
            {
                var dirAO = posList[i + 1] - posList[i];
                var dirOB = posList[i + 2] - posList[i + 1];
                var d = Mathf.Abs(Vector3.Dot(dirAO, dirOB) / dirAO.magnitude); //OB在AO上的投影
                transform.GetChild(i + 2).localPosition = posList[i + 1] + dirAO.normalized * d;
            }
        }

        protected override void OnSetMark(int i, CurvePointMark mark)
        {
            var isControl = i % 3 != 0;
            mark.shape = isControl ? PrimitiveType.Cube : PrimitiveType.Sphere;
            mark.name = isControl ? i.ToString() + "_c" : i.ToString();
        }
    }
}
