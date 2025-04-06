using System;

namespace GameCore.Unity
{
    public class Touch_SmoothlyDrag
    {
        private const float SMOOTH_TIME = 0.5f;
        private float lastDragDeltaX;
        private float lastDragDeltaY;
        private float timer; //��һ��ʱ���ڽ�lastDragDelta˥����0
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
            timer -= deltaTime;//�϶���ס����Ҳ����˥���������ⰴס�������ɿ���������Զ

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
