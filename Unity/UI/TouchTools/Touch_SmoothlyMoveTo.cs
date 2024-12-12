using System;

namespace GameCore.Unity
{
    //这个和触摸操作一般没啥关系，但是一般会同时实现这个功能，就放这一堆吧
    public class Touch_SmoothlyMoveTo
    {
        private float timer;
        private float fromX;
        private float fromY;
        private float toX;
        private float toY;
        private float duration;
        private Action<float, float> callback;
        private Action finishCallback;

        public Touch_SmoothlyMoveTo(Action<float, float> callback, Action finishCallback)
        {
            timer = -1;
            this.callback = callback;
            this.finishCallback = finishCallback;
        }

        public void StartMove(float duration, float fromX, float fromY, float toX, float toY)
        {
            if (duration < 0.1f)
            {
                duration = 0.1f;
            }

            this.duration = duration;
            this.timer = duration;
            this.fromX = fromX;
            this.fromY = fromY;
            this.toX = toX;
            this.toY = toY;
        }

        public void Tick(float deltaTime)
        {
            if (timer > 0)
            {
                timer -= deltaTime;
                var f = timer / duration;
                if (f < 0)
                {
                    f = 0;
                }

                var x = fromX + (toX - fromX) * f;
                var y = fromY + (toY - fromY) * f;
                callback(x, y);
                if (timer <= 0)
                {
                    finishCallback?.Invoke();
                }
            }
        }

        public void Stop()
        {
            timer = -1;
        }
    }
}
