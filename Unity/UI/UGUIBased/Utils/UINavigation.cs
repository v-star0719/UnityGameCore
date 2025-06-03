using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class UINavigation : MonoBehaviour
    {
        private UINavigationContainer __container;
        private Selectable __selectable;
        public bool startsSelected;

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

            var v = s.navigation;
            v.mode = b ? Navigation.Mode.Automatic : Navigation.Mode.None;
            Selectable.navigation = v;
        }
    }
}