using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Unity
{
    public class SoundPlayer
    {
        protected bool paused = false;
        protected bool soundEnable = true;
        protected float pitch = 1;
        protected float volume = 1f;
        protected  float volumeScale = 1f;

        //播放一个音效的流程：Play-->Loading-->FadeIn-->Playing-->FadeOut-->Dead
        //对于没有淡入淡出的声音也强制走一次fadein和fadeout流程，简化代码。不然得有很多if。比如有淡入进fadein列表，没有的直接进loop队列等等等。
        //如果没有淡出的loop音效播放完毕后直接回到playing列表，需要先删除，然后重新插入，遍历playering列表时适合删除的，但不适合插入。
        //stop不是立即停止播放，如果有淡出效果，会放进fadeout列表。loop的声音stop后，不会再循环了。
        //clear是立即停止播放，清理所有数据
        public List<SoundItem> LoadingSounds { get; private set; } = new List<SoundItem>(16); //正在加载的声音
        public List<SoundItem> FadeInSounds { get; private set; } = new List<SoundItem>(16); //淡入的声音
        public List<SoundItem> PlayingSounds { get; private set; } = new List<SoundItem>(32); //播放中的声音。有序的，结束时间早的排在后面
        public List<SoundItem> FadeOutSounds { get; private set; } = new List<SoundItem>(16); //淡出的声音
        public Queue<SoundItem> DeadSoundDatas { get; private set; } = new Queue<SoundItem>(16); //回收的空壳
        public Dictionary<string, int> AliveCounter { get; private set; } = new();//存活的声音计数，用来控制同时播放的声音数量

        protected Dictionary<int, SoundItem> aliveSounds = new Dictionary<int, SoundItem>();//所有存活的
        protected SoundManager mgr;
        protected GameObject go;
        public SoundLayer Layer { get; private set; }
        public Action<SoundItem> onPlayFinished;

        //整个player的音量。是一个缩放因子。sound音量*Volume*VolumeScale=实际音量
        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                foreach (var sound in aliveSounds.Values)
                {
                    sound.Volume = sound.Volume;//刷新一下实际值就行了
                }
            }
        }

        //这是另一个音量调整因子。目前用来做整个player的淡入淡出。sound音量*Volume*VolumeScale=实际音量
        public float VolumeScale
        {
            get => volumeScale;
            set
            {
                volumeScale = value;
                Volume = volume;
            }
        }
        
        public virtual bool SoundEnable
        {
            get => soundEnable;
            set
            {
                if(value == soundEnable)
                {
                    return;
                }

                soundEnable = value;
                if(!soundEnable)
                {
                    StopAllSounds();
                }
            }
        }

        public float Pitch
        {
            get => pitch;
            set
            {
                pitch = value;
            }
        }

        public int AliveCount => aliveSounds.Count;

        public SoundPlayer(SoundManager m, SoundLayer l)
        {
            mgr = m;
            Layer = l;
            go = new GameObject(Layer.ToString());
            go.transform.parent = m.transform;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
        }

        public void Udpate()
        {
            if (paused)
            {
                return;
            }
            float deltaTime = Time.deltaTime;
            for(var i = LoadingSounds.Count - 1; i >= 0; i--)
            {
                var sound = LoadingSounds[i];
                if(sound.IsLoaded)
                {
                    LoadingSounds.RemoveAt(i);
                    FadeInSounds.Add(sound);
                    sound.Play();
                }
            }

            for(var i = FadeInSounds.Count - 1; i >= 0; i--)
            {
                var sound = FadeInSounds[i];
                if(sound.TickFadeIn(deltaTime))
                {
                    InsertToPlayingList(sound);
                    FadeInSounds.RemoveAt(i);
                }
            }

            for(var i = PlayingSounds.Count - 1; i >= 0; i--)
            {
                var sound = PlayingSounds[i];
                if(!sound.IsPlaying)
                {
                    sound.isInPlayingList = false;
                    FadeOutSounds.Add(sound);
                    PlayingSounds.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            for(var i = FadeOutSounds.Count - 1; i >= 0; i--)
            {
                var sound = FadeOutSounds[i];
                if(sound.TickFadeOut(deltaTime))
                {
                    FadeOutSounds.RemoveAt(i);
                    OnSoundPlayFinished(sound);
                }
            }
        }

        public float GetFinalVolume(float v)
        {
            return v * Volume * VolumeScale;
        }

        public void Clear()
        {
            ClearSoundList(LoadingSounds);
            ClearSoundList(PlayingSounds);
            ClearSoundList(FadeInSounds);
            ClearSoundList(FadeOutSounds);
            foreach(SoundItem soundData in DeadSoundDatas)
            {
                soundData.Destroy();
            }
            DeadSoundDatas.Clear();
        }

        public void Pause()
        {
            paused = true;
            foreach(var data in PlayingSounds)
            {
                data.Pause();
            }

            foreach(var data in FadeInSounds)
            {
                data.Pause();
            }

            foreach(var data in FadeOutSounds)
            {
                data.Pause();
            }
        }

        public void Resume()
        {
            paused = false;
            foreach(var data in PlayingSounds)
            {
                data.Resume();
            }

            foreach(var data in FadeInSounds)
            {
                data.Resume();
            }

            foreach(var data in FadeOutSounds)
            {
                data.Resume();
            }
        }

        ///volume上是一个最大声音，实际播放出来多大声音，受player的音量调节影响
        ///duration默认值：循环音效是永久，非循环音效是音效时长
        public virtual SoundItemInfo Play(string name, bool loop, float volume, float duration, float fadein, float fadeout, int maxCount = 0)
        {
            if(!soundEnable)
            {
                return null;
            }

            var aliveCount = AliveCounter.GetValueOrDefault(name, 0);
            if (maxCount > 0 && maxCount <= aliveCount)
            {
                return null;
            }

            var clip = mgr.GetAudioClip(name);
            if (clip == null)
            {
                return null;
            }

            var soundData = GetSoundData(name);
            soundData.Init(this, name, mgr.NextId, clip, loop, volume, duration, fadein, fadeout);
            LoadingSounds.Add(soundData);
            aliveSounds.Add(soundData.Id, soundData);
            AliveCounter[name] = aliveCount + 1;
            return soundData.GetInfo();
        }

        //fadeount:-1表示用play时指定的时长。0表示不淡出
        public void Stop(int id, float fadeout = -1)
        {
            if (aliveSounds.TryGetValue(id, out var item))
            {
                item.Stop(fadeout);
                //挪到播放列表最后，走结束播放的流程
                if (item.isInPlayingList)
                {
                    PlayingSounds.Remove(item);
                    PlayingSounds.Add(item);
                }
            }
        }

        public virtual void StopAllSounds()
        {
            StopAllSoundList(LoadingSounds, -1);
            StopAllSoundList(PlayingSounds, 1);
            StopAllSoundList(FadeInSounds, 1);
        }

        public SoundItem GetSoundItem(SoundItemInfo info)
        {
            return aliveSounds.GetValueOrDefault(info.id, null);
        }

        private void InsertToPlayingList(SoundItem soundItem)
        {
            soundItem.isInPlayingList = true;
            if (PlayingSounds.Count == 0)
            {
                PlayingSounds.Add(soundItem);
                return;
            }

            //LeftPlayTime小的排在后面
            //5,4,3,2,1
            var value = soundItem.LeftPlayTime;

            //加的前面
            if (value >= PlayingSounds[0].LeftPlayTime)
            {
                PlayingSounds.Insert(0, soundItem);
                return;
            }

            //加到后面
            if (value < PlayingSounds[^1].LeftPlayTime)
            {
                PlayingSounds.Add(soundItem);
                return;
            }

            //二分法确定位置
            int left = 0;
            int right = PlayingSounds.Count - 1;
            var mid = 0;
            while(left <= right)
            {
                mid = (left + right) >> 1;
                var midValue = PlayingSounds[mid].LeftPlayTime;
                if(midValue > value)
                {
                    left = mid + 1;
                }
                else if(midValue < value)
                {
                    right = mid - 1;
                }
                else
                {
                    break;
                }
            }

            if (left <= right)
            {
                //找到了值一样的
                PlayingSounds.Insert(mid, soundItem);//值相等，把上一个顶后面去。
            }
            else
            {
                //没找到，那么值位于[right], [left]之间，right < left
                //插入到left的位置，把left的元素顶到后面，它比要插入的小
                PlayingSounds.Insert(left, soundItem);
            }
        }

        protected virtual void OnSoundPlayFinished(SoundItem sound)
        {
            sound.OnFinish();
            if(sound.IsLoop && !sound.IsStopped)
            {
                LoadingSounds.Add(sound);
            }
            else
            {
                RecycleSoundData(sound);
                aliveSounds.Remove(sound.Id);
            }
            onPlayFinished?.Invoke(sound);
        }

        private SoundItem GetSoundData(string name)
        {
            SoundItem rt;
            if(DeadSoundDatas.Count > 0)
            {
                rt = DeadSoundDatas.Dequeue();
            }
            else
            {
                rt = new SoundItem(go.AddComponent<AudioSource>());
            }
            return rt;
        }

        private void RecycleSoundData(SoundItem sound)
        {
            sound.Clear();
            DeadSoundDatas.Enqueue(sound);
            AliveCounter[sound.ResName] -= 1;
        }

        private void StopAllSoundList(List<SoundItem> list, float fadeout = -1)
        {
            foreach(var sound in list)
            {
                if(fadeout > 0 && sound.IsLoaded)
                {
                    sound.Stop(fadeout);
                    FadeOutSounds.Add(sound);
                }
                else
                {
                    RecycleSoundData(sound);
                }
            }
            list.Clear();
        }

        private void ClearSoundList(List<SoundItem> list)
        {
            for(var i = list.Count - 1; i >= 0; i--)
            {
                var data = list[i];
                data.Destroy();
            }
            list.Clear();
        }
    }
}