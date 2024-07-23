using System;
using UnityEngine;

namespace Kernel.Unity
{
    public class Touch_TwoFingerZoom
    {
        private float lastTouchDist; //上次的手指间距
        private bool isScalingStart; //是否开始缩放了
        private Action<float> callback;

        public Touch_TwoFingerZoom(Action<float> callback)
        {
            this.callback = callback;
        }

        public void Tick(float deltaTime)
        {
            if (Input.touchCount != 2)
            {
                isScalingStart = false;
                return;
            }

            var touchPos1 = Input.GetTouch(0).position;
            var touchPos2 = Input.GetTouch(1).position;
            //var touchPos1 = Vector3(Input.mousePosition.x, Input.mousePosition.y);//编辑器测试
            //var touchPos2 = Vector3(0, 0);//编辑器测试
            var dist = Vector2.Distance(touchPos1, touchPos2);

            if (isScalingStart)
            {
                var delta = dist - lastTouchDist;
                callback(delta);
            }
            else
            {
                isScalingStart = true;
            }
            lastTouchDist = dist;
        }
    }
}
