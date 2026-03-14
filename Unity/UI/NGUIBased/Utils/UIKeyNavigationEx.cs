#if NGUI
namespace GameCore.Unity.UI.NGUIEx
{
    public class UIKeyNavigationEx : UIKeyNavigation
    {
        private UIKeyNavigationContainer __container;

        public UIKeyNavigationContainer Container
        {
            get
            {
                if (__container == null)
                {
                    __container = GetComponentInParent<UIKeyNavigationContainer>(true);
                }
                return __container;
            }
        }
        public override void Start()
        {
            constraint = Constraint.Explicit;
            base.Start();
            if (Container != null)
            {
                Container.dict.TryAdd(gameObject, this);//UIKeyNavigation可能会触发一次
            }
        }

        protected void Update()
        {
            if (Container != null)
            {
                Container.dict.TryAdd(gameObject, this);
                enabled = false;
            }
        }

        protected void OnDestroy()
        {
            if (Container != null)
            {
                Container.dict.Remove(gameObject);
            }
        }
    }
}
#endif
