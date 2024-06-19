using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Kernel.Unity
{
    public class AnimatorPlayer : MonoBehaviour
    {
        public Animator animator;
        private float timer;
        private float duration;
        private Action onFinish;
        private string stateName;
        private bool applyRootMotion;

        private void Start()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
                applyRootMotion = animator.applyRootMotion;
            }
        }

        private void Update()
        {
            if (duration < 0)
            {
                var s = animator.GetCurrentAnimatorStateInfo(0);
                if (s.IsName(stateName))
                {
                    duration = s.length;
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= duration)
                {
                    enabled = false;
                    animator.applyRootMotion = applyRootMotion;
                    onFinish?.Invoke();
                }
            }
        }

        public void Play(string stateName, float crossFade = 0.2f, bool? rootMotion = null, Action onFinish = null)
        {
            this.stateName = stateName;
            enabled = true;
            this.onFinish = onFinish;

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
            timer = 0;
        }
    }
}
