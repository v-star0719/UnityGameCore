using GameCore.Core;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    public class RectTransformUtils
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
    }
}