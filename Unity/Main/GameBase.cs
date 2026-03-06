using GameCore.Core.Logger;
using GameCore.Core.Misc;
using GameCore.Unity.Misc;
using GameCore.Unity.Scene;
using GameCore.Unity.UI;
using UnityEngine;

namespace GameCore.Unity.Main
{
    /// 游戏主干，控制游戏的整体运行
    /// Main game core, controls overall game flow.
    public class GameBase : MonoBehaviour
    {
        public Transform topUIRoot;
        public Transform normalUIRoot;

        //当面板堆叠时，面板之间的 [Canvas sort order] 间隔
        //Interval between panels' [Canvas sort order] when panels are stacked
        [SerializeField]private int panelDepthGap = 10;

        //需要设置sceneManager
        //You need to set up the SceneManager.
        protected SceneManagerBase sceneManager;

        //需要设置resManager
        //You need to set up the resManager.
        protected IResManager resManager;

        private float checkInputTimer;

        //需要在Start之前做的初始化工作
        //Initialization work that needs to be done before the Start method is executed
        protected virtual void Awake()
        {
            LoggerX.AddTarget(new LogToUnity());
        }
        
        protected virtual void Start()
        {
            PanelManager.InitTop(new GameObject("TopUIRoot").transform, resManager, 0, panelDepthGap);
            PanelManager.InitNormal(new GameObject("NormalUIRoot").transform, resManager, 0, panelDepthGap);
        }

        protected virtual void Update()
        {
            var deltaTime = Time.deltaTime;
            EventManager.Inst.Tick(deltaTime);
            sceneManager.Tick(deltaTime);
            TimerManager.Inst.Tick(deltaTime);
            TickCheckCancelInput();
        }

        protected virtual void OnDestroy()
        {
            LoggerX.Clear();
        }

        //ESC键或者手柄的取消键
        //What the ESC key or controller Cancel button does—close panel, cancel action, exit scene, or other function—is handled per scene.
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
                        sceneManager.OnCancelAxis();
                    }
                    else
                    {
                        sceneManager.OnSubmitAxis();
                    }
                }
            }
        }
    }
}