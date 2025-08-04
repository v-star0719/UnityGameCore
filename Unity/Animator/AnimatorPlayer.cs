using System;
using UnityEngine;

namespace GameCore.Unity
{
    public class AnimatorPlayer : MonoBehaviour
    {
        public Animator animator;
        public float Timer { get; private set; }
        private float duration;
        private Action onFinish;
        public string StateName { get; private set; }
        private bool isLoop;
        private bool isRootMotion;

        private void Start()
        {
        }

        private void Update()
        {
            if (duration < 0)
            {
                duration--;
                if (duration < -2 && !animator.IsInTransition(0)) //等一帧，overidde的时候可能还是上个剪辑的时间
                {
                    var s = animator.GetCurrentAnimatorStateInfo(0);
                    if(s.IsName(StateName))
                    {
                        duration = s.length;
                        isLoop = s.loop;
                    }
                }
            }
            else
            {
                Timer += Time.deltaTime;
                if (Timer >= duration)
                {
                    if (isLoop)
                    {
                        Timer -= duration;
                    }
                    else
                    {
                        Stop();
                    }
                }
            }
        }

        public void Play(string stateName, float crossFade = 0.2f, bool rootMotion = false, Action onFinish = null)
        {
            this.StateName = stateName;
            enabled = true;
            this.onFinish = onFinish;

            if (rootMotion != animator.applyRootMotion)
            {
                isRootMotion = animator.applyRootMotion;
                animator.applyRootMotion = rootMotion;
                if (!rootMotion)
                {
                    animator.transform.localPosition = Vector3.zero;
                    animator.transform.localRotation = Quaternion.identity;
                }
            }

            if (crossFade > 0)
            {
                animator.CrossFade(stateName, crossFade);
            }
            else
            {
                animator.Play(stateName);
            }
            duration = -1;
            Timer = 0;
        }

        public void Pause()
        {
            animator.speed = 0;
        }

        public void Resume()
        {
            animator.speed = 1;
        }

        public void Stop()
        {
            duration = -1;
            enabled = false;
            onFinish?.Invoke();
            animator.applyRootMotion = isRootMotion;
        }
    }
}
