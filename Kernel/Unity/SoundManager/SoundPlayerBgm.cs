using System;

namespace Kernel.Unity
{
    //有一个播放列表，播放完了继续下一个，只有一个的话就循环播放那一个
    //支持播放列表
    //Enable的时候可以恢复播放
    public class SoundPlayerBgm : SoundPlayer
    {
        private string[] soundNames;//当前播放的是第一个。播放完成放到最后
        private float fadein;
        private float fadeout;

        public override bool SoundEnable
        {
            get => soundEnable;
            set
            {
                if(value == soundEnable)
                {
                    return;
                }

                soundEnable = value;
                if(soundEnable)
                {
                    base.Play(soundNames[0], false, volume, 0, fadein, fadeout);
                }
                else
                {
                    StopAllSounds();
                }
            }
        }

        public SoundPlayerBgm(SoundManager m, SoundLayer l) : base(m, l)
        {
        }

        public void Play(string[] names, float volume, float fadein = 0, float fadeout = 0)
        {
            StopAllSounds();
            soundNames = names;
            this.volume = volume;
            this.fadein = fadein;
            this.fadeout = fadeout;
            base.Play(names[0], false, volume, 0, fadein, fadeout);
        }

        public override SoundItemInfo Play(string name, bool loop, float volume, float duration, float fadein = 0, float fadeout = 0, int maxCount = 0)
        {
            throw new Exception("please use Play(string[] names, float volume, float fadein = 0, float fadeout = 0)");
        }

        protected override void OnSoundPlayFinished(SoundItem sound)
        {
            base.OnSoundPlayFinished(sound);

            //可能是上次播放的停止了，这时候不需要触发播放下一个
            if (!sound.IsStopped)
            {
                var last = soundNames.Length - 1;
                var t = soundNames[0];
                soundNames[0] = soundNames[last];
                soundNames[last] = t;
                base.Play(soundNames[0], false, volume, 0, fadein, fadeout);
            }
        }
    }
}
