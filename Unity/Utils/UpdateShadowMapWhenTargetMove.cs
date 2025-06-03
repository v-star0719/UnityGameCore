using UnityEngine;

#if HDRP
using UnityEngine.Rendering.HighDefinition;

namespace GameCore.Unity
{
    public class UpdateShadowMapWhenTargetMove : MonoBehaviour
    {
        public Transform target;

        private Vector3 pos = new Vector3(float.MaxValue, 0, 0);
        private Quaternion rotation;
        private HDAdditionalLightData lightData;

        // Start is called before the first frame update
        void Start()
        {
            lightData = GetComponent<HDAdditionalLightData>();
        }

        // Update is called once per frame
        void Update()
        {
            if (pos != target.position || rotation != target.rotation)
            {
                pos = target.position;
                rotation = target.rotation;
                lightData.RequestShadowMapRendering();
            }
        }
    }
}

#endif
