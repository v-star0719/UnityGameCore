using Misc;
using TMPro;
using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    public class UIL10n : MonoBehaviour, IL10nMsgReceiver
    {
        public string key;

        private int __id;
        private TMP_Text text;

        public int Id
        {
            get => __id;
            set => __id = value;
        }

        private void Start()
        {
            text = GetComponent<TMP_Text>();
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
            text.text = L10n.Inst.Get(key);
        }
    }
}
