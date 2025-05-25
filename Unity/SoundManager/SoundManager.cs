using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameCore.Unity
{
    //将声音分层，每层一个sound player。这样就可以做出一个声音压过其他所有声音的效果了
    //支持AudioClip后台加载。
    //这里播放的都是固定位置的，如果是移动的声音，请自行管理带着它跑。
    //配套调试工具：SoundManagerDebugger
    public class SoundManager : MonoBehaviour
    {
        public const float DEFAULT_BKVOLUMN = 0.5f;
        public const float DEFAULT_SOUND_VOLUMN = 0.5f;
        public static SoundManager Instance { get; private set; }
        public static Func<string, AudioClip> loader;

        private float bgmVolume = DEFAULT_BKVOLUMN;
        private float soundVolume = DEFAULT_SOUND_VOLUMN;
        private bool soundEnable = true;
        private bool bgmEnable = true;
        private float pitch = 1;
        private int nextId = 0;

        private Dictionary<int, SoundFreeAudioSource> freeAudioSources = new Dictionary<int, SoundFreeAudioSource>();
        private Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        public SoundPlayer[] Players { get; private set; }
        private SoundGlobalEffectBase globalEffect;

        public float BgmVolume
        {
            get => bgmVolume;
            set
            {
                bgmVolume = value;
                Players[(int) SoundLayer.bgm].Volume = value;
            }
        }

        public float SoundVolume
        {
            get => soundVolume;
            set
            {
                soundVolume = value;
                Players[(int) SoundLayer.normal].Volume = value;
            }
        }

        public bool SoundEnable
        {
            get => soundEnable;
            set
            {
                if (value == soundEnable)
                {
                    return;
                }

                soundEnable = value;
                int n = (int) SoundLayer.top;
                for (int i = (int) SoundLayer.normal; i <= n; i++)
                {
                    Players[i].SoundEnable = value;
                }

                foreach (var source in freeAudioSources.Values)
                {
                    source.Source.enabled = value;
                }
            }
        }

        public bool BgmEnable
        {
            get => bgmEnable;
            set
            {
                if (value == bgmEnable)
                {
                    return;
                }

                bgmEnable = value;
                Players[(int) SoundLayer.bgm].SoundEnable = value;
            }
        }

        public float Pitch
        {
            get => pitch;
            set { pitch = value; }
        }

        public int NextId => ++nextId;

        public SoundPlayer NormalPlayer => Players[(int)SoundLayer.normal];
        public SoundPlayer TopPlayer => Players[(int)SoundLayer.top];
        public SoundPlayerBgm BgmPlayer => Players[(int)SoundLayer.bgm] as SoundPlayerBgm;

        #region MonoLife 

        void Awake()
        {
            if (Instance != null)
            {
                throw new Exception("multi Sound Manager running");
            }

            Instance = this;
            Players = new SoundPlayer[3];
            Players[(int) SoundLayer.bgm] = new SoundPlayerBgm(this, SoundLayer.bgm);
            Players[(int) SoundLayer.normal] = new SoundPlayer(this, SoundLayer.normal);
            Players[(int) SoundLayer.top] = new SoundPlayer(this, SoundLayer.top);

            Players[(int)SoundLayer.top].onPlayFinished = OnTopSoundPlayFinished;
        }

        void Start()
        {
        }

        void Update()
        {
            foreach (var player in Players)
            {
                player.Udpate();
            }

            if (globalEffect != null)
            {
                if (globalEffect.Tick(Time.deltaTime))
                {
                    globalEffect = null;
                }
            }
        }

        void OnDestroy()
        {
            Clear();
            Instance = null;
        }

#endregion

        public void Clear()
        {
            foreach (var player in Players)
            {
                player.Clear();
            }

            clips.Clear();
            globalEffect = null;
        }

        public void Pause()
        {
            foreach (var player in Players)
            {
                player.Pause();
            }
        }

        public void Resume()
        {
            foreach (var player in Players)
            {
                player.Resume();
            }
        }

        public void Warm(string name)
        {
            if (!soundEnable)
            {
                return;
            }

            GetAudioClip(name);
        }

        public SoundItemInfo PlaySound(SoundLayer layer, string name, bool loop, float volume, float duration = 0, float fadeIn = 0f, float fadeout = 0f, int maxCount = 0)
        {
            var player = Players[(int)layer];
            switch(layer)
            {
                case SoundLayer.bgm:
                    PlayBgm(new []{name}, volume, fadeIn, fadeout);
                    return null;

                case SoundLayer.normal:
                    return PlayNormalSound(name, loop, volume, duration, fadeIn, fadeout, maxCount);

                case SoundLayer.top:
                    return PlayTopSound(name, loop, volume, duration, fadeIn, fadeout, maxCount);
            }
            return null;
        }

        public void StopSound(SoundItemInfo sound, float fadeout = -1)
        {
            Players[(int)sound.layer].Stop(sound.id);
        }

        public void PlayBgm(string name, float volume = 1, float fadeIn = 2f, float fadeOut = 2f)
        {
            PlayBgm(new []{name}, volume, fadeIn, fadeOut);
        }

        public void PlayBgm(string[] names, float volume = 1, float fadeIn = 2f, float fadeOut = 2f)
        {
            var player = Players[(int)SoundLayer.bgm] as SoundPlayerBgm;
            player.Play(names, volume, fadeIn, fadeOut);
        }

        public SoundItemInfo PlayNormalSound(string name, bool loop = false, float volume = 1, float duration = 0, float fadein = 0f, float fadeout = 0f, int maxCount = 0)
        {
            var player = Players[(int)SoundLayer.normal];
            return player.Play(name, loop, volume, duration, fadein, fadeout, maxCount);
        }

        //顶层的声音播放的时候会将底层声音降低，让顶层的声音凸显出来
        public SoundItemInfo PlayTopSound(string name, bool loop = false, float volume = 1, float duration = 0, float fadein = 0f, float fadeout = 0f, int maxCount = 0)
        {
            var player = Players[(int)SoundLayer.top];
            if (globalEffect == null)
            {
                globalEffect = new SoundGlobalEffectPress(this);
            }
            globalEffect.Start();
            return player.Play(name, loop, volume, duration, fadein, fadeout, maxCount);
        }

        public void Stop()
        {
            foreach (var player in Players)
            {
                player.StopAllSounds();
            }
        }

        public AudioClip GetAudioClip(string name)
        {
            if (loader == null)
            {
                Debug.LogError("Loader has not been set");
                return null;
            }

            if (clips.TryGetValue(name, out var clip))
            {
                clip = clips[name];
            }

            if (clip == null)
            {
                clip = loader(name);
                if (clip != null)
                {
                    clips[name] = clip;
                }
                else
                {
                    Debug.LogWarning("AudioClip does not exist: " + name);
                }
            }

            clip?.LoadAudioData();
            return clip;
        }

        public void RegisterFreeAudioSource(SoundFreeAudioSource s)
        {
            s.id = nextId;
            freeAudioSources.Add(s.id, s);
            s.Source.enabled = soundEnable;
        }

        public void UnregisterFreeAudioSource(SoundFreeAudioSource s)
        {
            freeAudioSources.Remove(s.id);
        }

        private void OnTopSoundPlayFinished(SoundItem sound)
        {
            if (Players[(int)SoundLayer.top].AliveCount <= 0)
            {
                globalEffect?.Stop();
            }
        }
    }
}