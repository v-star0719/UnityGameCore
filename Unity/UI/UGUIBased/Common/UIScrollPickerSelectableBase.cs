using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    public class UIScrollPickerSelectableBase : Selectable, ISubmitHandler
    {
        public GameObject selectedGo;

        protected UIScrollPickerBase picker;
        protected override void Awake()
        {
            base.Awake();
            picker = GetComponent<UIScrollPickerBase>();
            if (selectedGo != null)
            {
                selectedGo.SetActive(false);
            }
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (selectedGo != null)
            {
                selectedGo.SetActive(true);
            }
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (selectedGo != null)
            {
                selectedGo.SetActive(false);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            var item = picker.CurItem;
            if (item != null)
            {
                item.OnPointerClick(null);
            }
        }
    }
}

