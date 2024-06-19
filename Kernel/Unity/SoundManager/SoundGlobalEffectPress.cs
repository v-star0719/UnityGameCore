using System;

namespace Kernel.Unity
{
    //把层级比top低的音效压低
    //timer在Press的时候是加，release的时候是减。这样press中途就开始release可以丝滑接上。反过来同理。
    public class SoundGlobalEffectPress : SoundGlobalEffectBase
    {
        private enum State
        {
            Idle,
            Press,
            Release,
        }

        private const float DURA = 2;
        private const float FORCE = 0.3f;//最低音量
        private State state;

        public SoundGlobalEffectPress(SoundManager mgr):base(mgr)
        {
        }

        public override void Start()
        {
            state = State.Press;
        }

        public override void Stop()
        {
            state = State.Release;
        }

        public override bool Tick(float deltaTime)
        {
            switch (state)
            {
                case State.Press:
                    TickPress(deltaTime);
                    if (timer >= DURA)
                    {
                        state = State.Idle;
                    }
                    return false;

                case State.Idle:
                    return false;

                case State.Release:
                    TickRelease(deltaTime);
                    return timer <= 0;

                default:
                    return false;
            }
        }

        private void TickPress(float deltaTime)
        {
            timer += deltaTime;
            int n = (int)SoundLayer.top;
            for (int i = 0; i < n; i++)
            {
                manager.Players[i].VolumeScale = 1 - timer / DURA * (1 - FORCE);
            }
        }

        private void TickRelease(float deltaTime)
        {
            timer -= deltaTime;
            int n = (int)SoundLayer.top;
            for (int i = 0; i < n; i++)
            {
                manager.Players[i].VolumeScale = 1 - timer / DURA * (1 - FORCE);
            }
        }
    }
}
