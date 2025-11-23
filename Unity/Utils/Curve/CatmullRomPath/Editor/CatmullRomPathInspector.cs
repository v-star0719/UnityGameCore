using UnityEditor;

namespace Curves
{
    [CustomEditor(typeof(CatmullRomPathEdit))]
    public class CatmullRomPathInspector : CurveEditInspectorBase<CatmullRomPathData>
    {
    }
}
