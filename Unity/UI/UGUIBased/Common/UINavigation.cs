using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class UINavigation : MonoBehaviour
    {
        private UINavigationContainer __container;
        private Selectable __selectable;
        public bool startsSelected;
        private bool hasInitMode;
        private Navigation.Mode mode;

        public UINavigationContainer Container
        {
            get
            {
                if (__container == null)
                {
                    __container = GetComponentInParent<UINavigationContainer>(true);
                }
                return __container;
            }
        }

        public Selectable Selectable
        {
            get
            {
                if (__selectable == null)
                {
                    __selectable = GetComponent<Selectable>();
                }
                return __selectable;
            }
        }

        public void Start()
        {
            Container.dict.TryAdd(gameObject, this);
            mode = Selectable.navigation.mode;
            hasInitMode = true;
        }

        protected void OnDestroy()
        {
            if (Container != null)
            {
                Container.dict.Remove(gameObject);
            }
        }

        public void SetWorking(bool b)
        {
            var s = Selectable;
            if(s == null)
            {
                Debug.LogError("Selectable is required", gameObject);
                return;
            }

            //可能在Start前调用，先记录原始的配置
            if (!hasInitMode)
            {
                hasInitMode = true;
                mode = Selectable.navigation.mode;
            }

            var v = s.navigation;
            v.mode = b ? mode : Navigation.Mode.None;
            Selectable.navigation = v;
        }
    }
}