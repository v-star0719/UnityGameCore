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
                var f = TouchUtils.EaseOut(timer / SMOOTH_TIME);
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
