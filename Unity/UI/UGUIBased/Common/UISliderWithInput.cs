using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class UISliderWithInput : MonoBehaviour
    {
        public int decimalPlaces = 1;
        public Slider slider;
        public TMP_InputField input;
        public UnityEvent<float> onChange;

        public float Min
        {
            get => slider.minValue;
            set => slider.minValue = value;
        }

        public float Max
        {
            get => slider.maxValue;
            set => slider.maxValue = value;
        }

        public float Value
        {
            get => slider.value;
            set
            {
                slider.SetValueWithoutNotify(value);
                input.SetTextWithoutNotify(NumberToString());
            }
        }

        public float IntValue
        {
            get => (int)Value;
            set => Value = value;
        }

        private string NumberToString()
        {
            return slider.value.ToString("f" + decimalPlaces);
        }

        public void OnSliderChanged(float f)
        {
            input.SetTextWithoutNotify(NumberToString());
            onChange?.Invoke(Value);
        }

        public void OnInputChanged()
        {
            if(float.TryParse(input.text, out var number))
            {
                slider.SetValueWithoutNotify(number);
                onChange?.Invoke(Value);
            }
        }
    }
}
