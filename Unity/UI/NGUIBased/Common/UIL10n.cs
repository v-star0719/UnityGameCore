using Misc;
using UnityEngine;

#if NGUI

namespace GameCore.Unity.NGUIEx
{
    public class UIL10n : MonoBehaviour, IL10nMsgReceiver
    {
        public string key;

        private int __id;
        private UILabel label;

        public int Id
        {
            get => __id;
            set => __id = value;
        }

        private void Start()
        {
            label = GetComponent<UILabel>();
            L10n.Inst.AddReceiver(this);
            OnL10nChange();
        }

        private void OnDestroy()
        {
            //没有Start也会触发OnDestroy
            if (Id > 0)
            {
                L10n.Inst.RemoveReceiver(this);
            }
        }

        public void OnL10nChange()
        {
            label.text = L10n.Inst.Get(key);
        }
    }
}
#endif
