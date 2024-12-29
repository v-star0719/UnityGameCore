using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class SpriteAnimation : MonoBehaviour
    {
        public int framerate = 20;
        public bool ignoreTimeScale = true;
        public bool loop = true;
        public Sprite[] frames;
        public Image image;
        public bool playOnStart;

        private float mUpdate = 0f;
        private int frameIndex = 0;
        private float totalTime = 0;

        public void Play()
        {
            if (frames == null || frames.Length <= 0)
            {
                return;
            }

            //frameIndex = MathUtils.Clamp(0, frames.Length - 1);
            totalTime = frames.Length * 1f / framerate;
            enabled = true;
            UpdateSprite();
        }

        public void Pause()
        {
            enabled = false;
        }

        public void ResetToBeginning()
        {
            frameIndex = 0;
            UpdateSprite();
        }

        public void ResetToEnd()
        {
            frameIndex = frames.Length - 1;
            UpdateSprite();
        }

        public void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        public void Update()
        {
            if (frames == null || frames.Length == 0 || framerate == 0)
            {
                enabled = false;
                return;
            }

            mUpdate += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            if (!loop && mUpdate >= totalTime)
            {
                enabled = false;
                frameIndex = frames.Length - 1;
            }
            else
            {
                frameIndex = (int)(mUpdate * framerate) % frames.Length;
            }

            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (image == null)
            {
                enabled = false;
                return;
            }

            image.sprite = frames[frameIndex];
        }
    }
}