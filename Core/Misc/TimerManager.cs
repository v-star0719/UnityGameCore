using System;
using System.Collections.Generic;
using GameCore.Lang.Extension;
using UnityEngine;

namespace GameCore.Core
{
    public class TimerManager : Singleton<TimerManager>
    {
        protected class Timer
        {
            public int id;
            public int leftTimes;
            public float time;
            public float interval;
            public float delay;
            public Action onTick;//初始计时为0的时候不会触发tick
            public Action onFinish;

            public Timer(int id, float delay, float interval, int times = 1, Action onTick = null, Action onFinish = null)
            {
                this.id = id;
                this.delay = delay;
                this.interval = interval;
                this.leftTimes = times;
                this.onTick = onTick;
                this.onFinish = onFinish;
                time = interval + delay;
            }
        }

        private List<Timer> timers = new List<Timer>();//同时运行的计时器数量较少时（低于30），直接遍历就行。如果数量较多，可用堆。
        private int id = 0;

        public int Add(float delay, Action onFinish)
        {
            var timer = new Timer(id++, delay, 0, 1, null, onFinish);
            timers.Add(timer);
            return timer.id;
        }

        //timers <= 0时是无限次
        public int Add(float delay, float interval, int times = 1, Action onTick = null, Action onFinish = null)
        {
            var timer = new Timer(id++, delay, interval, times, onTick, onFinish);
            timers.Add(timer);
            return timer.id;
        }

        public void Del(int id)
        {
            for (var i = timers.Count - 1; i >= 0; i--)
            {
                var timer = timers[i];
                if (timer.id == id)
                {
                    timers.FastRemove(i);
                    break;
                }
            }
        }

        public void Tick(float deltaTime)
        {
            for(var i = timers.Count - 1; i >= 0; i--)
            {
                var timer = timers[i];
                timer.time -= deltaTime;
                if (timer.time <= 0)
                {
                    timer.onTick?.Invoke();
                    timer.leftTimes--;
                    if (timer.leftTimes == 0)
                    {
                        timer.onFinish?.Invoke();
                        timers.FastRemove(i);
                    }
                    else
                    {
                        timer.time += timer.interval;
                    }
                }
            }
        }

        public void Clear()
        {
            timers.Clear();
        }
    }
}