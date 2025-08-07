using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore.Unity
{
    public class CameraShakeManager : SingletonMonoBehaviour<CameraShakeManager>
    {
        private CameraShakeAsset processingShake;
        private float timer;
        private Vector3 orgPos;
        private Quaternion orgRotation;
        private float seed;

        public void Play(CameraShakeAsset shake)
        {
            if (processingShake != null)
            {
                Debug.Log("CameraShakeManager: I am busy.");
                return;
            }

            processingShake = shake;
            timer = 0;
            orgPos = transform.localPosition;
            orgRotation = transform.localRotation;
            seed = Random.Range(0, 10000);
        }

        private void Update()
        {
            if (processingShake == null)
            {
                return;
            }

            if (timer > processingShake.duration)
            {
                processingShake = null;
                transform.localPosition = orgPos;
                return;
            }

            timer += Time.deltaTime;
            var f = processingShake.decayCurve.Evaluate(timer / processingShake.duration);
            var shake = Vector3.zero;
            if (processingShake.positionIntensity.x != 0 || processingShake.rotationIntensity.x != 0)
            {
                shake.x = (Mathf.PerlinNoise(seed + Time.time * processingShake.frequency, 0) * 2 - 1) * f;
            }

            if (processingShake.positionIntensity.y != 0 || processingShake.rotationIntensity.y != 0)
            {
                shake.y = (Mathf.PerlinNoise(0, seed + Time.time * processingShake.frequency) * 2 - 1) * f;
            }

            if (processingShake.positionIntensity.z != 0 || processingShake.rotationIntensity.z != 0)
            {
                shake.z = (Mathf.PerlinNoise(seed + 100 + Time.time * processingShake.frequency, seed + 200) * 2 - 1) * f;
            }

            if (processingShake.positionIntensity != Vector3.zero)
            {
                transform.localPosition = orgPos + Vector3.Scale(shake, processingShake.positionIntensity);
            }

            if (processingShake.rotationIntensity != Vector3.zero)
            {
                transform.localRotation = orgRotation * Quaternion.Euler(Vector3.Scale(shake, processingShake.rotationIntensity));
            }
        }
    }
}
