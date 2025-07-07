using GameCore.Unity.UGUIEx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.Tweener
{
    public class TweenAlpha : Tweener
    {
        public float from;
        public float to = 1;

        public CanvasGroup canvasGroup;
        public Image image;
        public TMP_Text text;
        public bool disableCanvasGroupRaycastIfZero;

        protected override void OnUpdate(float factor)
        {
            var p = Mathf.Lerp(from, to, factor);
            if(canvasGroup != null)
            {
                canvasGroup.alpha = p;
                if (disableCanvasGroupRaycastIfZero)
                {
                    canvasGroup.blocksRaycasts = p > 0;
                    canvasGroup.interactable = p > 0;
                }
            }

            if(image != null)
            {
                image.SetAlpha(p);
            }

            if(text != null)
            {
                text.alpha = p;
            }
        }
    }
}