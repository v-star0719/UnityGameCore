namespace GameCore.Unity
{
    public abstract class SoundGlobalEffectBase
    {
        protected SoundManager manager;
        protected float timer;
        public SoundGlobalEffectBase(SoundManager mgr)
        {
            manager = mgr;
        }

        public virtual void Start()
        {

        }

        public virtual void Stop()
        {

        }

        //返回是否完成
        public virtual bool Tick(float deltaTime)
        {
            return true;
        }
    }
}
