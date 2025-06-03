using UnityEngine;
using UnityEngine.Playables;

namespace GameCore.Unity
{
    //仅仅是支持反向播放而已
    //https://discussions.unity.com/t/reversing-timeline/817922/6
    public class PlayableDirectorEx : MonoBehaviour
    {
        private PlayableDirector _director;
        private double timer = 0;
        private bool reverse = false;

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
            Director.enabled = false;
            timer = 0;
            enabled = true;
            reverse = false;
        }

        public void PlayReverse()
        {
            Director.enabled = false;
            timer = Director.duration;
            enabled = true;
            reverse = true;
        }

        private void Update()
        {
            if (reverse)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    timer = 0;
                    enabled = false;
                }
            }
            else
            {
                timer += Time.deltaTime;
                if(timer >= Director.duration)
                {
                    timer = Director.duration;
                    enabled = false;
                }
            }
            
            Director.time = timer;
            Director.Evaluate();
        }

        public void SetToBegin()
        {
            Director.time = 0;
            Director.Evaluate();
            Director.enabled = false;
            enabled = false;
        }

        public void SetToEnd()
        {
            Director.time = Director.duration;
            Director.Evaluate();
            Director.enabled = false;
            enabled = false;
        }

        public void Play(bool forward)
        {
            if (forward)
            {
                PlayForward();
            }
            else
            {
                PlayReverse();
            }
        }
    }
}