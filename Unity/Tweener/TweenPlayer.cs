using System;
using UnityEngine;

namespace GameCore.Unity.Tweener
{
    public class TweenPlayer : MonoBehaviour
    {
        private Tweener[] _tweeners;
        private int finishedCount = 0;
        private Action onFinish;

        private Tweener[] Tweeners
        {
            get
            {
                if(_tweeners == null)
                {
                    _tweeners = GetComponentsInChildren<Tweener>(true);
                }
                return _tweeners;
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

        private void OnATweenFinished()
        {
            finishedCount++;
            if (onFinish != null && finishedCount == _tweeners.Length)
            {
                onFinish();
            }
        }
    }
}