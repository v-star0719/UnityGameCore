using System;
using GameCore.Core;
using GameCore.Unity.UGUIEx;

namespace GameCore.Unity
{
    public class SceneManagerBase
    {
        public enum StateType
        {
            None,
            Loading,
            Running,
            Unloading,
        }

        private const int WAIT = 2;

        public SceneBase CurScene { get; private set; }

        public StateType State { get; private set; }
        public int timer;
        public SceneArgBase newSceneArg;
        public LoadingPanelBase loadingPanel;

        public virtual void LoadScene(SceneArgBase arg)
        {
            newSceneArg = arg;//判断这个参数不是null就自动切换场景。分帧执行，确保上一个场景已经卸载完毕
            if (arg.loadingPanelArg != null)
            {
                loadingPanel = PanelManager.Top.OpenPanel("LoadingPanel", arg.loadingPanelArg) as LoadingPanelBase;
            }
        }

        public void Tick(float deltaTime)
        {
            switch (State)
            {
                case StateType.None:
                    if (newSceneArg != null)
                    {
                        CurScene = CreateScene(newSceneArg.type);
                        if (CurScene == null)
                        {
                            LoggerX.Error("场景未实现：" + newSceneArg.type);
                            return;
                        }
                        CurScene.Init(newSceneArg);
                        CurScene.Load();
                        newSceneArg = null;
                        ChangeState(StateType.Loading);
                    }
                    break;
                case StateType.Loading:
                    timer++;
                    if (timer >= WAIT && CurScene.IsLoaded)
                    {
                        ChangeState(StateType.Running);
                        CurScene.Start();
                        if (loadingPanel != null)
                        {
                            PanelManager.Top.ClosePanel(loadingPanel);
                        }
                    }
                    break;
                case StateType.Running:
                    CurScene?.Tick(deltaTime);
                    if (newSceneArg != null && (loadingPanel == null || !loadingPanel.IsCapturingScreen))
                    {
                        ChangeState(StateType.Unloading);
                        CurScene.Destroy();
                        CurScene = null;
                    }
                    break;
                case StateType.Unloading:
                    timer++;
                    if (timer >= WAIT)
                    {
                        ChangeState(StateType.None);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //ESC键或者手柄的取消键
        //默认是先关闭打开的界面，当只剩余主界面时，由场景决定怎么处理
        public virtual void OnCancelAxis()
        {
            if(CurScene == null)
            {
                return;
            }
            var topPanel = PanelManager.Normal.GetTopPanel();
            if (topPanel != CurScene.MainPanel)
            {
                if (topPanel.Age > 0.5f)//按下回车打开面板，可能立刻又把面板关闭
                {
                    topPanel.OnCancelAxis();
                }
            }
            else
            {
                CurScene.OnCancelAxis();
            }
        }

        //回车键键或者手柄的确认键
        public virtual void OnSubmitAxis()
        {
            if(CurScene == null)
            {
                return;
            }
            var topPanel = PanelManager.Normal.GetTopPanel();
            if(topPanel != CurScene.MainPanel)
            {
                if (topPanel.Age > 0.5f)//按下回车打开面板，可能立刻又把面板关闭
                {
                    topPanel.OnSubmitAxis();
                }
            }
            else
            {
                CurScene.OnSubmitAxis();
            }
        }

        private void ChangeState(StateType t)
        {
            State = t;
            timer = 0;
        }

        protected virtual SceneBase CreateScene(int type)
        {
            return null;
        }
    }
}