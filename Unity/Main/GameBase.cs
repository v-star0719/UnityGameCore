using GameCore.Core;
using GameCore.Unity;
using UnityEngine;

namespace GameCore
{
    /// 游戏主干
    public class GameBase : MonoBehaviour
    {
        public Transform topUIRoot;
        public Transform normalUIRoot;
        private float checkInputTimer;

        protected virtual void Awake()
        {
            LoggerX.AddTarget(new LogToUnity());
        }
        
        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            var deltaTime = Time.deltaTime;
            EventManager.Inst.Tick(deltaTime);
            SceneManagerX.Inst.Tick(deltaTime);
            TimerManager.Inst.Tick(deltaTime);
            TickCheckCancelInput();
        }

        protected virtual void OnDestroy()
        {
            LoggerX.Clear();
        }

        //ESC键或者手柄的取消键
        protected virtual void TickCheckCancelInput()
        {
            if (checkInputTimer < Time.time)
            {
                var cancel = Input.GetAxis(InputAxisName.cancel) > 0;
                var submit = Input.GetAxis(InputAxisName.submit) > 0;
                if (cancel || submit)
                {
                    checkInputTimer = Time.time + 0.2f;
                    if (cancel)
                    {
                        SceneManagerX.Inst.OnCancelAxis();
                    }
                    else
                    {
                        SceneManagerX.Inst.OnSubmitAxis();
                    }
                }
            }
        }
    }
}