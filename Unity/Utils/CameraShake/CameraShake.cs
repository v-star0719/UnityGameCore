using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Unity
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
