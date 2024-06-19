using System.Collections;
using UnityEngine;

namespace Kernel.Unity
{
    [ExecuteInEditMode]
    public class MirrorPlane : MonoBehaviour
    {
        public Camera cam;
        public Material material;
        public float renderTextureScale = 1;

        private RenderTexture renderTexture;

        private void Start()
        {
            Reset();
        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }
        }

        public void Reset()
        {
            Clear();
            if (cam != null && material != null)
            {
                renderTexture = new RenderTexture((int)(Screen.width * renderTextureScale),
                    (int)(Screen.height * renderTextureScale), 32);
                Debug.Log($"{renderTexture.width} {renderTexture.height} {Screen.width} {Screen.height}");
                cam.targetTexture = renderTexture;
                material.SetTexture("_BaseMap", renderTexture);
            }
        }
    }
}