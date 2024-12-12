using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace GameCore.Unity
{
    //仅仅是支持反向播放而已
    //https://discussions.unity.com/t/reversing-timeline/817922/6
    public class PlayableDirectorEx : MonoBehaviour
    {
        private PlayableDirector _director;
        private float timer = 0;

        public PlayableDirector Director
        {
            get
            {
                if (_director == null)
                {
                    _director = GetComponent<PlayableDirector>();
                }

                return _director;
            }
        }

        public void PlayForward()
        {
            _director.Play();
        }

        public void PlayReverse()
        {
            timer = (float)Director.duration;
            Director.time = timer;
            Director.Play();
            Director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            enabled = true;
        }

        private void Update()
        {
            if (timer <= 0)
            {
                enabled = false;
                return;
            }

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                enabled = false;
            }
            Director.time = timer;
            Director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        }
    }
}