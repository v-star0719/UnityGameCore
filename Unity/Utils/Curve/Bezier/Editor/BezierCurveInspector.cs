using UnityEditor;

namespace Curves
{
    [CustomEditor(typeof(BezierCurveEdit))]
    public class BezierCurveInspector : CurveEditInspectorBase<BezierCurveData>
    {
    }
}