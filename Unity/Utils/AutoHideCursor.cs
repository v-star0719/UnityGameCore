using UnityEngine;

namespace GameCore.Unity
{
    //鼠标静止一段时间后隐藏，移动了就显示
    public class AutoHideCursor : MonoBehaviour
    {
        private enum State
        {
            Hidden,
            Hiding,
            Showed,
        }
        public float hideDelay = 1f;
        public float checkInterval = 0.3f;//连续两帧鼠标位置可能是一样的
        private State state;
        private Vector3 lastMousePos;
        private float timer;

        private void Start()
        {
            state = Cursor.visible ? State.Showed : State.Hidden;
            lastMousePos = Input.mousePosition;
        }

        private void Update()
        {
            switch (state)
            {
                case State.Hidden:
                    if (lastMousePos != Input.mousePosition)
                    {
                        ChangeState(State.Showed);
                    }
                    break;

                case State.Hiding:
                    if (lastMousePos == Input.mousePosition)
                    {
                        timer += Time.deltaTime;
                        if(timer >= hideDelay)
                        {
                            ChangeState(State.Hidden);
                        }
                    }
                    else
                    {
                        ChangeState(State.Showed);
                    }

                    break;

                case State.Showed:
                    timer += Time.deltaTime;
                    if(timer < checkInterval)
                    {
                        break;
                    }

                    if(lastMousePos == Input.mousePosition)
                    {
                        ChangeState(State.Hiding);
                    }
                    else
                    {
                        timer = 0;
                        lastMousePos = Input.mousePosition;
                    }
                    break;
            }
        }

        public void OnDestroy()
        {
            Cursor.visible = true;
        }

        private void ChangeState(State s)
        {
            state = s;
            timer = 0;
            Cursor.visible = s != State.Hidden;
            //Debug.Log($"AutoHideCursor: {s}");
        }
    }
}