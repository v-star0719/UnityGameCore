using System.Collections;
using UnityEngine;

namespace GameCore.Unity.CameraShake
{
    public class CameraShake : MonoBehaviour
    {
        public CameraShakeAsset asset;
        public float delay;

        private void OnEnable()
        {
            if (asset != null)
            {
                StartCoroutine(SendEvent());
            }
        }

        private IEnumerator SendEvent()
        {
            yield return new WaitForSeconds(delay);
            if (CameraShakeManager.Inst == null)
            {
                Debug.LogError("CameraShakeManager is not running");
                yield break;
            }
            CameraShakeManager.Inst.Play(asset);
        }
    }
}
