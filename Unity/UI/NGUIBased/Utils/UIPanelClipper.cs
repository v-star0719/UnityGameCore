using UnityEngine;

#if NGUI

namespace GameCore.Unity.NGUIEx
{
    [RequireComponent(typeof(UIPanel))]
    public class UIPanelClipper : MonoBehaviour
    {
        private static readonly int _CLIP_AREA_ID = Shader.PropertyToID("_UIClipArea");

        public static void InitClipArea()
        {
            Shader.SetGlobalVector(_CLIP_AREA_ID, new Vector4(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue));
        }

        UIPanel targetPanel = null;
        Vector4 clipArea;


        Vector4 CalcClipArea()
        {
            if(targetPanel == null)
            {
                targetPanel = GetComponent<UIPanel>();

                var clipRegion = targetPanel.finalClipRegion;
                Vector4 nguiArea = new Vector4()
                {
                    x = clipRegion.x - clipRegion.z / 2,
                    y = clipRegion.y - clipRegion.w / 2,
                    z = clipRegion.x + clipRegion.z / 2,
                    w = clipRegion.y + clipRegion.w / 2
                };


                Vector3 worldClipXY = targetPanel.transform.TransformPoint(nguiArea.x, nguiArea.y, 0);
                Vector3 worldClipZW = targetPanel.transform.TransformPoint(nguiArea.z, nguiArea.w, 0);

                clipArea = new Vector4()
                {
                    x = worldClipXY.x,
                    y = worldClipXY.y,
                    z = worldClipZW.x,
                    w = worldClipZW.y
                };
            }

            return clipArea;
        }

        private void OnEnable()
        {
            Shader.SetGlobalVector(_CLIP_AREA_ID, CalcClipArea());
        }

        private void OnDisable()
        {
            Shader.SetGlobalVector(_CLIP_AREA_ID, new Vector4(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue));
        }
    }

}
#endif
