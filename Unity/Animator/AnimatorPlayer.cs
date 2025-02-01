using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCore.Unity
{
    public class AnimatorPlayer : MonoBehaviour
    {
        public Animator animator;
        public float Timer { get; private set; }
        private float duration;
        private Action onFinish;
        private string stateName;
        private bool applyRootMotion;
        private bool isLoop;

        private void Start()
        {
        }

        private void Update()
        {
            if (duration < 0)
            {
                var s = animator.GetCurrentAnimatorStateInfo(0);
                if (s.IsName(stateName))
                {
                    duration = s.length;
                    isLoop = s.loop;
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
                        enabled = false;
                        animator.applyRootMotion = applyRootMotion;
                        onFinish?.Invoke();
                    }
                }
            }
        }

        public void Play(string stateName, float crossFade = 0.2f, bool? rootMotion = null, Action onFinish = null)
        {
            this.stateName = stateName;
            enabled = true;
            this.onFinish = onFinish;

            applyRootMotion = animator.applyRootMotion;

            if (rootMotion != null)
            {
                animator.applyRootMotion = rootMotion.Value;
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
        }
    }
}
