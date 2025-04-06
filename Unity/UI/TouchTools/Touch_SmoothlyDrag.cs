using System;

namespace GameCore.Unity
{
    public class Touch_SmoothlyDrag
    {
        private const float SMOOTH_TIME = 0.5f;
        private float lastDragDeltaX;
        private float lastDragDeltaY;
        private float timer; //在一定时间内将lastDragDelta衰减到0
        private Action<float, float> callback;
        private bool start = false;

        public Touch_SmoothlyDrag(Action<float, float> callback)
        {
            timer = -1;
            lastDragDeltaX = 0;
            lastDragDeltaY = 0;
            this.callback = callback;
        }
        
        public void Tick(float deltaTime)
        {
            timer -= deltaTime;//拖动后按住不动也进行衰减处理，避免按住不动后松开还滑动很远

            if(!start)
            {
                return;
            }

            if(timer > 0)
            {
                var f = TouchUtils.EaseIn(timer / SMOOTH_TIME);
                callback(f * lastDragDeltaX, f * lastDragDeltaY);
            }
            else
            {
                start = false;
            }
        }

        public void SetLastDragDelta(float x, float y)
        {
            timer = SMOOTH_TIME;
            lastDragDeltaX = x;
            lastDragDeltaY = y;
        }

        public void Start()
        {
            start = true;
        }

        public void Stop()
        {
            timer = -1;
        }
    }
}
