namespace GameCore.Unity
{
    public class SceneBase
    {
        public int type;
        public PanelBaseCore MainPanel { get; protected set; }

        public virtual bool IsLoaded => true;

        public virtual void Init(SceneArgBase d)
        {
        }

        public virtual void Load()
        {

        }

        public virtual void Start()
        {

        }

        public virtual void Tick(float deltaTime)
        {

        }

        public virtual void Destroy()
        {

        }

        //ESC键或者手柄的取消键
        public virtual void OnCancelAxis()
        {

        }
    }

    public class SceneArgBase
    {
        public int type;
        public bool showLoadingPanel;
    }
}