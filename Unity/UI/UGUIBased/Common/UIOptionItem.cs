using System;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace GameCore.Unity.UGUIEx
{
    public class UIOptionItem : UIGridExItemT<UIOptionItem.Data>
    {
        public class Data : IGridExData
        {
            public int GridId => id;
            public int id;
            public string text;
        }

        public TMP_Text text;

        public override void SetData(IGridExData d)
        {
            base.SetData(d);

            text.text = data.text;
        }

        public override void OnSelectX(bool isClick)
        {
            base.OnSelectX(isClick);
            if (isClick)
            {
                GetComponentInParent<UIOptionUI>().OnItemSelect(this);
            }
        }
    }
}
