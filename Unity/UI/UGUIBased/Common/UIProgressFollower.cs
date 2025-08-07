using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    ///进度条跟着另一个进度条变。实现的效果是：原进度先变化，另一个进度条从原位置变化到新位置
    public class UIProgressFollower : MonoBehaviour
    {
        [Header("My")] public Image myImage; //控制fillAmount
        public Slider MySlider; //控制Value
        [Header("Target")]
        public Image TargetImage;
        public Slider TargetSlider;
        [Header("Time")] public float Duration = 1;
        public AnimationCurve Curve;

        private float _curValue = 0;
        private float _targetValue = 0;
        private float _timer = 0;

        private float GetTargetValue()
        {
            if (TargetImage != null)
            {
                return TargetImage.fillAmount;
            }

            if (TargetSlider != null)
            {
                return TargetSlider.value;
            }

            return 0;
        }

        public void Reset()
        {
            SetCurValue(_targetValue);
        }

        public void Update()
        {
            var t = GetTargetValue();
            if (t != _targetValue)
            {
                _targetValue = t;
                _timer = 0;
            }

            if (_curValue != _targetValue)
            {
                _timer += Time.deltaTime;
                _curValue = Mathf.Lerp(_curValue, _targetValue, Curve.Evaluate(_timer / Duration));
                SetCurValue(_curValue);
            }
        }

        public void SetCurValue(float v)
        {
            _curValue = v;
            if (myImage != null)
            {
                myImage.fillAmount = v;
            }

            if (MySlider != null)
            {
                MySlider.value = v;
            }
        }
    }
}
