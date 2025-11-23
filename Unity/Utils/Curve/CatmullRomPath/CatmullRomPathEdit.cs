using UnityEngine;

namespace Curves
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
