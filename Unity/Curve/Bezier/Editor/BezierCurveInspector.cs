using UnityEditor;

namespace GameCore.Unity.Curve
{
    [CustomEditor(typeof(BezierCurveEdit))]
    public class BezierCurveInspector : CurveEditInspectorBase<BezierCurveData>
    {
    }
}