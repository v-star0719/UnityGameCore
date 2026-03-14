using GameCore.Unity.UI.UGUIEx;
using GameCore.Unity.UI;

namespace GameCore.Unity.Scene
{
    public class SceneBase
    {
        public int type;
        public PanelBaseCore MainPanel { get; protected set; }

        public virtual bool IsLoaded => true;

        //场景初始化，在Load之前执行
        public virtual void Init(SceneArgBase d)
        {
        }

        //开始加载场景
        public virtual void Load()
        {

        }

        //场景加载完成，开始运行
        public virtual void Start()
        {

        }

        //场景运行
        public virtual void Tick(float deltaTime)
        {

        }

        //场景销毁
        public virtual void Destroy()
        {

        }

        //ESC键或者手柄的取消键
        public virtual void OnCancelAxis()
        {

        }

        //Enter键或者手柄的确认键
        public virtual void OnSubmitAxis()
        {

        }
    }

    public class SceneArgBase
    {
        public int type;
        public LoadingPanelBase.Argument loadingPanelArg;
    }
}