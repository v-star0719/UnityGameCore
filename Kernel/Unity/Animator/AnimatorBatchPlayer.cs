using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kernel.Unity
{
//随机播放动画序列
    public class AnimatorBatchPlayer : MonoBehaviour
    {
        [Serializable]
        public class ActionInfo
        {
            public string name;
            public ActionNodeInfo[] nodes;
        }

        [Serializable]
        public class ActionNodeInfo
        {
            public AnimationClip clip;
            public float durationMax; //持续时间，-1表示动画时长
            public float durationMin;
        }

        public ActionInfo[] actions;

        private Animator animator;
        private int acitonIndex;
        private int nodeIndex;
        private int nodeDuration;
        private float timer;

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            Play(Random.Range(0, actions.Length));
        }

        void Update()
        {
            var action = actions[acitonIndex];
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                nodeIndex++;
                if (nodeIndex >= action.nodes.Length)
                {
                    Play(Random.Range(0, actions.Length));
                }
                else
                {
                    PlayNode(nodeIndex);
                }
            }
        }

        private void Play(int actionIndex)
        {
            this.acitonIndex = actionIndex;
            nodeIndex = 0;
            timer = 0;
            PlayNode(nodeIndex);
        }

        private void PlayNode(int nodeIndex)
        {
            var node = actions[acitonIndex].nodes[nodeIndex];
            animator.CrossFade(node.clip.name, 0.2f);
            if (node.durationMax == 0 && node.durationMin == 0)
            {
                timer = node.clip.length;
            }
            else if (node.durationMax < 0 && node.durationMin < 0)
            {
                timer = float.MaxValue;
            }
            else
            {
                timer = Random.Range(node.durationMin, node.durationMax);
            }
        }
    }
}