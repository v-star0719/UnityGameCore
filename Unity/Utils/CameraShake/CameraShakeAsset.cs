using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace GameCore.Unity
{
    [CreateAssetMenu(
        fileName = "CameraShakeAsset",  // 默认文件名
        menuName = "ScriptableObjects/CameraShakeAsset",  // 菜单路径
        order = 1  // 菜单显示顺序
    )]
    public class CameraShakeAsset : ScriptableObject
    {
        [Header("位置强度")]
        public Vector3 positionIntensity = new Vector3(10, 10, 10);
        [Header("旋转强度")]
        public Vector3 rotationIntensity = new Vector3(0, 0, 10);
        [Header("时长")]
        public float duration = 0.3f;
        [Header("频率")]
        public float frequency = 15f;
        [Tooltip("衰减曲线")]
        public AnimationCurve decayCurve = AnimationCurve.Linear(1, 1, 0, 0);

        public static void CreateAsset()
        {
        }
    }
}