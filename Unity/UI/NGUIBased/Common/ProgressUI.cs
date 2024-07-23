using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UI
{
    [ExecuteInEditMode]
    public class ProgressUI : MonoBehaviour
    {
        public UIWidget widget;
        public bool isHorz;

        [SerializeField][HideInInspector]
        private float __value = 1;

        public float Value
        {
            get
            {
                return __value;
            }
            set
            {
                __value = Mathf.Clamp01(value);
                widget.transform.localScale = new Vector3(
                    isHorz ? value : 1,
                    isHorz ? 1 : value,
                    1);
            }
        }
    }
}
