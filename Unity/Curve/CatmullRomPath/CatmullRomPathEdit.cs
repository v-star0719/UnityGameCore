using UnityEngine;

namespace GameCore.Unity.Curve
{
    [ExecuteInEditMode]
    public class CatmullRomPathEdit : CurveEditBase<CatmullRomPathData>
    {
        protected override void OnUpdate()
        {
            CatmullRomPathData.GenerateControlPoints(posList);
        }
    }
}
