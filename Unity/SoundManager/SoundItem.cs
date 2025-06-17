using System;
using UnityEngine;
namespace GameCore.Unity
{
    //SoundItem会被回收复用，通过SoundItemInfo进行长时间引用。
    //SoundItemInfo中有一个播放完成的回调
    //声音播放不会受TimeScale控制，所以用Time.time计时
    //AudioSource.time会自动回到0,不能直接用
    //
    //初始的设计循环的方式是AudioSource本身不循环，播放完之后重新播放，可以使用淡入淡出平滑循环。也就是循环播放。
    //问题1，在淡出停止的时候，音效可能马上播放完了，没有足够的时间淡出
    //问题2，重复播放有间隔，剩余会有短暂的间隙。
    //因此需要一个播放循环音效的功能（注意区别循环播放)。循环是AudioSource本身循环。目前就是循环音效
    //循环播放目前还是支持，但需要扩展出接口。需要的时候再改吧
    public class SoundItem
    {
        public int Id { get; private set; }
        public SoundLayer Layer {get; private set; }
        public string ResName { get; private set; }
        public AudioSource AudioSource { get; private set; }
        public bool IsLoop { get; private set; }
        public bool IsStopped { get; private set; }//停止了的在fadeOut之后不会在继续循环
        public float Duration { get; private set; }
        public bool isInPlayingList;

        //时间点[0............FadeInEnd............FadeOutStart............FadeOutEnd]
        //     |----FadeIn---|        |--Playing--|           |--FadeOut--|
        public float FadeinDura { get; private set; }
        public float FadeoutDura { get; private set; }
        public float FadeinEndTime { get; private set; }//正常结束的话是淡入时长。如果提前结束，就不是了。
        public float FadeoutStartTime { get; private set; }//正常结束的话是播放完成的时候。如果提前结束，就不是了。
        public float CurTime => IsPaused ? pauseTiming - playStartTime - pauseDuration :
            Time.time - playStartTime - pauseDuration;
        public float LeftPlayTime => FadeoutStartTime - CurTime;//Playing那一段的剩余播放时间。

        public bool IsLoaded => AudioSource.clip.loadState == AudioDataLoadState.Loaded;
        public bool IsFadeining => CurTime < FadeinEndTime;
        public bool IsPlaying
        {
            get
            {
                var time = CurTime;
                return time >= FadeinEndTime && time < FadeoutStartTime;
            }
        }
        public bool IsFadeouting => CurTime >= FadeoutStartTime;
        public bool IsPaused { get; private set; }

        private float volume;
        private float playStartTime;
        private SoundItemInfo info;
        private float pauseDuration;//计算暂停持续的时长
        private float pauseTiming;//暂停开始时刻。
        private SoundPlayer player;

        public float Volume
        {
            get => volume;
            set
            {
                //渐变过程中会自动设置AudioSource的音量
                var time = CurTime;
                if(time >= FadeinDura && time <= FadeoutStartTime)
                {
                    AudioSource.volume = player.GetFinalVolume(value);
                }
                volume = value;
            }
        }
        
        public SoundItem(AudioSource s)
        {
            AudioSource = s;
        }

        public void Init(SoundPlayer player, string resName, int id, AudioClip clip, bool loop)
        {
            Init(player, resName, id, clip, loop, 1, 0, 0, 0);
        }

        public void Init(SoundPlayer player, string resName, int id, AudioClip clip, bool loop, float volume, float duration, float fadein, float fadeout)
        {
            AudioSource.enabled = true;
            AudioSource.clip = clip;
            AudioSource.time = 0;
            AudioSource.loop = loop;
            ResName = resName;
            Duration = duration == 0 ? (loop ? 1080000 : clip.length) : duration; //300天=3600*30=1080000秒

            if (fadein > Duration)
            {
                fadein = Duration;
                Debug.LogWarning("fadein time is longer than duration");
            }
            if (fadeout > Duration)
            {
                fadeout = Duration;
                Debug.LogWarning("fadeout time is longer than duration");
            }

            this.player = player;
            Layer = player.Layer;
            Id = id;
            IsLoop = loop;
            IsStopped = false;
            this.volume = volume;
            FadeinDura = fadein;
            FadeoutDura = fadeout;
            FadeinEndTime = fadein;
            FadeoutStartTime = Duration - fadeout;
            AudioSource.volume = fadein > 0 ? 0 : player.GetFinalVolume(volume);
            isInPlayingList = false;
            info = null;
            pauseDuration = 0;
            pauseTiming = 0;
            IsPaused = false;
        }

        public void Play()
        {
            var state = AudioSource.clip.loadState;
            if(state == AudioDataLoadState.Loaded)
            {
                AudioSource.Play();
                playStartTime = Time.time;
            }
            else if(state == AudioDataLoadState.Failed)
            {
                Debug.LogError("audio clip load failed " + ResName);
            }
        }

        public void Pause()
        {
            IsPaused = true;
            pauseTiming = Time.time;
            AudioSource.Pause();
        }

        public void Resume()
        {
            if (IsPaused)
            {
                IsPaused = false;
                pauseDuration += Time.time - pauseTiming;//重复Resume这里会出问题
                AudioSource.UnPause();
            }
        }

        //返回是否完成
        public bool TickFadeIn(float deltaTime)
        {
            if(FadeinDura <= 0)
            {
                return true;
            }

            var time = CurTime;
            AudioSource.volume = (time / FadeinDura) * player.GetFinalVolume(volume);
            return time >= FadeinEndTime;
        }

        //返回是否完成
        public bool TickFadeOut(float deltaTime)
        {
            if(FadeoutDura <= 0)
            {
                return true;
            }

            var t = CurTime - FadeoutStartTime;
            AudioSource.volume = Volume - t / FadeoutDura * player.GetFinalVolume(volume);
            return t >= FadeoutDura;
        }

        public void Clear()
        {
            AudioSource.clip = null;
            AudioSource.enabled = false;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(AudioSource);
            AudioSource = null;
        }

        //不要直接调用，会导致player的playing列表不是有序的。
        //
        public void Stop(float fadeout = -1)
        {
            IsStopped = true;
            if(IsFadeouting)
            {
                return;
            }
            //立即停止淡入
            FadeinEndTime = CurTime;

            //立即开始淡出
            FadeoutDura = fadeout == -1 ? this.FadeoutDura : fadeout;
            FadeoutStartTime = CurTime;
            var t = Duration - CurTime;
            if(t < FadeoutDura)
            {
                FadeoutDura = t;
            }
            volume = AudioSource.volume;//可能正在淡入中，音量改成当前音量
        }

        public SoundItemInfo GetInfo()
        {
            if (info == null)
            {
                info = new SoundItemInfo(Id, Layer);
            }

            return info;
        }

        //fade out结束时调用
        public void OnFinish()
        {
            info?.finishCallback?.Invoke();
        }
    }

    //用来返回播放的声音信息，打包放一起，传参的话太多了
    public class SoundItemInfo
    {
        public int id;
        public SoundLayer layer;
        public Action finishCallback; //fade out结束时调用
        public SoundItemInfo(int id, SoundLayer layer)
        {
            this.id = id;
            this.layer = layer;
        }
    }
}