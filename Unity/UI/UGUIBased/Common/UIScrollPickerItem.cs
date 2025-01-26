using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore.Unity.UGUIEx
{
    [ExecuteInEditMode]
    public class UIScrollPickerItem : MonoBehaviour, IPointerClickHandler
    {
        public int index = 0;

        public bool transitAlpha = false;
        public AnimationCurve transitAlphaCurve;
        public bool transitScale = false;
        public AnimationCurve transitScaleCurve;

        private UIScrollPickerBase picker;

        private CanvasGroup canvasGroup;
        
        public virtual void ShowTransit()
        {
            if (picker == null)
            {
                picker = GetComponentInParent<UIScrollPickerBase>();//编辑器里可能删除一个组件，换成另一个了
            }

            if (transitAlpha || transitScale)
            {
                var halfViewSize = picker.GetViewportSize() * 0.5f;
                var d = picker.GetDistToViewportCenter(transform);
                var f = Mathf.Clamp01((Mathf.Abs(d) - picker.pickAreaSize * 0.5f) / halfViewSize);
                if (transitAlpha)
                {

                    if(canvasGroup == null)
                    {
                        canvasGroup = GetComponent<CanvasGroup>();
                    }
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = transitAlphaCurve.Evaluate(f);
                    }
                    else
                    {
                        Debug.LogWarning("CanvasGroup is required for transit alpha");
                    }
                }
                if (transitScale)
                {
                    var s = transitScaleCurve.Evaluate(f);
                    transform.localScale = new Vector3(s, s, s);
                }
            }
        }

        public virtual void OnItemClicked()
        {
        }

        public void Start()
        {
            if (transitAlphaCurve == null)
            {
                transitAlphaCurve = AnimationCurve.Linear(0, 1, 1, 0);
            }
            if (transitScaleCurve == null)
            {
                transitScaleCurve = AnimationCurve.Linear(0, 1, 1, 0.8f);
            }

            picker = GetComponentInParent<UIScrollPickerBase>();
            picker.AddItem(this);
        }

        public void OnDestroy()
        {
            picker.RemoveItem(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            picker.Select(index);
        }

        public virtual void OnSelect()
        {
        }
    }
}
