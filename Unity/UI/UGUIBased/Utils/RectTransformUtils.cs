using GameCore.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public static class RectTransformUtils
    {
        //https://discussions.unity.com/t/how-to-find-out-the-rect-of-a-group-of-ui-objects/933245
        public static void CalculateBoundingRect(List<RectTransform> rectTransforms, out Vector3 min, out Vector3 max)
        {
            if(rectTransforms == null || rectTransforms.Count == 0)
            {
                min = Vector3.zero;
                max = Vector3.zero;
                return;
            }

            // Initialize min and max with the first rectTransform's corners
            Vector3[] corners = new Vector3[4];
            rectTransforms[0].GetWorldCorners(corners);
            min = corners[0];
            max = corners[0];

            // Iterate through each rectTransform
            foreach(RectTransform rectTransform in rectTransforms)
            {
                rectTransform.GetWorldCorners(corners);

                foreach(Vector3 corner in corners)
                {
                    min = Vector3.Min(min, corner);
                    max = Vector3.Max(max, corner);
                }
            }
        }

        public static Bounds CalculateBounding(RectTransform trans, Space space = Space.World)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue);
            CalculateBoundingRectInner(trans, ref min, ref max);
            if (space == Space.Self)
            {
                min = trans.InverseTransformPoint(min);
                max = trans.InverseTransformPoint(max);
            }
            return new Bounds((min + max) * 0.5f, max - min);
        }

        public static void SetWidth(this RectTransform trans, float w)
        {
            var sz = trans.sizeDelta;
            sz.x = w;
            trans.sizeDelta = sz;
        }

        public static void SetHeight(this RectTransform trans, float h)
        {
            var sz = trans.sizeDelta;
            sz.y = h;
            trans.sizeDelta = sz;
        }

        private static void CalculateBoundingRectInner(RectTransform trans, ref Vector3 min, ref Vector3 max)
        {
            if(trans == null)
            {
                min = Vector3.zero;
                max = Vector3.zero;
                return;
            }

            var corners = new Vector3[4];
            trans.GetWorldCorners(corners);
            foreach(Vector3 corner in corners)
            {
                min = Vector3.Min(min, corner);
                max = Vector3.Max(max, corner);
            }

            for (int i = 0; i < trans.childCount; i++)
            {
                CalculateBoundingRectInner(trans.GetChild(i) as RectTransform, ref min, ref max);
            }
        }
    }
}