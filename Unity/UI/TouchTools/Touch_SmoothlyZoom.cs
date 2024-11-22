using System;
using Kernel.Unity;

namespace UKernel.Unity
{
    public class Touch_SmoothlyZoom
    {
        private const float SMOOTH_TIME = 0.5f;
        private float timer;
        private float lastZoomDelta;
        private Action<float> callback;

        public Touch_SmoothlyZoom(Action<float> callback)
        {
            timer = -1;
            this.callback = callback;
        }

        public void Tick(float deltaTime)
        {
            if (timer > 0)
            {
                timer -= deltaTime;
                //(1 - x) * (1 - x)
                var f = timer / SMOOTH_TIME;
                f = 1 - f;
                f *= f;
                callback(f * lastZoomDelta);
            }
        }

        public void SetLastZoomDelta(float delta)
        {
            timer = SMOOTH_TIME;
            lastZoomDelta = delta;
        }

        public void Stop()
        {
            timer = 0;
        }
    }
}
