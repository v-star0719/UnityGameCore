using System.Collections;
using System.Collections.Generic;
using GameCore.Unity;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    [CustomEditor(typeof(CameraShakeAsset))]
    public class CameraShakeAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var csa = target as CameraShakeAsset;

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("爆炸"))
                {
                    csa.positionIntensity = new Vector3(2, 2, 0);
                    csa.frequency = 20f;
                    csa.duration = 0.5f;
                }
                if(GUILayout.Button("撞击"))
                {
                    csa.positionIntensity = new Vector3(1, 1, 0);
                    csa.frequency = 15f;
                    csa.duration = 0.3f;
                }
                if(GUILayout.Button("轻微震动"))
                {
                    csa.positionIntensity = new Vector3(0.2f, 0.2f, 0);
                    csa.frequency = 10f;
                    csa.duration = 0.2f;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}

