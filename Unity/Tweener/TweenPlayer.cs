using System;
using UnityEngine;

namespace GameCore.Unity.Tweener
{
    public class TweenPlayer : MonoBehaviour
    {
        public Tweener[] tweeners;
        private int finishedCount = 0;
        private Action onFinish;
        private bool noTweeners;

        private Tweener[] Tweeners
        {
            get
            {
                if(tweeners == null || (tweeners.Length == 0 && !noTweeners))
                {
                    tweeners = GetComponentsInChildren<Tweener>(true);
                    noTweeners = tweeners.Length == 0;
                }
                return tweeners;
            }
        }

        public void Play(bool forward, Action onFinish = null)
        {
            finishedCount = 0;
            this.onFinish = onFinish;
            foreach (Tweener t in Tweeners)
            {
                t.Play(forward, OnATweenFinished);
            }
        }

        public void Reset(bool beginning)
        {
            foreach (var t in Tweeners)
            {
                if (beginning)
                {
                    t.ResetToBeginning();
                }
                else
                {
                    t.ResetToEnd();
                }
            }
        }

        private void OnATweenFinished()
        {
            finishedCount++;
            if (onFinish != null && finishedCount == Tweeners.Length)
            {
                onFinish();
            }
        }
    }
}